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
        private static readonly Random _random = new Random(); 

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

            ApiContext.OnBehalfOfUser = null;
        }

        /*
         * Content + Provisioning - put data custodian user on legal hold, and read deleted data on her behalf.
         * Note: actions in this example require setting values from Legal Holds configuration section.
         *
         * - as eDiscovery administrator looking up for the data custodian user and putting him on legal hold
         * - as custodian user creating a content and then deleting it
         * - as eDiscovery administrator reading the content on behalf of the user, but also reading deleted content
         */
        public static void ExecuteLegalHold()
        {
            if (string.IsNullOrEmpty(ConfigurationHelper.DataCustodianUserEmail)
                || string.IsNullOrEmpty(ConfigurationHelper.eDiscoveryAdminToken)
                || string.IsNullOrEmpty(ConfigurationHelper.DataCustodianUserToken)
                )
            {
                Console.WriteLine();
                Console.WriteLine("Required settings are not set. Skipping Legal Hold requests");
                return;
            }

            var custodian = UsersService.GetUser(ConfigurationHelper.DataCustodianUserEmail);

            // reset OAuth for eDiscovery administrator
            ApiContext.SyncplicityUserAppToken = ConfigurationHelper.eDiscoveryAdminToken;
            OAuth.OAuth.Authenticate();

            Console.WriteLine("eDiscovery: putting the data custodian user on legal hold for 30 days...");
            LegalHoldsService.PutOnLegalHold(custodian.Id, TimeSpan.FromDays(30));

            // reset OAuth for data custodian user
            ApiContext.SyncplicityUserAppToken = ConfigurationHelper.DataCustodianUserToken;
            OAuth.OAuth.Authenticate();

            Console.WriteLine("User: creating a content...");
            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return; // something went wrong

            var localFilePath = ConfigurationHelper.UploadFileSmall;
            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);

            Console.WriteLine("User: removing the content permanently...");
            RemoveFilePermanently(localFilePath);

            // reset OAuth for eDiscovery administrator but act On-Behalf-Of the data custodian user
            ApiContext.SyncplicityUserAppToken = ConfigurationHelper.eDiscoveryAdminToken;
            OAuth.OAuth.Authenticate();
            ApiContext.OnBehalfOfUser = custodian.Id;
            
            // go beyond the user possibilities and download earlier deleted and legally held content
            Console.WriteLine("eDiscovery On-Behalf-Of User: Downloading permanently deleted content...");
            DownloadFile(localFilePath, true);
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

        private static void RemoveFilePermanently(string localFilePath)
        {
            var fileToRemove = GetCurrentFile(localFilePath);
            SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, false);
            SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, true);
        }

        private static void DownloadFile(string localFilePath, bool deleted = false)
        {
            var fileToDownload = GetCurrentFile(localFilePath, deleted);

            Console.WriteLine();
            Console.WriteLine("Start File Downloading...");

            var downloadClient = new DownloadClient();

            Console.WriteLine();

            try
            {
                downloadClient.DownloadFileSimple(fileToDownload, ConfigurationHelper.DownloadFolder);
                Console.WriteLine($"Download of {fileToDownload.Filename} to {ConfigurationHelper.DownloadFolder} succeeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Download of {fileToDownload.Filename} to {ConfigurationHelper.DownloadFolder} failed.");
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);

                throw;
            }

        }

        private static File GetCurrentFile(string localFilePath, bool deleted = false)
        {
            // Refresh folder info
            _currentFolder = SyncService.GetFolder(_currentSyncpoint.Id, _currentFolder.FolderId, deleted);
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

            if (isObo)
                ApiContext.OnBehalfOfUser = user.Id;

            // Get default storage endpoint of current user
            var storageEndpoint = StorageEndpointsService.GetStorageEndpoint();
            if (storageEndpoint == null)
            {
                Console.WriteLine();
                Console.WriteLine("Unable to determine the user's storage endpoint. " +
                                  "Content APIs will not be able to proceed.");
                return;
            }
            ApiContext.HasStorageEndpoint = true;

            var newSyncPoint = new SyncPoint
            {
                Name = ConfigurationHelper.SyncpointName + _random.Next(),
                Type = SyncPointType.Custom,
                Path = @"C:\Syncplicity",
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

            // Need to call getSyncPoint to hydrate all the meta data of the syncpoint,
            // in particular we need RootFolderId so that we can create a folder next.
            _currentSyncpoint = SyncPointsService.GetSyncpoint(createdSyncPoints[0].Id);

            // Map syncpoint to device
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                Console.WriteLine($"Mapping the syncpoint {_currentSyncpoint.Id} to machine {ConfigurationHelper.MachineId}");
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

            Console.WriteLine($"Finished SyncPoint Creation. Created new SyncPoint {_currentSyncpoint.Name} with Id: {_currentSyncpoint.Id}");
        }

        private static void CreateFolder()
        {
            if (_currentSyncpoint == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Start Folder Creation...");

            // Internal integer id of syncpoint
            var syncpointId = _currentSyncpoint.Id;

            var newFolder = new Folder
            {
                VirtualPath = $@"\{ConfigurationHelper.FolderName}{_random.Next()}\",
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
                Console.WriteLine($"Unable to find local file {localFilePath}.  Content APIs will not be able to continue.");
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

            Console.WriteLine($"File {localFilePath} uploaded successfully to folder {_currentFolder.Name}.");
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
                    Console.WriteLine(
                        $"Exception caught during UploadCallback: Status Code: {(int) response.StatusCode}, Status Description: {response.StatusDescription}");
                _lastException = e;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught during UploadCallback: {e}");
                _lastException = e;
            }
            finally
            {
                // Must set this, otherwise, hangs when uploadFinished.WaitOne() is called. 
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
