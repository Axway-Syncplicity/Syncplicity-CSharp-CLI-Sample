using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Services.Download
{
    public class DownloadClient
    {
        private const int MaxBufferSize = 8 * 1024;
        private const int ResponseTimeoutMilliseconds = 60 * 60 * 1000;
        private const string DownloadUlrFormat = "{0}/v2/files";
        private const string SessionKeyFormat = "Bearer {0}";

        private readonly byte[] _data = new byte[MaxBufferSize];

        private string  _downloadUrl;
        private long     _syncpointId;
        private long     _latestVersionId;
        private string  _sessionKey;

        public void DownloadFileSimple(Entities.File file, string downloadFolder)
        {
            var combinedFileName = Path.Combine(downloadFolder, file.Filename);
            PrepareDownloadFolder(downloadFolder, combinedFileName);

            Initialize(file);

            using (var fileStream = OpenFileForDownload(combinedFileName))
            {
                try
                {
                    DownloadChunk(fileStream);
                }
                catch
                {
                    fileStream.Close();
                    File.Delete(combinedFileName);

                    throw;
                }
            }
        }

        private static void PrepareDownloadFolder(string downloadFolder, string combinedFileName)
        {
            EnsureFolderExists(downloadFolder);

            if (File.Exists(combinedFileName))
            {
                // Delete existing file, otherwise sample does nothing.
                // It would try to resume download from the end, but since we have the whole file,
                // resuming is no-op and returns no content from server
                File.Delete(combinedFileName);
            }
        }

        private static FileStream OpenFileForDownload(string fileName)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                    MaxBufferSize, FileOptions.SequentialScan);

                File.SetAttributes(fileName, FileAttributes.Temporary | FileAttributes.NotContentIndexed);

                return fileStream;
            }
            catch
            {
                fileStream?.Dispose();

                throw;
            }
        }

        private void DownloadChunk(FileStream fileStream)
        {
            const int startByte = 0;
            fileStream.Seek(startByte, SeekOrigin.Begin);

            var request = CreateWebRequest(_downloadUrl, _sessionKey, _syncpointId, _latestVersionId, startByte);
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    DownloadFileFromResponse(response, fileStream);
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse) e.Response;
                if (response != null)
                {
                    Console.WriteLine(
                        $"Exception caught during DownloadChunk: Status Code: {(int) response.StatusCode}, Status Description: {response.StatusDescription}");
                }

                throw;
            }
        }

        private static void EnsureFolderExists(string downloadFolder)
        {
            if (Directory.Exists(downloadFolder)) return;

            Directory.CreateDirectory(downloadFolder);
        }

        private void Initialize(Entities.File file)
        {
            _syncpointId = file.SyncpointId;
            _latestVersionId = file.LatestVersionId;

            var syncpoint = SyncPointsService.GetSyncpoint(_syncpointId);
            if (syncpoint == null)
                throw new ArgumentException("Invalid Syncpoint Id");

            var storageEndpoint = StorageEndpointsService.GetStorageEndpoint(syncpoint.StorageEndpointId);

            if (storageEndpoint.Urls == null || storageEndpoint.Urls.Length == 0)
                throw new ArgumentException("Invalid StorageEndpoint Urls");

            _downloadUrl = string.Format(DownloadUlrFormat, storageEndpoint.Urls[0].Url);

            if (ConfigurationHelper.UseSecureSessionToken)
            {
                _sessionKey = string.Format(SessionKeyFormat, ApiGateway.CreateSst(storageEndpoint.Id));
            }
            else
            {
                _sessionKey = string.Format(SessionKeyFormat,
                    ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                        ? ApiContext.MachineToken
                        : ApiContext.AccessToken);
            }

            Debug.WriteLine($"Download Url: {_downloadUrl}");
            Debug.WriteLine($"Session Key: {_sessionKey}");

            Console.WriteLine($"Download Url: {_downloadUrl}");
            Console.WriteLine($"Session Key: {_sessionKey}");
        }

        /// <summary>
        /// Get the response and start downloading the file
        /// </summary>
        private void DownloadFileFromResponse(HttpWebResponse response, FileStream fileStream)
        {
            using (var responseStream = response.GetResponseStream())
            {
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.PartialContent)
                {
                    var reader = new StreamReader(responseStream);
                    var responseError = reader.ReadToEnd();

                    Debug.WriteLine(responseError);

                    return;
                }

                // Start reading the file
                do
                {
                    var bytesRead = responseStream.Read(_data, 0, MaxBufferSize);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    fileStream.Write(_data, 0, bytesRead);
                } 
                while (true);
            }
        }

        private HttpWebRequest CreateWebRequest(string downloadUrl, string sessionKey, long syncpointId, long latestVersionId, long startByte)
        {
            var requestUrl =
                $"{downloadUrl}?syncpoint_id={syncpointId}&file_version_id={latestVersionId}";

            var request = (HttpWebRequest) WebRequest.Create(requestUrl);

            request.Timeout = ResponseTimeoutMilliseconds;
            request.KeepAlive = true;
            request.Method = "GET";
            request.UserAgent = "Syncplicity Client";
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
            request.Headers.Add(HttpRequestHeader.Authorization, sessionKey);

            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                request.Headers.Add("Syncplicity-Storage-Authorization", ConfigurationHelper.StorageToken);
            }

            if (ApiContext.OnBehalfOfUser.HasValue)
            {
                request.Headers.Add("As-User", ApiContext.OnBehalfOfUser.Value.ToString("D"));
            }

            if (startByte > 0)
            {
                request.AddRange(startByte);
            }

            return request;
        }
    }
}