using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Services.Download
{
    public class DownloadClient
    {
        const int MAX_BUFFER_SIZE = 8 * 1024;
        const int RESPONSE_TIMEOUT_MILLISECONDS = 60 * 60 * 1000;
        const string DOWNLOAD_ULR_FORMAT = "{0}/retrieveFile.php";
        const string SESSION_KEY_FORMAT = "Bearer {0}";

        readonly byte[] _data = new byte[MAX_BUFFER_SIZE];

        private bool    _maxBytesPerSecondChanged;
        private string  _downloadUrl;
        private long     _syncpointId;
        private long     _latestVersionId;
        private string  _sessionKey;

        public bool DownloadFile(Entities.File file, string localFilepath)
        {
            string combinedFileName = Path.Combine(localFilepath, file.Filename);
            if (File.Exists(combinedFileName))
            {
                // Set it back to normal so we can append to it
                File.SetAttributes(combinedFileName, FileAttributes.Normal);
            }

            Initialize(file);

            using (var fileStream = new FileStream(combinedFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, MAX_BUFFER_SIZE, FileOptions.SequentialScan))
            {
                File.SetAttributes(combinedFileName, FileAttributes.Temporary | FileAttributes.NotContentIndexed);
                HttpWebRequest request = CreateWebRequest(_downloadUrl, _sessionKey, _syncpointId, _latestVersionId, fileStream.Length);

                fileStream.Seek(fileStream.Length, SeekOrigin.Begin);

                try
                {
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        DownloadFileFromResponse(response, fileStream);
                    }

                    return true;
                }
                catch (WebException e)
                {
                    var response = (HttpWebResponse) e.Response;
                    if (response != null)
                    {
                        Console.WriteLine("Exception caught during UploadCallback: Status Code: {0}, Status Description: {1} ", (int)response.StatusCode, response.StatusDescription);
                    }

                    fileStream.Close();
                    File.Delete(combinedFileName);

                    return false;
                }
            }
        }

        private void Initialize(Entities.File file)
        {
            _syncpointId = file.SyncpointId;
            _latestVersionId = file.LatestVersionId;

            var syncpoint = SyncPointsService.GetSyncpoint(_syncpointId);
            if (syncpoint == null)
                throw new ArgumentException("Invalid Syncpoint Id");

            var storageEndpoints = StorageEndpointsService.GetStorageEndpoints();
            var storageEndpoint = storageEndpoints.First(x => x.Id == syncpoint.StorageEndpointId);

            if (storageEndpoint.Urls == null || storageEndpoint.Urls.Length == 0)
                throw new ArgumentException("Invalid StorageEnpodin Urls");

            _downloadUrl = string.Format(DOWNLOAD_ULR_FORMAT, storageEndpoint.Urls[0].Url);
            _sessionKey = string.Format(SESSION_KEY_FORMAT,
                ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                    ? APIContext.MachineToken
                    : APIContext.AccessToken);

            Debug.WriteLine(string.Format("Download Url: {0}", _downloadUrl));
            Debug.WriteLine(string.Format("Session Key: {0}", _sessionKey));

            Console.WriteLine("Download Url: {0}", _downloadUrl);
            Console.WriteLine("Session Key: {0}", _sessionKey);
        }

        /// <summary>
        /// Get the response and start downloading the file
        /// </summary>
        private void DownloadFileFromResponse(HttpWebResponse response, FileStream fileStream)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.PartialContent)
                {
                    var reader = new StreamReader(responseStream);
                    string responseError = reader.ReadToEnd();

                    Debug.WriteLine(responseError);

                    return;
                }

                // Start reading the file
                using (Stream stream = responseStream)
                {
                    int bufferSize = MAX_BUFFER_SIZE;

                    var stopwatch = new Stopwatch();
                    do
                    {
                        if (_maxBytesPerSecondChanged)
                        {
                            _maxBytesPerSecondChanged = false;

                            bufferSize = MAX_BUFFER_SIZE;
                        }

                        stopwatch.Start();
                        int bytesRead = stream.Read(_data, 0, bufferSize);

                        if (bytesRead == 0)
                        {
                            break;
                        }

                        fileStream.Write(_data, 0, bytesRead);
                        stopwatch.Stop();

                        stopwatch.Reset();
                    } 
                    while (true);
                }
            }
        }

        HttpWebRequest CreateWebRequest(string downloadUrl, string sessionKey, long syncpointId, long latestVersionId, long startByte)
        {
            string requestUrl = String.Format
                (
                    "{0}?vToken={1}",
                    downloadUrl,
                    HttpUtility.UrlEncode(String.Format("{0}-{1}", syncpointId, latestVersionId))
                );

            var request = (HttpWebRequest) WebRequest.Create(requestUrl);

            request.Timeout = RESPONSE_TIMEOUT_MILLISECONDS;
            request.KeepAlive = true;
            request.Method = "GET";
            request.UserAgent = "Syncplicity Client";
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
            request.Headers.Add(HttpRequestHeader.Authorization, sessionKey);
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                request.Headers.Add("Syncplicity-Storage-Authorization", ConfigurationHelper.StorageToken);
            }
            if (APIContext.OnBehalfOfUser.HasValue)
            {
                request.Headers.Add("As-User", APIContext.OnBehalfOfUser.Value.ToString("D"));
            }
            request.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            if (startByte > 0)
            {
                MethodInfos.AddWithoutValidate.Invoke(request.Headers, new object[] { "Range", String.Format("bytes={0}-", startByte) });
            }

            return request;
        }

        internal static class MethodInfos
        {
            public static readonly MethodInfo AddWithoutValidate;

            static MethodInfos()
            {
                AddWithoutValidate = typeof(WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

    }
}