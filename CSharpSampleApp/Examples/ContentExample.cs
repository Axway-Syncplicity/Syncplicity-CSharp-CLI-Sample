using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using CSharpSampleApp.Entities;
using CSharpSampleApp.Services;
using CSharpSampleApp.Services.Download;
using CSharpSampleApp.Services.Upload;
using CSharpSampleApp.Util;
using File = CSharpSampleApp.Entities.File;

namespace CSharpSampleApp.Examples
{
    public class ContentExample
    {
        private static SyncPoint _currentSyncpoint;
        private static Folder _currentFolder;

        private static Exception _lastException;
        private static readonly AutoResetEvent UploadFinished = new AutoResetEvent(false);
        private static User _oldOwner;

        /*
         * Content - Simple
         * - Creating a Syncpoint to allow uploads/downloads to folders
         * - Creating a folder
         * - Uploading a file into the folder
         * - Downloading the uploaded file        
         * - Removing the uploaded file
         * - Removing the folder
         * - Changing owner of the syncpoint
         */
        public static void ExecuteSimple()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            var localFilePath = ConfigurationHelper.UploadFileSmall;

            CreateSyncpoint();
            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);
            DownloadFile(localFilePath);
            RemoveFile(localFilePath);
            RemoveFolder();
            ChangeOwnerOfSyncpoint(ConfigurationHelper.NewSyncpointOwnerEmail);
        }

        /*
         * Content - Chunked upload
         * - Creating a Syncpoint to allow uploads/downloads to folders
         * - Creating a folder
         * - Uploading a file into the folder using chunked upload method
         * - Downloading the uploaded file        
         * - Removing the uploaded file
         * - Removing the folder
         */
        public static void ExecuteChunked()
        {
            if (!ValidateConfigurationForChunkedSample()) return;

            var localFilePath = ConfigurationHelper.UploadFileLarge;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            UploadFile(localFilePath, UploadMode.Chunked);
            DownloadFile(localFilePath);
            RemoveFile(localFilePath);
            RemoveFolder();
        }

        /*
         * Content - simple upload on behalf of another user
         * Note: content actions in this example are performed on behalf of the user specified in asUserEmail config setting.
         *
         * - Creating a Syncpoint to allow uploads/downloads to folders
         * - Creating a folder
         * - Uploading a file into the folder using chunked upload method
         * - Downloading the uploaded file        
         * - Removing the uploaded file
         * - Removing the folder
         */
        public static void ExecuteOnBehalfOf()
        {
            var localFilePath = ConfigurationHelper.UploadFileSmall;

            if (!OnBehalfOfPrepare()) return;

            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);
            DownloadFile(localFilePath);
            RemoveFile(localFilePath);
            RemoveFolder();
        }

        private static bool ValidateConfigurationForOrdinarySample()
        {
            var errors = EvaluateConfigValidationRulesForOrdinarySample().ToList();
            if (!errors.Any()) return true;

            Console.WriteLine("Cannot proceed to Content.ExecuteSimple example because of configuration errors");
            errors.ForEach(e => Console.WriteLine($"Error: {e}"));

            return false;
        }

        private static IEnumerable<string> EvaluateConfigValidationRulesForOrdinarySample()
        {
            var localFilePath = ConfigurationHelper.UploadFileSmall;

            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                yield return "uploadFileSmall is not specified";
            }
            else
            {
                if (!System.IO.File.Exists(localFilePath))
                {
                    yield return
                        $"Cannot find file '{localFilePath}'. Please check that the file exists and the process has access to it";
                }
            }

            if (string.IsNullOrWhiteSpace(ConfigurationHelper.DownloadFolder)) yield return "downloadDirectory is not specified";
        }

        private static bool ValidateConfigurationForChunkedSample()
        {
            var errors = EvaluateConfigValidationRulesForChunkedSample().ToList();
            if (!errors.Any()) return true;

            Console.WriteLine("Cannot proceed to Content.ExecuteChunked example because of configuration errors");
            errors.ForEach(e => Console.WriteLine($"Error: {e}"));

            return false;
        }

        private static IEnumerable<string> EvaluateConfigValidationRulesForChunkedSample()
        {
            var localFilePath = ConfigurationHelper.UploadFileLarge;

            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                yield return "uploadFileLarge is not specified";
            }
            else
            {
                if (!System.IO.File.Exists(localFilePath))
                {
                    yield return
                        $"Cannot find file '{localFilePath}'. Please check that the file exists and the process has access to it";
                }
            }

            if (string.IsNullOrWhiteSpace(ConfigurationHelper.DownloadFolder)) yield return "downloadDirectory is not specified";
        }

        private static void RemoveFolder()
        {
            SyncService.RemoveFolder(_currentFolder.SyncpointId, _currentFolder.FolderId, false);
        }

        private static void RemoveFile(string localFilePath)
        {
            var fileToRemove = GetCurrentFile(localFilePath);
            SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, false);
        }

        private static void DownloadFile(string localFilePath)
        {
            var fileToDownload = GetCurrentFile(localFilePath);

            Console.WriteLine();
            Console.WriteLine("Start File Downloading...");

            var downloadClient = new DownloadClient();

            Console.WriteLine();

            try
            {
                downloadClient.DownloadFileSimple(fileToDownload, ConfigurationHelper.DownloadFolder);
                Console.WriteLine("Download of {0} to {1} succeeded.", fileToDownload.Filename, ConfigurationHelper.DownloadFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("Download of {0} to {1} failed.", fileToDownload.Filename, ConfigurationHelper.DownloadFolder);
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);

                throw;
            }

        }

        private static File GetCurrentFile(string localFilePath)
        {
            //refresh folder info
            _currentFolder = SyncService.GetFolder(_currentSyncpoint.Id, _currentFolder.FolderId);
            var currentFile =
                _currentFolder.Files.First(x => x.Filename == Path.GetFileName(localFilePath));
            return currentFile;
        }

        private static bool OnBehalfOfPrepare()
        {
            if (string.IsNullOrEmpty(ConfigurationHelper.AsUserEmail))
            {
                Console.WriteLine();
                Console.WriteLine("asUserEmail is not set. Skipping OnBehalfOf Requests");
                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Start OnBehalfOf Requests...");

            var user = UsersService.GetUser(ConfigurationHelper.AsUserEmail);
            CreateSyncpointInternal(user, true);

            return ApiContext.HasStorageEndpoint;
        }

        private static void CreateSyncpoint()
        {
            Console.WriteLine();
            Console.WriteLine("Start Common Requests...");

            var user = UsersService.GetUser(ConfigurationHelper.OwnerEmail);
            CreateSyncpointInternal(user);
        }

        private static void CreateSyncpointInternal(User user, bool isObo = false)
        {
            Console.WriteLine();
            Console.WriteLine("Start SyncPoint Creation...");

            //get storage endpoint of current user need admin rights
            var storageEndpoint = StorageEndpointsService.GetStorageEndpointByUser(user.Id);

            if (storageEndpoint == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Unable to determine the user's storage endpoint.  Content APIs will not be able to proceed.");
                return;
            }
            ApiContext.HasStorageEndpoint = true;
            if (isObo)
                ApiContext.OnBehalfOfUser = user.Id;

            var random = new Random();
            var newSyncPoint = new SyncPoint
            {
                Name = ConfigurationHelper.SyncpointName + random.Next(),
                Type = SyncPointType.Custom,
                Path = "C:\\Syncplicity",
                StorageEndpointId = storageEndpoint.Id,
                Mapped = true,
                DownloadEnabled = true,
                UploadEnabled = true
            };

            var createdSyncPoints = SyncPointsService.CreateSyncpoints(new[] { newSyncPoint });

            Console.WriteLine();

            if (createdSyncPoints == null || createdSyncPoints.Length == 0)
            {
                Console.WriteLine("Error occurred during creating SyncPoint.");
                return;
            }

            //Need to call getSyncPoint to hydrate all the meta data of the syncpoint, in particular
            //we need RootFolderId so that we can create a folder next.
            _currentSyncpoint = SyncPointsService.GetSyncpoint(createdSyncPoints[0].Id);

            //map syncpoint to device
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                Console.WriteLine("Mapping the syncpoint {0} to machine {1}", _currentSyncpoint.Id,
                    ConfigurationHelper.MachineId);
                _currentSyncpoint.Mappings = new[]
                {
                    new Mapping
                    {
                        SyncPointId = _currentSyncpoint.Id,
                        Mapped = true,
                        Machine = new Machine {Id = ConfigurationHelper.MachineId}
                    }
                };
                _currentSyncpoint = SyncPointsService.PutSyncpoint(_currentSyncpoint);
            }

            Console.WriteLine("Finished SyncPoint Creation. Created new SyncPoint {0} with Id: {1}", _currentSyncpoint.Name, _currentSyncpoint.Id);
        }

        public static void CreateFolder()
        {
            if (_currentSyncpoint == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Start Folder Creation...");

            var syncpointId = _currentSyncpoint.Id;            // internal integer id of syncpoint

            var random = new Random();
            var newFolder = new Folder
            {
                VirtualPath = $@"\{ConfigurationHelper.FolderName}{random.Next()}\",
                Status = FolderStatus.Added,
                SyncpointId = syncpointId
            };

            var createdFolders = SyncService.CreateFolders(syncpointId, new [] { newFolder });

            Console.WriteLine();

            if (createdFolders == null || createdFolders.Length == 0)
            {
                Console.WriteLine("Error occurred during creating new folder. Content APIs will not be able to continue.");
                return;
            }
            
            _currentFolder = createdFolders[0];

            Console.WriteLine(
                $"Finished Folder Creation. Created new Folder {createdFolders[0].Name} with Id: {createdFolders[0].FolderId}");
        }

        private static void UploadFile(string localFilePath, UploadMode mode)
        {
            if (_currentFolder == null)
            {
                return;
            }

            Console.WriteLine();

            if (!System.IO.File.Exists(localFilePath))
            {
                Console.WriteLine("Unable to find local file {0}.  Content APIs will not be able to continue.", localFilePath);
                throw new ConfigurationErrorsException("Cannot find the file defined as the value of \"uploadFile\" configuration");
            }

            Console.WriteLine("Start File Uploading...");

            try
            {
                var uploadClient = new UploadClient(_currentFolder, localFilePath, UploadCallBack);
                switch (mode)
                {
                    case UploadMode.Chunked:
                        uploadClient.UploadFileUsingChunks();
                        break;
                    case UploadMode.Simple:
                    default:
                        uploadClient.UploadFileWithoutChunking();
                        break;
                }

                UploadFinished.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Uploading file failed.");
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);
                UploadFinished.Set();
                throw;
            }

            Console.WriteLine();

            if (_lastException != null)
            {
                Console.WriteLine("Uploading file failed.");
                throw new InvalidOperationException("Uploading file failed", _lastException);
            }

            Console.WriteLine("File {0} uploaded successfully to folder {1}.", localFilePath, _currentFolder.Name);
        }
        private static void UploadCallBack(IAsyncResult result)
        {
            try
            {
                var info = (UploadClientAsyncInfo)result.AsyncState;
                info.Request.EndGetResponse(result).Close();
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                if (response != null)
                    Console.WriteLine(@"Exception caught during UploadCallback: Status Code: {0}, Status Description: {1} ", (int)response.StatusCode, response.StatusDescription);
                _lastException = e;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Exception caught during UploadCallback: {0} ", e);
                _lastException = e;
            }
            finally
            {
                //Must set this, otherwise, hangs when uploadFinished.WaitOne() is called. 
                UploadFinished.Set();
            }
        }

        private static void ChangeOwnerOfSyncpoint(string newOwnerEmail)
        {
            if (string.IsNullOrEmpty(newOwnerEmail))
            {
                Console.WriteLine();
                Console.WriteLine("New owner is not defined - skipping change syncpoint's owner");

                return;
            }
            Console.WriteLine();
            Console.WriteLine("Start changing the owner of syncpoint...");

            _oldOwner = _currentSyncpoint.Owner;
            _currentSyncpoint.Owner = new User {EmailAddress = newOwnerEmail };
            _currentSyncpoint = SyncPointsService.PutSyncpoint(_currentSyncpoint);
            ParticipantsService.RemoveParticipants(_currentSyncpoint.Id, _oldOwner.EmailAddress);

            Console.WriteLine();
            Console.WriteLine("Owner of syncpoint has been changed...");
        }

        private enum UploadMode
        {
            Simple,
            Chunked
        }
    }
}
