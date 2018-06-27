using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Services.Upload
{
    public class UploadClient
    {
        const string DOWNLOAD_ULR_FORMAT = "{0}/saveFile.php";
        const string SESSION_KEY_FORMAT = "Bearer {0}";
        const int MAX_BUFFER_SIZE = 8 * 1024;
        const int RESUME_INCOMPLETE_STATUS_CODE = 308;
        const int SMALL_FILE_SIZE = 4 * 1024 * 1024; // files smaller than this will not be chunked
        static readonly byte[] _data = new byte[MAX_BUFFER_SIZE];

        readonly bool   _useGzip;
        readonly string _virtualPath;
        readonly string _localFilePath;
        readonly long    _syncpointId;
        
        readonly string _storageToken;
        readonly FileInfo _fileInfo;

        readonly AsyncCallback _callback;

        private string  _sessionKey;
        private string  _uploadUrl;
        private long    _chunkSize;
        private string  _eTag;
        private long    _startByte;
        private byte[]  _headerData;
        private byte[]  _boundaryData;
        private bool    _maxBytesPerSecondChanged;
        private bool    _shouldChunk;

        public UploadClientSettings UploadClientSettings { get; set; }

        public UploadClient(Folder folder, string localFilePath, string eTag, AsyncCallback finalCallback, UploadClientSettings uploadClientSettings, string storageToken)
        {
            _storageToken = storageToken;

            _useGzip = false;

            _syncpointId = folder.SyncpointId;
            _virtualPath = folder.VirtualPath + Path.GetFileName(localFilePath);
            _localFilePath = localFilePath;

            _callback = finalCallback;

            _fileInfo = new FileInfo(localFilePath);

            _eTag = eTag;
            UploadClientSettings = uploadClientSettings;
        }

        public UploadClient(Folder folder, string localFilePath, string eTag, AsyncCallback finalCallback, UploadClientSettings uploadClientSettings)
            : this(folder, localFilePath, eTag, finalCallback, uploadClientSettings, "")
        {
        }

        public UploadClient(Folder folder, string localFilePath, string eTag, AsyncCallback finalCallback)
            : this(folder, localFilePath, eTag, finalCallback, new UploadClientSettings())
        {
        }

        public string QueryFile()
        {
            if (!ShouldChunk())
            {
                // we'll avoid dealing with query/etag/chunking for this small file
                return null;
            }

            // Query the server to determine chunk upload status
            HttpWebRequest queryRequest = CreateQueryRequest();
            using (Stream queryRequestStream = CreateRequestStream(queryRequest))
            {
                WriteSessionKey(queryRequestStream);
                queryRequestStream.Write(_boundaryData, 0, _boundaryData.Length);
            }

            GetResponse(queryRequest);

            return _eTag;
        }

        public void UploadFile()
        {
            if (!_fileInfo.Exists)
            {
                throw new ApplicationException("File doesn't exist.");
            }

            Initialize();

            _shouldChunk = false;

            if (String.IsNullOrEmpty(_eTag) && ShouldChunk())
            {
                QueryFile();
            }

            using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                MAX_BUFFER_SIZE, FileOptions.SequentialScan))
            {
                do
                {
                    // the server sends us the startByte back after every chunk request. we'll reset
                    // our stream position here, inside the while loop to conceivably support future
                    // situations such as the server informing the client to restart an upload,
                    // upload chunks out of order, etc.
                    //
                    // for sequential chunk uploading this will be a nop.
                    fileStream.Position = Math.Min(fileStream.Length, _startByte);
                } while (UploadChunk(fileStream, true));
            }
        }

        public void UploadFileUsingChunks(bool waitforServerResponse, long? maxBytesToSend = null, int? maxChunksAllow = null, int? startChunk = null, long? actualStartByte = null)
        {
            if (!_fileInfo.Exists)
            {
                Console.WriteLine(@"File not exists.");
                return;
            }

            Initialize();

            _shouldChunk = true;

            if (String.IsNullOrEmpty(_eTag) && ShouldChunk())
            {
                QueryFile();
            }

            if (startChunk.HasValue)
            {
                _startByte = _chunkSize * (startChunk.Value - 1);
            }
            else if (actualStartByte.HasValue)
            {
                _startByte = actualStartByte.Value;
            }

            int chunkCount = 0;
            long? maxBytesToSendInChunk = null;

            using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                MAX_BUFFER_SIZE, FileOptions.SequentialScan))
            {
                do
                {
                    // the server sends us the startByte back after every chunk request. we'll reset
                    // our stream position here, inside the while loop to conceivably support future
                    // situations such as the server informing the client to restart an upload,
                    // upload chunks out of order, etc.
                    //
                    // for sequential chunk uploading this will be a nop.
                    fileStream.Position = Math.Min(fileStream.Length, _startByte);

                    if (maxBytesToSend.HasValue)
                    {
                        if (fileStream.Position < maxBytesToSend.Value)
                        {
                            maxBytesToSendInChunk = maxBytesToSend.Value - fileStream.Position;
                        }
                        else
                        {
                            maxBytesToSendInChunk = 0;
                        }
                    }
                    chunkCount++;
                    if (maxChunksAllow.HasValue && maxChunksAllow.Value < chunkCount)
                    {
                        break;
                    }
                } while (UploadChunk(fileStream, waitforServerResponse, maxBytesToSendInChunk));
            }
        }

        bool ShouldChunk()
        {
            return _shouldChunk;
        }

        void GetResponse(HttpWebRequest request)
        {
            try
            {
                request.GetResponse().Close();
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse &&
                    (int)((e.Response as HttpWebResponse).StatusCode) == RESUME_INCOMPLETE_STATUS_CODE)
                {
                    _eTag = e.Response.Headers.Get("ETag");
                    _startByte = GetStartByte(e.Response);

                    // if this fails FileUploader will catch the exception and mark the file as unable to upload
                    _chunkSize = Convert.ToInt64(e.Response.Headers.Get("Syncplicity-Chunk-Size"));
                }
                else
                {
                    throw;
                }
            }
        }

        private String GetSha256FromStream(FileStream fileStream)
        {
            String sha256String;

            using (var chunkSha265 = new SHA256Managed())
            {
                long bytesReadTotal = 0;
                int bufferSize = MAX_BUFFER_SIZE;
                var stopwatch = new Stopwatch();

                do
                {
                    if (_maxBytesPerSecondChanged)
                    {
                        _maxBytesPerSecondChanged = false;

                        bufferSize = MAX_BUFFER_SIZE;
                    }

                    if (_chunkSize > 0)
                    {
                        // clamp buffer size down if this last Read() of the chunk
                        bufferSize = Math.Min(bufferSize, (int)(_chunkSize - bytesReadTotal));
                    }

                    stopwatch.Start();

                    int bytesRead = fileStream.Read(_data, 0, bufferSize);
                    if (bytesRead == 0)
                    {
                        chunkSha265.TransformFinalBlock(_data, 0, bytesRead);
                        break;
                    }

                    bytesReadTotal += bytesRead;

                    // Hash data
                    chunkSha265.TransformBlock(_data, 0, bytesRead, null, 0);
                    stopwatch.Stop();

                    stopwatch.Reset();
                } 
                while (true);

                sha256String = ConvertByteArrayToHashString(chunkSha265.Hash);
            }

            fileStream.Seek(0, SeekOrigin.Begin);

            return sha256String;
        }

        // returns true if there are more chunks remaining, false if we've reached the end of the file
        bool UploadChunk(FileStream fileStream, bool waitForResponse, long? bytesReadTotalMax = null)
        {
            string sha256;
            bool finalChunk;
            long bytesReadTotal = 0;
            bool needForceStop = false;
            HttpWebRequest chunkRequest = CreateChunkUploadRequest(fileStream);

            using (Stream chunkRequestStream = CreateRequestStream(chunkRequest))
            {
                // Write the header data always at full speed
                chunkRequestStream.Write(_headerData, 0, _headerData.Length);

                _maxBytesPerSecondChanged = true;
                int bufferSize = MAX_BUFFER_SIZE;

                using (var chunkSha265 = new SHA256Managed())
                {
                    var stopwatch = new Stopwatch();

                    do
                    {
                        if (_maxBytesPerSecondChanged)
                        {
                            _maxBytesPerSecondChanged = false;
                            bufferSize = MAX_BUFFER_SIZE;
                        }

                        if (_chunkSize > 0)
                        {
                            // clamp buffer size down if this last Read() of the chunk
                            bufferSize = Math.Min(bufferSize, (int)(_chunkSize - bytesReadTotal));
                        }

                        stopwatch.Start();

                        int bytesRead = fileStream.Read(_data, 0, bufferSize);
                        if (bytesRead == 0 || (bytesReadTotalMax.HasValue && bytesReadTotal >= bytesReadTotalMax.Value))
                        {
                            if (bytesReadTotalMax.HasValue && bytesReadTotal >= bytesReadTotalMax.Value)
                            {
                                needForceStop = true;
                            }
                            chunkSha265.TransformFinalBlock(_data, 0, bytesRead);
                            break;
                        }

                        bytesReadTotal += bytesRead;

                        // Hash data
                        chunkSha265.TransformBlock(_data, 0, bytesRead, null, 0);

                        // Send chunk data
                        chunkRequestStream.Write(_data, 0, bytesRead);

                        stopwatch.Stop();
                        stopwatch.Reset();
                    } 
                    while (true);

                    sha256 = bytesReadTotal > 0 ? ConvertByteArrayToHashString(chunkSha265.Hash) : "";

                    if (!UploadClientSettings.UseQueryParams)
                    {
                        WriteSha256Data(chunkRequestStream, "sha256", bytesReadTotal > 0 ? chunkSha265.Hash : null);
                        WriteSessionKey(chunkRequestStream);
                        WriteVirtualFolderId(chunkRequestStream, _syncpointId);
                        WriteFileDates(chunkRequestStream);
                    }

                    finalChunk = fileStream.Position == fileStream.Length;

                    if (finalChunk)
                    {
                        WriteFormValuePair(chunkRequestStream, "fileDone", String.Empty);
                    }
                }

                chunkRequestStream.Write(_boundaryData, 0, _boundaryData.Length);
            }

            if (finalChunk)
            {
                // we've reached the end of the file, time to call our callback
                Console.WriteLine(@"Uploading file {0} with hash {1} to {2}", _localFilePath, sha256, chunkRequest.Address);
                Console.WriteLine("ChunkRequest Headers:\\r\\n{0}", chunkRequest.Headers);

                Debug.WriteLine("Uploading file {0} with hash {1} to {2}", _localFilePath, sha256, chunkRequest.Address);
                Debug.WriteLine("ChunkRequest Headers:\\r\\n{0}", chunkRequest.Headers);

                if (waitForResponse)
                {
                    chunkRequest.BeginGetResponse(_callback, CreateAsyncInfo(chunkRequest));
                }
                else
                {
                    chunkRequest.Abort();
                }
            }
            else
            {
                if (waitForResponse)
                {
                    GetResponse(chunkRequest);
                }
                else
                {
                    chunkRequest.Abort();
                }

                if (needForceStop)
                {
                    return false;
                }
            }

            return !finalChunk;
        }

        long GetStartByte(WebResponse response)
        {
            long startByte = 0;

            string range = response.Headers.Get("Range");
            if (!String.IsNullOrEmpty(range))
            {
                string[] rangeTokens = range.Split('-');
                if (rangeTokens.Length == 2)
                {
                    long.TryParse(rangeTokens[1], out startByte);
                }
            }

            return startByte;
        }

        string CreatePostDataString(string boundary)
        {
            var sb = new StringBuilder();

            sb.Append(boundary + "\r\n");

            String fileDataTagName;
            switch (UploadClientSettings.FileBinaryDataBodyTag)
            {
                case FileBinaryDataBodyTag.File:
                    fileDataTagName = "file";
                    break;
                case FileBinaryDataBodyTag.FileData:
                default:
                    fileDataTagName = "fileData";
                    break;
            }

            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n", fileDataTagName, Path.GetFileName(_localFilePath));
            sb.Append("Content-Transfer-Encoding: binary\r\n");
            sb.Append("Content-Type: application/octet-stream\r\n\r\n");

            return sb.ToString();
        }

        HttpWebRequest CreateRequest(FileStream fileStream = null)
        {
            var fullUploadUrl = new StringBuilder(String.Format("{0}?filepath={1}", _uploadUrl, HttpUtility.UrlEncode(_virtualPath)));

            if (UploadClientSettings.UseQueryParams)
            {
                if (UploadClientSettings.AddCreationTimeToRequest)
                {
                    fullUploadUrl.AppendFormat("&creationTimeUtc={0}", _fileInfo.CreationTimeUtc.ToString("o"));
                }

                if (UploadClientSettings.AddLastWriteTimeToRequest)
                {
                    fullUploadUrl.AppendFormat("&lastWriteTimeUtc={0}", _fileInfo.LastWriteTimeUtc.ToString("o"));
                }

                String sha256 = GetSha256FromStream(fileStream);
                fullUploadUrl.AppendFormat("&sha256={0}", sha256);
                fullUploadUrl.AppendFormat("&sessionKey={0}", HttpUtility.UrlEncode(_sessionKey));
                fullUploadUrl.AppendFormat("&virtualFolderId={0}", _syncpointId);
                fullUploadUrl.AppendFormat("&fileDone=");
            }

            var request = (HttpWebRequest) WebRequest.Create(fullUploadUrl.ToString());

            request.AllowWriteStreamBuffering = false;
            request.KeepAlive = true;
            request.Timeout = int.MaxValue;
            request.SendChunked = true;
            request.Method = "POST";
            request.UserAgent = "Syncplicity Client";
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
            request.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            if (!String.IsNullOrEmpty(_eTag))
            {
                // if we don't have an etag, it most likely means that the server couldn't create one at the time of our query request.
                // without an If-Match header, the server will fall back to non chunked acceptance.
                request.Headers.Add("If-Match: " + _eTag);
            }

            request.Headers.Add(HttpRequestHeader.Authorization, _sessionKey);
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                request.Headers.Add("Syncplicity-Storage-Authorization", ConfigurationHelper.StorageToken);
            }

            if (_useGzip)
            {
                request.Headers.Add("Content-Encoding: gzip");
            }

            if (APIContext.OnBehalfOfUser.HasValue)
            {
                request.Headers.Add("As-User", APIContext.OnBehalfOfUser.Value.ToString("D"));
            }

            string boundary = "-------" + DateTime.Now.Ticks.ToString("x");
            // Send boundary with two less dashes as per RFC
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            // Add the two dashes to boundary per RFC for the actual boundary cases
            boundary = "--" + boundary;

            // encode header
            string postHeader = CreatePostDataString(boundary);
            _headerData = Encoding.ASCII.GetBytes(postHeader);
            _boundaryData = Encoding.ASCII.GetBytes("\r\n" + boundary + "\r\n");

            return request;
        }

        HttpWebRequest CreateQueryRequest()
        {
            HttpWebRequest request = CreateRequest();
            request.Headers.Add("Content-Range: */*");
            return request;
        }

        HttpWebRequest CreateChunkUploadRequest(FileStream fileStream)
        {
            HttpWebRequest request = CreateRequest(fileStream);
            request.Headers.Add(String.Format("Content-Range: {0}-*/*", _startByte));

            return request;
        }

        Stream CreateRequestStream(HttpWebRequest request)
        {
            return _useGzip ? new GZipStream(request.GetRequestStream(), CompressionMode.Compress) : request.GetRequestStream();
        }

        UploadClientAsyncInfo CreateAsyncInfo(HttpWebRequest request)
        {
            return new UploadClientAsyncInfo(request, _localFilePath);
        }

        void WriteFormValuePair(Stream requestStream, string formName, string formValue)
        {
            requestStream.Write(_boundaryData, 0, _boundaryData.Length);
            byte[] inputNameData = Encoding.ASCII.GetBytes("Content-Disposition: form-data; name=\"" + formName + "\"\r\n\r\n" + formValue);
            requestStream.Write(inputNameData, 0, inputNameData.Length);
        }

        void WriteSessionKey(Stream requestStream)
        {
            WriteFormValuePair(requestStream, "sessionKey", _sessionKey);
        }

        private void WriteSha256Data(Stream requestStream, string name, byte[] sha256Data)
        {
            requestStream.Write(_boundaryData, 0, _boundaryData.Length);

            byte[] inputNameData = Encoding.ASCII.GetBytes(String.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", name));
            requestStream.Write(inputNameData, 0, inputNameData.Length);

            if (sha256Data != null)
            {
                requestStream.Write(Encoding.ASCII.GetBytes(ConvertByteArrayToHashString(sha256Data)), 0, 64);
            }
        }

        internal static string ConvertByteArrayToHashString(byte[] data)
        {
            if (data == null)
                return null;

            var sb = new StringBuilder(64);

            foreach (byte b in data)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private void WriteVirtualFolderId(Stream requestStream, long virtualFolderId)
        {
            if (virtualFolderId <= 0)
                throw new ApplicationException("Virtual Folder Id is not valid.");

            WriteFormValuePair(requestStream, "virtualFolderId", virtualFolderId.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteFileDates(Stream requestStream)
        {
            if (UploadClientSettings.AddCreationTimeToRequest)
            {
                WriteFormValuePair(requestStream, "creationTimeUtc", _fileInfo.CreationTimeUtc.ToString("o"));
            }

            if (UploadClientSettings.AddLastWriteTimeToRequest)
            {
                WriteFormValuePair(requestStream, "lastWriteTimeUtc", _fileInfo.LastWriteTimeUtc.ToString("o"));
            }
        }

        private void Initialize()
        {
            var syncpoint = SyncPointsService.GetSyncpoint(_syncpointId);
            if (syncpoint == null)
                throw new ArgumentException("Invalid Syncpoint Id");

            var storageEndpoints = StorageEndpointsService.GetStorageEndpoints();
            var storageEndpoint = storageEndpoints.First(x => x.Id == syncpoint.StorageEndpointId);

            if (storageEndpoint.Urls == null || storageEndpoint.Urls.Length == 0)
                throw new ArgumentException("Invalid StorageEnpodin Urls");

            _uploadUrl = string.Format(DOWNLOAD_ULR_FORMAT, storageEndpoint.Urls[0].Url);
            _sessionKey = string.Format(SESSION_KEY_FORMAT,
                ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                    ? APIContext.MachineToken
                    : APIContext.AccessToken);

            Debug.WriteLine(string.Format("Upload Url: {0}", _uploadUrl));
            Debug.WriteLine(string.Format("Session Key: {0}", _sessionKey));

            Console.WriteLine(@"Upload Url: {0}", _uploadUrl);
            Console.WriteLine(@"Session Key: {0}", _sessionKey);
        }
    }

    public class UploadClientAsyncInfo
    {
        public UploadClientAsyncInfo(HttpWebRequest request, string filename)
        {
            Request = request;
            Filename = filename;
        }

        public readonly HttpWebRequest Request;

        public readonly string Filename;
    }
}