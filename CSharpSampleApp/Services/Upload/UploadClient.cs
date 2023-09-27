using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CSharpSampleApp.Services.Upload
{
    public class UploadClient
    {
        private const string UploadUlrFormat = "{0}/v2/mime/files";
        private const string SessionKeyFormat = "Bearer {0}";
        private const int MaxBufferSize = 8 * 1024;
        private const HttpStatusCode ResumeIncompleteStatusCode = (HttpStatusCode)308;
        private static readonly byte[] Data = new byte[MaxBufferSize];

        private readonly string _virtualPath;
        private readonly string _localFilePath;
        private readonly long _syncpointId;

        private readonly FileInfo _fileInfo;

        private readonly AsyncCallback _callback;

        private string _sessionKey;
        private string _ssltToken;
        private string _uploadUrl;
        private long _chunkSize;
        private string _eTag;
        private long _startByte;
        private byte[] _headerData;
        private byte[] _boundaryData;
        private string _multipartBoundaryParameter;

        private bool ShouldChunk { get; set; }

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
                WriteMultipartBodyTerminator(queryRequestStream);
            }

            GetResponseAndUpdateChunkUploadStateFields(queryRequest);
        }

        public void UploadFileWithoutChunking(Link link = null)
        {
            if (!_fileInfo.Exists)
            {
                throw new ApplicationException("File doesn't exist.");
            }

            Initialize(link);

            ShouldChunk = false;

            using (var fileStream = new FileStream(_localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
                MaxBufferSize, FileOptions.SequentialScan))
            {
                bool shouldContinue;
                do
                {
                    // The server sends us the startByte back after every chunk request.
                    // We'll reset our stream position here, inside the while loop -
                    // to conceivably support future situations
                    // such as the server informing the client to restart an upload, upload chunks out of order, etc.
                    //
                    // For sequential chunk uploading this will be a no-op.
                    fileStream.Position = Math.Min(fileStream.Length, _startByte);

                    shouldContinue = UploadChunk(fileStream, link);
                } while (shouldContinue);
            }
        }

        public void UploadFileUsingChunks()
        {
            if (!_fileInfo.Exists)
            {
                Console.WriteLine("File not exists.");
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
                    // The server sends us the startByte back after every chunk request.
                    // We'll reset our stream position here, inside the while loop -
                    // to conceivably support future situations
                    // such as the server informing the client to restart an upload, upload chunks out of order, etc.
                    //
                    // For sequential chunk uploading this will be a no-op.
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

                    // If this fails FileUploader will catch the exception and mark the file as unable to upload
                    _chunkSize = Convert.ToInt64(httpWebResponse.Headers["Syncplicity-Chunk-Size"]);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns true if there are more chunks remaining, false if we've reached the end of the file
        /// </summary>
        private bool UploadChunk(Stream fileStream, Link link = null)
        {
            string sha256;
            bool finalChunk;
            long bytesReadTotal = 0;
            var chunkRequest = CreateChunkUploadRequest(link);

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
                            // Clamp buffer size down if this last Read() of the chunk
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

                WriteMultipartBodyTerminator(chunkRequestStream);
            }

            if (finalChunk)
            {
                // We've reached the end of the file, time to call our callback
                Console.WriteLine($"Uploading file {_localFilePath} with hash {sha256} to {chunkRequest.Address}");
                Console.WriteLine("ChunkRequest Headers:");
                Console.WriteLine(chunkRequest.Headers);

                Debug.WriteLine($"Uploading file {_localFilePath} with hash {sha256} to {chunkRequest.Address}");
                Debug.WriteLine("ChunkRequest Headers:");
                Debug.WriteLine(chunkRequest.Headers);

                chunkRequest.BeginGetResponse(_callback, CreateAsyncInfo(chunkRequest));
            }
            else
            {
                GetResponseAndUpdateChunkUploadStateFields(chunkRequest);
            }

            return !finalChunk;
        }

        private void WriteMultipartBodyTerminator(Stream requestStream)
        {
            var terminator = $"\r\n--{_multipartBoundaryParameter}--\r\n";
            var terminatorData = Encoding.ASCII.GetBytes(terminator);
            requestStream.Write(terminatorData, 0, terminatorData.Length);
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

        private string CreatePostDataString(string partBoundaryLine, Link link = null)
        {
            var sb = new StringBuilder();

            sb.Append(partBoundaryLine);

            sb.Append($"Content-Disposition: form-data; name=\"fileData\"; filename=\"{Path.GetFileName(_localFilePath)}\"\r\n");
            sb.Append("Content-Transfer-Encoding: binary\r\n");
            sb.Append("Content-Type: application/octet-stream\r\n\r\n");
            if (link != null) //Upload will be performed on shared link
            {
                sb.Append("api test\r\n");
                sb.Append($"{partBoundaryLine}");
                sb.Append("Content-Disposition: form-data; name=\"sha256\"\r\n");
                sb.AppendLine();
                sb.Append("e498668995cd5cff2660d797130f8b8bb2252a44d8f88c9cb4f9daf761538072\r\n");
                sb.Append($"{ partBoundaryLine}");
                sb.Append("Content-Disposition: form-data; name=\"Filename\"\r\n");
                sb.AppendLine();
                sb.Append($"{Path.GetFileName(_localFilePath)}\"\r\n");
                sb.Append($"{ partBoundaryLine}");
                sb.Append("Content-Disposition: form-data; name=\"sessionKey\"\r\n");
                sb.AppendLine();
                sb.Append($"{_ssltToken}\r\n");
                sb.Append($"{partBoundaryLine}");
                sb.Append("Content-Disposition: form-data; name=\"dirId\"\r\n");
                sb.AppendLine();
                sb.Append($"{link.SyncPointId}-{link.Folder.FolderId}\r\n");
                sb.Append($"{partBoundaryLine}");
                sb.Append("Content-Disposition: form-data; name=\"fileDone\"\r\n");
                sb.AppendLine();
                sb.AppendLine();
                sb.Append($"{partBoundaryLine}\r\n");
            }

            return sb.ToString();
        }

        private HttpWebRequest CreateRequest(Link link = null)
        {
            var fullUploadUrl = $"{_uploadUrl}?filepath={HttpUtility.UrlEncode(_virtualPath)}";

            var request = (HttpWebRequest)WebRequest.Create(fullUploadUrl);

            request.KeepAlive = true;
            request.Timeout = int.MaxValue;
            request.Method = "POST";
            request.UserAgent = "Syncplicity C# CLI Sample Application";
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);

            if (!string.IsNullOrEmpty(_eTag))
            {
                // If we don't have an etag, it most likely means that the server couldn't create one at the time of our query request.
                // Without an If-Match header, the server will fall back to non-chunked acceptance.
                request.Headers.Add(HttpRequestHeader.IfMatch, _eTag);
            }

            request.Headers.Add(HttpRequestHeader.Authorization, link != null ? _ssltToken : _sessionKey);
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                request.Headers.Add("Syncplicity-Storage-Authorization", ConfigurationHelper.StorageToken);
            }

            if (ApiContext.OnBehalfOfUser.HasValue)
            {
                request.Headers.Add("As-User", ApiContext.OnBehalfOfUser.Value.ToString("D"));
            }

            _multipartBoundaryParameter = "-------" + DateTime.Now.Ticks.ToString("x");
            // Send boundary with two less dashes as per RFC
            request.ContentType = "multipart/form-data; boundary=" + _multipartBoundaryParameter;
            // Add the two dashes to boundary per RFC for the actual boundary cases
            var partBoundaryLine = "--" + _multipartBoundaryParameter + "\r\n";

            // Encode header
            var postHeader = CreatePostDataString(partBoundaryLine, link);
            _headerData = Encoding.ASCII.GetBytes(postHeader);
            _boundaryData = Encoding.ASCII.GetBytes("\r\n" + partBoundaryLine);

            return request;
        }

        private HttpWebRequest CreateQueryRequest()
        {
            var request = CreateRequest();
            request.Headers.Add(HttpRequestHeader.ContentRange, "*/*");
            return request;
        }

        private HttpWebRequest CreateChunkUploadRequest(Link link = null)
        {
            var request = CreateRequest(link);
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

        private void Initialize(Link link = null)
        {
            var syncpoint = SyncPointsService.GetSyncpoint(_syncpointId);
            if (syncpoint == null)
                throw new ArgumentException("Invalid Syncpoint Id");

            var storageEndpoints = StorageEndpointsService.GetStorageEndpoints();
            var storageEndpoint = storageEndpoints.First(x => x.Id == syncpoint.StorageEndpointId);

            if (storageEndpoint.Urls == null || storageEndpoint.Urls.Length == 0)
                throw new ArgumentException("Invalid StorageEndpoint Urls");

            _uploadUrl = string.Format(UploadUlrFormat, storageEndpoint.Urls[0].Url);

            if (ConfigurationHelper.UseSecureSessionToken)
            {
                _sessionKey = string.Format(SessionKeyFormat, ApiGateway.CreateSst(storageEndpoint.Id));
            }
            if (link != null && ConfigurationHelper.UseSecureSessionLinkToken)
            {
                _ssltToken = string.Format(SessionKeyFormat, ApiGateway.CreateSslt(link.Token));
            }
            else
            {
                _sessionKey = string.Format(SessionKeyFormat,
                    ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                        ? ApiContext.MachineToken
                        : ApiContext.AccessToken);
            }

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