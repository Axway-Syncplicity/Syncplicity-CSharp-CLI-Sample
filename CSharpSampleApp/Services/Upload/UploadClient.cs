using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        private const string DownloadUlrFormat = "{0}/saveFile.php";
        private const string SessionKeyFormat = "Bearer {0}";
        private const int MaxBufferSize = 8 * 1024;
        private const HttpStatusCode ResumeIncompleteStatusCode = (HttpStatusCode)308;
        private static readonly byte[] Data = new byte[MaxBufferSize];

        private readonly string _virtualPath;
        private readonly string _localFilePath;
        private readonly long    _syncpointId;

        private readonly FileInfo _fileInfo;

        private readonly AsyncCallback _callback;

        private string  _sessionKey;
        private string  _uploadUrl;
        private long    _chunkSize;
        private string  _eTag;
        private long    _startByte;
        private byte[]  _headerData;
        private byte[]  _boundaryData;

        private bool ShouldChunk {get; set; }

        public UploadClient(Folder folder, string localFilePath, AsyncCallback finalCallback)
        {
            _syncpointId = folder.SyncpointId;
            _virtualPath = folder.VirtualPath + Path.GetFileName(localFilePath);
            _localFilePath = localFilePath;

            _callback = finalCallback;

            _fileInfo = new FileInfo(localFilePath);
        }

        private void QueryFileChunkedUploadStatus()
        {
            // Query the server to determine chunk upload status
            var queryRequest = CreateQueryRequest();
            using (var queryRequestStream = CreateRequestStream(queryRequest))
            {
                WriteSessionKey(queryRequestStream);
                queryRequestStream.Write(_boundaryData, 0, _boundaryData.Length);
            }

            GetResponseAndUpdateChunkUploadStateFields(queryRequest);
        }

        public void UploadFileWithoutChunking()
        {
            if (!_fileInfo.Exists)
            {
                throw new ApplicationException("File doesn't exist.");
            }

            Initialize();

            ShouldChunk = false;

            using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                MaxBufferSize, FileOptions.SequentialScan))
            {
                bool shouldContinue;
                do
                {
                    // the server sends us the startByte back after every chunk request. we'll reset
                    // our stream position here, inside the while loop to conceivably support future
                    // situations such as the server informing the client to restart an upload,
                    // upload chunks out of order, etc.
                    //
                    // for sequential chunk uploading this will be a nop.
                    fileStream.Position = Math.Min(fileStream.Length, _startByte);

                    shouldContinue = UploadChunk(fileStream);
                } while (shouldContinue);
            }
        }

        public void UploadFileUsingChunks()
        {
            if (!_fileInfo.Exists)
            {
                Console.WriteLine(@"File not exists.");
                return;
            }

            Initialize();

            ShouldChunk = true;

            QueryFileChunkedUploadStatus();

            using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                MaxBufferSize, FileOptions.SequentialScan))
            {
                bool shouldContinue;
                do
                {
                    // the server sends us the startByte back after every chunk request. we'll reset
                    // our stream position here, inside the while loop to conceivably support future
                    // situations such as the server informing the client to restart an upload,
                    // upload chunks out of order, etc.
                    //
                    // for sequential chunk uploading this will be a nop.
                    fileStream.Position = Math.Min(fileStream.Length, _startByte);

                    shouldContinue = UploadChunk(fileStream);

                } while (shouldContinue);
            }
        }

        private void GetResponseAndUpdateChunkUploadStateFields(WebRequest request)
        {
            try
            {
                request.GetResponse().Close();
            }
            catch (WebException e)
            {
                var httpWebResponse = e.Response as HttpWebResponse;
                if (httpWebResponse != null &&
                    httpWebResponse.StatusCode == ResumeIncompleteStatusCode)
                {
                    _eTag = httpWebResponse.Headers[HttpResponseHeader.ETag];
                    _startByte = GetStartByte(httpWebResponse);

                    // if this fails FileUploader will catch the exception and mark the file as unable to upload
                    _chunkSize = Convert.ToInt64(httpWebResponse.Headers["Syncplicity-Chunk-Size"]);
                }
                else
                {
                    throw;
                }
            }
        }

        // returns true if there are more chunks remaining, false if we've reached the end of the file
        private bool UploadChunk(Stream fileStream)
        {
            string sha256;
            bool finalChunk;
            long bytesReadTotal = 0;
            var chunkRequest = CreateChunkUploadRequest();

            using (var chunkRequestStream = CreateRequestStream(chunkRequest))
            {
                // Write the header data always at full speed
                chunkRequestStream.Write(_headerData, 0, _headerData.Length);

                var bufferSize = MaxBufferSize;

                using (var chunkSha265 = new SHA256Managed())
                {
                    do
                    {
                        if (_chunkSize > 0)
                        {
                            // clamp buffer size down if this last Read() of the chunk
                            bufferSize = Math.Min(bufferSize, (int)(_chunkSize - bytesReadTotal));
                        }

                        var bytesRead = fileStream.Read(Data, 0, bufferSize);
                        if (bytesRead == 0)
                        {
                            chunkSha265.TransformFinalBlock(Data, 0, bytesRead);
                            break;
                        }

                        bytesReadTotal += bytesRead;

                        // Hash data
                        chunkSha265.TransformBlock(Data, 0, bytesRead, null, 0);

                        // Send chunk data
                        chunkRequestStream.Write(Data, 0, bytesRead);
                    } 
                    while (true);

                    sha256 = bytesReadTotal > 0 ? ConvertByteArrayToHashString(chunkSha265.Hash) : "";

                    WriteSha256Data(chunkRequestStream, "sha256", bytesReadTotal > 0 ? chunkSha265.Hash : null);
                    WriteSessionKey(chunkRequestStream);
                    WriteVirtualFolderId(chunkRequestStream, _syncpointId);
                    WriteFileDates(chunkRequestStream);

                    finalChunk = fileStream.Position == fileStream.Length;

                    if (finalChunk)
                    {
                        WriteFormValuePair(chunkRequestStream, "fileDone", string.Empty);
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

                chunkRequest.BeginGetResponse(_callback, CreateAsyncInfo(chunkRequest));
            }
            else
            {
                GetResponseAndUpdateChunkUploadStateFields(chunkRequest);
            }

            return !finalChunk;
        }

        private static long GetStartByte(WebResponse response)
        {
            long startByte = 0;

            var range = response.Headers["Range"];
            if (string.IsNullOrEmpty(range)) return startByte;

            var rangeTokens = range.Split('-');
            if (rangeTokens.Length == 2)
            {
                long.TryParse(rangeTokens[1], out startByte);
            }

            return startByte;
        }

        private string CreatePostDataString(string boundary)
        {
            var sb = new StringBuilder();

            sb.Append(boundary + "\r\n");

            sb.Append($"Content-Disposition: form-data; name=\"fileData\"; filename=\"{Path.GetFileName(_localFilePath)}\"\r\n");
            sb.Append("Content-Transfer-Encoding: binary\r\n");
            sb.Append("Content-Type: application/octet-stream\r\n\r\n");

            return sb.ToString();
        }

        private HttpWebRequest CreateRequest()
        {
            var fullUploadUrl = new StringBuilder($"{_uploadUrl}?filepath={HttpUtility.UrlEncode(_virtualPath)}");

            var request = (HttpWebRequest) WebRequest.Create(fullUploadUrl.ToString());

            request.KeepAlive = true;
            request.Timeout = int.MaxValue;
            request.Method = "POST";
            request.UserAgent = "Syncplicity Client";
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
            request.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                /*
                 * SECURITY WARNING!
                 * DO NOT reproduce this in production code.
                 * This means disabling server SSL certificate validation errors,
                 * which in turn makes the code vulnerable to man-in-the-middle attacks.
                 */
                return true;
            };

            if (!string.IsNullOrEmpty(_eTag))
            {
                // if we don't have an etag, it most likely means that the server couldn't create one at the time of our query request.
                // without an If-Match header, the server will fall back to non chunked acceptance.
                request.Headers.Add(HttpRequestHeader.IfMatch, _eTag);
            }

            request.Headers.Add(HttpRequestHeader.Authorization, _sessionKey);
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                request.Headers.Add("Syncplicity-Storage-Authorization", ConfigurationHelper.StorageToken);
            }

            if (ApiContext.OnBehalfOfUser.HasValue)
            {
                request.Headers.Add("As-User", ApiContext.OnBehalfOfUser.Value.ToString("D"));
            }

            var boundary = "-------" + DateTime.Now.Ticks.ToString("x");
            // Send boundary with two less dashes as per RFC
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            // Add the two dashes to boundary per RFC for the actual boundary cases
            boundary = "--" + boundary;

            // encode header
            var postHeader = CreatePostDataString(boundary);
            _headerData = Encoding.ASCII.GetBytes(postHeader);
            _boundaryData = Encoding.ASCII.GetBytes("\r\n" + boundary + "\r\n");

            return request;
        }

        private HttpWebRequest CreateQueryRequest()
        {
            var request = CreateRequest();
            request.Headers.Add(HttpRequestHeader.ContentRange, "*/*");
            return request;
        }

        private HttpWebRequest CreateChunkUploadRequest()
        {
            var request = CreateRequest();
            request.Headers.Add(HttpRequestHeader.ContentRange, $"{_startByte}-*/*");

            return request;
        }

        private static Stream CreateRequestStream(WebRequest request)
        {
            return request.GetRequestStream();
        }

        private UploadClientAsyncInfo CreateAsyncInfo(HttpWebRequest request)
        {
            return new UploadClientAsyncInfo(request, _localFilePath);
        }

        private void WriteFormValuePair(Stream requestStream, string formName, string formValue)
        {
            requestStream.Write(_boundaryData, 0, _boundaryData.Length);
            var inputNameData = Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{formName}\"\r\n\r\n{formValue}");
            requestStream.Write(inputNameData, 0, inputNameData.Length);
        }

        private void WriteSessionKey(Stream requestStream)
        {
            WriteFormValuePair(requestStream, "sessionKey", _sessionKey);
        }

        private void WriteSha256Data(Stream requestStream, string name, byte[] sha256Data)
        {
            requestStream.Write(_boundaryData, 0, _boundaryData.Length);

            var inputNameData = Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{name}\"\r\n\r\n");
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

            foreach (var b in data)
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
            WriteFormValuePair(requestStream, "creationTimeUtc", _fileInfo.CreationTimeUtc.ToString("o"));
            WriteFormValuePair(requestStream, "lastWriteTimeUtc", _fileInfo.LastWriteTimeUtc.ToString("o"));
        }

        private void Initialize()
        {
            var syncpoint = SyncPointsService.GetSyncpoint(_syncpointId);
            if (syncpoint == null)
                throw new ArgumentException("Invalid Syncpoint Id");

            var storageEndpoints = StorageEndpointsService.GetStorageEndpoints();
            var storageEndpoint = storageEndpoints.First(x => x.Id == syncpoint.StorageEndpointId);

            if (storageEndpoint.Urls == null || storageEndpoint.Urls.Length == 0)
                throw new ArgumentException("Invalid StorageEndpoint Urls");

            _uploadUrl = string.Format(DownloadUlrFormat, storageEndpoint.Urls[0].Url);
            _sessionKey = string.Format(SessionKeyFormat,
                ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                    ? ApiContext.MachineToken
                    : ApiContext.AccessToken);

            Debug.WriteLine($"Upload Url: {_uploadUrl}");
            Debug.WriteLine($"Session Key: {_sessionKey}");

            Console.WriteLine($"Upload Url: {_uploadUrl}");
            Console.WriteLine($"Session Key: {_sessionKey}");
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