using CSharpSampleApp.Entities;
using CSharpSampleApp.Entities.Tagging;
using CSharpSampleApp.Services;
using CSharpSampleApp.Services.Download;
using CSharpSampleApp.Services.Upload;
using CSharpSampleApp.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using File = CSharpSampleApp.Entities.File;

namespace CSharpSampleApp.Examples
{
    public class ContentExample
    {
        private static SyncPoint _currentSyncpoint;
        private static Folder _currentFolder;
        private static Link _currentSharedLink;

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
            RemoveFilePermanently(localFilePath);
            RemoveFolderPermanently();
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
            RemoveFilePermanently(localFilePath);
            RemoveFolderPermanently();
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
            RemoveFilePermanently(localFilePath);
            RemoveFolderPermanently();

            ApiContext.OnBehalfOfUser = null;
        }

        /*
       * Content - Rename Folder
       * - Creating a Syncpoint to allow uploads/downloads to folders
       * - Creating a folder
       * - Renaming the folder
       * - Removing the folder
       */
        public static void ExecuteRenameFolder()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            RenameFolder();
            RemoveFolderPermanently();
        }

        public static void ExecuteNewsFeedEvents()
        {
            // In order to generate some events
            if (_currentFolder == null)
            {
                CreateSyncpoint();
                CreateFolder();
            }

            EventsService.GetNewsFeedEventsForUser();
        }

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
        public static void ExecuteRenameFile()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            var localFilePath = ConfigurationHelper.UploadFileSmall;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);
            var renamedFile = RenameFile(localFilePath);
            RemoveFilePermanently(renamedFile);
            RemoveFolderPermanently();
        }

        /*
        * Content - Folder Tagging
        * - Creating a Syncpoint to allow uploads/downloads to folders
        * - Creating a folder
        * - Add tags for folder
        * - Gets tags for folder
        * - Deleting tags for folder
        * - Removing the folder
        */
        public static void ExecuteFolderTagging()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            PostTagsForFolder();
            GetTagsForFolder();
            DeleteTagsForFolder();
            RemoveFolderPermanently();
        }

        /*
        * Content - File Tagging
        * - Creating a Syncpoint to allow uploads/downloads to folders
        * - Creating a folder
        * - Upload a file
        * - Add tags for file
        * - Gets tags for file
        * - Deleting tags for file
        * - Removing the file
        * - Removing the folder
        */
        public static void ExecuteFileTagging()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            var localFilePath = ConfigurationHelper.UploadFileSmall;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);
            var file = GetCurrentFile(localFilePath);
            PostTagsForFile(file);
            GetTagsForFile(file);
            DeleteTagsForFile(file);
            RemoveFilePermanently(localFilePath);
            RemoveFolderPermanently();
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
            ApiGateway.Authenticate();

            Console.WriteLine("eDiscovery: putting the data custodian user on legal hold for 30 days...");
            LegalHoldsService.PutOnLegalHold(custodian.Id, TimeSpan.FromDays(30));

            // reset OAuth for data custodian user
            ApiContext.SyncplicityUserAppToken = ConfigurationHelper.DataCustodianUserToken;
            ApiGateway.Authenticate();

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
            ApiGateway.Authenticate();
            ApiContext.OnBehalfOfUser = custodian.Id;

            // go beyond the user possibilities and download earlier deleted and legally held content
            Console.WriteLine("eDiscovery On-Behalf-Of User: Downloading permanently deleted content...");
            DownloadFile(localFilePath, true);

            RemoveFolderPermanently();
        }

        #region LinksCategory
        /// <summary>
        /// GET /syncpoint/links.svc/
        /// returns collection of Links
        /// </summary>
        public static void ExecuteLinksGet()
        {
            LinksService.GetSharedLinks();
        }

        /// <summary>
        /// POST /syncpoint/links.svc/
        /// receives collection of Links to create
        /// returns collection of created Links
        /// </summary>
        public static void ExecuteLinksPost()
        {
            // Prepare syncpoint, folder and file
            var localFilePath = ConfigurationHelper.UploadFileExcel;
            if (!ValidateConfigurationForSharedLinkSample(localFilePath))
            {
                return;
            }
            CreateSyncpoint();
            CreateFolder();
            UploadFile(localFilePath, UploadMode.Simple);

            var linkData = JsonConvert.DeserializeObject<LinkData>(ConfigurationHelper.LinksContributeToFolderData);

            var sharedLinks = CreateSharedLinks(linkData);
            _currentSharedLink = sharedLinks.FirstOrDefault();

            RemoveFolderPermanently();
        }

        /// <summary>
        /// POST /syncpoint/links.svc/ with contribute permission to folder
        /// receives collection of Links to create
        /// returns collection of created Links
        /// </summary>
        public static void ExecuteLinksWithContributePermissionsToFolderPost()
        {
            // Prepare syncpoint, folder
            var localFilePath = ConfigurationHelper.UploadFileExcel;
            if (!ValidateConfigurationForSharedLinkSample(localFilePath))
            {
                return;
            }
            CreateSyncpoint();
            CreateFolder();

            var linkData = JsonConvert.DeserializeObject<LinkData>(ConfigurationHelper.LinksContributeToFolderData);

            var sharedLinks = CreateSharedLinksWithContributePermissionsToFolder(linkData);
            _currentSharedLink = sharedLinks.FirstOrDefault();

            //RemoveFolderPermanently();
        }

        /// <summary>
        /// GET /syncpoint/link.svc/{TOKEN}
        /// returns Link entity that matches the token
        /// </summary>
        public static void ExecuteLinkTokenGet()
        {
            if (_currentSharedLink == null)
            {
                Console.WriteLine("You need to call ExecuteLinksPost() first");
                return;
            }
            LinkService.GetSharedLink(_currentSharedLink.Token);
        }

        /// <summary>
        /// PUT /syncpoint/link.svc/{TOKEN}
        /// updates Link entity that matches the token
        /// returns updated entity
        /// </summary>
        public static void ExecuteLinkTokenPut()
        {
            if (_currentSharedLink == null)
            {
                Console.WriteLine("You need to call ExecuteLinksPost() first");
                return;
            }
            var linkData = JsonConvert.DeserializeObject<LinkData>(ConfigurationHelper.LinksData);

            Console.WriteLine($"Shared Link BEFORE update - {JsonConvert.SerializeObject(_currentSharedLink, Formatting.Indented)}");
            _currentSharedLink.Message = linkData.NewMessage;
            _currentSharedLink = LinkService.UpdateSharedLink(_currentSharedLink.Token, _currentSharedLink);
            Console.WriteLine($"Shared Link AFTER update - {JsonConvert.SerializeObject(_currentSharedLink, Formatting.Indented)}");
        }

        /// <summary>
        /// DELETE /syncpoint/link.svc/{TOKEN}
        /// deletes entity that matches the token
        /// doesn't return anything
        /// </summary>
        public static void ExecuteLinkTokenDelete()
        {
            if (_currentSharedLink == null)
            {
                Console.WriteLine("You need to call ExecuteLinksPost() first");
                return;
            }
            var localFilePath = ConfigurationHelper.UploadFileExcel;
            LinkService.DeleteSharedLink(_currentSharedLink.Token);
            RemoveFilePermanently(localFilePath);
            RemoveFolderPermanently();
        }

        #endregion

        #region FolderFolders
        /*
         * Content - Create Nested Folders
         * - Creating a Syncpoint
         * - Creating apParent folder
         * - Creating nested folders into parent folder
         * - Removing the folders
        */
        public static void ExecuteCreateNestedFolder()
        {
            if (!ValidateConfigurationForOrdinarySample()) return;

            CreateSyncpoint();

            if (!ApiContext.HasStorageEndpoint) return;

            CreateFolder();
            CreateNestedFolders();
            RemoveFoldersPermanently();
        }

        #endregion

        private static bool ValidateConfigurationForSharedLinkSample(string localFilePath)
        {
            var errors = EvaluateConfigValidationRulesForOrdinarySample().ToList();
            if (errors.Any())
            {
                Console.WriteLine(string.Join(Environment.NewLine, $"Error:{errors}"));
                return false;
            }

            if (!localFilePath.EndsWith(".xlsx") && !localFilePath.EndsWith(".xls"))
            {
                Console.WriteLine($"Error: Unable to execute [Created Shared Links]. Supported file type extensions are \".xls\", \".xlsx\"");
                return false;
            }

            return true;
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

        private static void RemoveFolderPermanently()
        {
            if (_currentFolder != null)
            {
                SyncService.RemoveFolder(_currentFolder.SyncpointId, _currentFolder.FolderId, removePermanently: false);
                SyncService.RemoveFolder(_currentFolder.SyncpointId, _currentFolder.FolderId, removePermanently: true);
                return;
            }

            Console.WriteLine("No folder to remove.");
        }

        private static void RemoveFolderPermanently(long syncpointId, long folderId)
        {
            if (_currentFolder != null)
            {
                SyncService.RemoveFolder(syncpointId, folderId, removePermanently: false);
                SyncService.RemoveFolder(syncpointId, folderId, removePermanently: true);
                return;
            }

            Console.WriteLine("No folder to remove.");
        }

        private static void RemoveFoldersPermanently()
        {
            if (_currentFolder == null)
            {
                Console.WriteLine("No folders to remove.");
                return;
            }

            var folders = SyncService.GetFolderFolders(_currentFolder.SyncpointId, _currentFolder.FolderId);

            foreach (var folder in folders)
            {
                RemoveFolderPermanently(folder.SyncpointId, folder.FolderId);
                Console.WriteLine($"Folder with id: {folder.FolderId} and ParentFolderId: {_currentFolder.FolderId} is Reomved Permanently");
                Console.WriteLine();
            }

            RemoveFolderPermanently(_currentFolder.SyncpointId, _currentFolder.FolderId);
            Console.WriteLine($"Parent Folder with id: {_currentFolder.FolderId} is Reomved Permanently");
        }

        private static void RemoveFilePermanently(string localFilePath)
        {
            var fileToRemove = GetCurrentFile(localFilePath);
            RemoveFilePermanently(fileToRemove);
        }

        private static void RemoveFilePermanently(File fileToRemove)
        {
            if (fileToRemove != null)
            {
                SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, removePermanently: false);
                SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, removePermanently: true);
                return;
            }

            Console.WriteLine("No file to remove.");
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
            var currentFile = _currentFolder.Files.First(x => x.Filename == Path.GetFileName(localFilePath));

            if (currentFile.FolderId != _currentFolder.FolderId)
            {
                currentFile.FolderId = _currentFolder.FolderId;
            }

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
                Type = SyncPointType.SyncplicityDrive,
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

            var createdFolders = SyncService.CreateFolders(syncpointId, new[] { newFolder });

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

        private static void CreateNestedFolders()
        {
            if (_currentSyncpoint == null)
            {
                return;
            }

            // Internal integer id of syncpoint
            var syncpointId = _currentSyncpoint.Id;

            if (_currentFolder == null)
            {
                return;
            }

            // Internal folder id of parentFolder
            var parentFolderId = _currentFolder.FolderId;

            // Create array of folders to be nested under the parent folder with parentFolderId
            var nestedFolders = CreateFoldersCollection(ConfigurationHelper.NumberOfNestedFolders);
            var createdFolderFolders = SyncService.CreateFolderFolders(syncpointId, parentFolderId, nestedFolders);

            if (createdFolderFolders == null || createdFolderFolders.Length == 0)
            {
                Console.WriteLine("Error occurred during creating new folder. Content APIs will not be able to continue.");
                return;
            }

            foreach (var folder in createdFolderFolders)
            {
                Console.WriteLine(
               $"Finished Folder Creation. Created new Folder {folder.Name} with Id: {folder.FolderId} and parent folder id: {folder.ParentFolderId}.");
            }
        }

        private static Folder[] CreateFoldersCollection(int numberOfNestedFolders)
        {
            var nestedFolders = new Folder[numberOfNestedFolders];

            for (int i = 0; i < numberOfNestedFolders; i++)
            {
                var folder = new Folder
                {
                    Name = $@"{ConfigurationHelper.FolderName}{_random.Next()}",
                    Status = FolderStatus.Added
                };

                nestedFolders[i] = folder;
            }

            return nestedFolders;
        }

        private static void RenameFolder()
        {
            try
            {
                string folderNewName = $"{ConfigurationHelper.FolderName}{_random.Next()}";

                Console.WriteLine();
                Console.WriteLine("Start Folder Rename...");
                Console.WriteLine($"Current Folder Name: {_currentFolder.Name} will be replaced with: {folderNewName}");

                _currentFolder.Name = folderNewName;
                _currentFolder.Status = FolderStatus.MovedOrRenamed;

                var renamedFolder = SyncService.UpdateFolder(_currentFolder.SyncpointId, _currentFolder);

                Console.WriteLine($"Finished Folder Rename. Renamed Folder {renamedFolder.Name} with Id: {renamedFolder.FolderId}");
            }
            catch (Exception)
            {
                RemoveFolderPermanently();
                throw;
            }
        }

        private static File RenameFile(string localFilePath)
        {
            try
            {
                var fileToRename = GetCurrentFile(localFilePath);
                var fileNewName = Path.GetFileName(ConfigurationHelper.UploadedFileRenamed);

                Console.WriteLine();
                Console.WriteLine("Start File Rename...");
                Console.WriteLine($"Current File Name: {fileToRename.Filename} will be replaced with: {fileNewName}");

                fileToRename.Filename = fileNewName;
                fileToRename.Status = FileStatus.MovedOrRenamed;

                var renamedFile = SyncService.RenameFile(_currentFolder.SyncpointId, fileToRename);

                Console.WriteLine($"Finished File Rename. Renamed File {renamedFile.Filename} with Id: {renamedFile.FileId}");

                return renamedFile;
            }
            catch (Exception)
            {
                RemoveFilePermanently(localFilePath);
                throw;
            }
        }

        private static IEnumerable<Link> CreateSharedLinks(LinkData linkData)
        {
            Console.WriteLine("Creating Shared Links ...");
            var links = new Link[]
            {
                new Link
                {
                    SyncPointId = _currentFolder.SyncpointId,
                    VirtualPath = $"{_currentFolder.VirtualPath}{linkData.FileName}",
                    LinkExpireInDays = 90,
                    LinkExpirationPolicy = ShareLinkExpirationPolicy.Enabled,
                    PasswordProtectPolicy = ShareLinkPasswordProtectedPolicy.Disabled,
                    RolId = 1,
                    ShareLinkPolicy = ShareLinkPolicy.IntendedOnly,
                    IrmRoleType = IrmRoleType.Reader,
                    IsIrmProtected = true,
                    Users = new User []
                    {
                        new User {EmailAddress = linkData.Email }
                    },
                    Message = linkData.OldMessage
                }
            };
            return LinksService.CreateSharedLinks(links);
        }

        private static IEnumerable<Link> CreateSharedLinksWithContributePermissionsToFolder(LinkData linkData)
        {
            Console.WriteLine("Creating Shared Links ...");
            var links = new Link[]
            {
                new Link
                {
                    RolId = 1,
                    Users = new User []
                    {
                        new User {EmailAddress = linkData.Email } // Recipient
                    },
                    ShareLinkPolicy = ShareLinkPolicy.IntendedOnly,
                    PasswordProtectPolicy = ShareLinkPasswordProtectedPolicy.Disabled,
                    Folder = new Folder
                    {
                        FolderId = _currentFolder.FolderId
                    },
                    SyncPointId = _currentFolder.SyncpointId,
                    ShareResourceType = ShareResourceType.Folder,
                    ShareType = ShareType.ShareLink,
                    LinkPermissionType = LinkPermissionType.Contribute
                }
            };
            return LinksService.CreateSharedLinks(links);
        }

        private static void UploadFile(string localFilePath, UploadMode mode)
        {
            if (_currentFolder == null) return;

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
                        $"Exception caught during UploadCallback: Status Code: {(int)response.StatusCode}, Status Description: {response.StatusDescription}");
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
            _currentSyncpoint.Owner = new User { EmailAddress = newOwnerEmail };
            _currentSyncpoint = SyncPointsService.PutSyncpoint(_currentSyncpoint);
            ParticipantsService.RemoveParticipants(_currentSyncpoint.Id, _oldOwner.EmailAddress);

            Console.WriteLine();
            Console.WriteLine("Owner of syncpoint has been changed...");
        }

        private static void PostTagsForFolder()
        {
            if (_currentFolder == null) return;

            try
            {
                Console.WriteLine();
                Console.WriteLine("Start adding Tags to folder...");

                var tags = JsonConvert.DeserializeObject<List<Tag>>(ConfigurationHelper.TagCollection);
                var postedTags = TagService.PostTagsForFolder(_currentFolder.SyncpointId, _currentFolder.FolderId, tags);

                Console.WriteLine($"Finished adding Tags to Folder {_currentFolder.Name} in Syncpoint {_currentFolder.SyncpointId}.");
                Console.WriteLine($"{JsonConvert.SerializeObject(postedTags)}");
            }
            catch (Exception)
            {
                RemoveFolderPermanently();
                throw;
            }
        }

        private static void GetTagsForFolder()
        {
            if (_currentFolder == null) return;

            try
            {
                Console.WriteLine();
                Console.WriteLine("Start getting folder Tags...");

                var tags = TagService.GetTagsForFolder(_currentFolder.SyncpointId, _currentFolder.FolderId);

                Console.WriteLine($"Finished getting Tags for Folder {_currentFolder.Name} in Syncpoint {_currentFolder.SyncpointId}. Tags count: {tags.Count}");
                Console.WriteLine($"{JsonConvert.SerializeObject(tags)}");
            }
            catch (Exception)
            {
                RemoveFolderPermanently();
                throw;
            }
        }

        private static void DeleteTagsForFolder()
        {
            if (_currentFolder == null) return;

            try
            {
                Console.WriteLine();
                Console.WriteLine("Start deleting Tags...");

                var tags = JsonConvert.DeserializeObject<List<Tag>>(ConfigurationHelper.TagCollection);
                TagService.DeleteTagsForFolder(_currentFolder.SyncpointId, _currentFolder.FolderId, tags);

                Console.WriteLine($"Finished deleting Tags of Folder {_currentFolder.Name} in Syncpoint {_currentFolder.SyncpointId}.");
            }
            catch (Exception)
            {
                RemoveFolderPermanently();
                throw;
            }
        }

        private static void PostTagsForFile(File file)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Start adding Tags to file...");

                var tags = JsonConvert.DeserializeObject<List<Tag>>(ConfigurationHelper.TagCollection);
                var postedTags = TagService.PostTagsForFile(file.SyncpointId, file.FileId, tags);

                Console.WriteLine($"Finished adding Tags to File {file.Filename} in Syncpoint {file.SyncpointId}.");
                Console.WriteLine($"{JsonConvert.SerializeObject(postedTags)}");
            }
            catch (Exception)
            {
                RemoveFilePermanently(file);
                RemoveFolderPermanently();
                throw;
            }
        }

        private static void GetTagsForFile(File file)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Start getting Tags for file...");

                var tags = TagService.GetTagsForFile(file.SyncpointId, file.FileId);

                Console.WriteLine($"Finished adding Tags to File {file.Filename} in Syncpoint {file.SyncpointId}.");
                Console.WriteLine($"{JsonConvert.SerializeObject(tags)}");
            }
            catch (Exception)
            {
                RemoveFilePermanently(file);
                RemoveFolderPermanently();
                throw;
            }
        }

        private static void DeleteTagsForFile(File file)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Start deleting Tags for file...");

                var tags = JsonConvert.DeserializeObject<List<Tag>>(ConfigurationHelper.TagCollection);
                TagService.DeleteTagsForFile(file.SyncpointId, file.FileId, tags);

                Console.WriteLine($"Finished deleting Tags for File {file.Filename} in Syncpoint {file.SyncpointId}.");
            }
            catch (Exception)
            {
                RemoveFilePermanently(file);
                RemoveFolderPermanently();
                throw;
            }
        }

        private enum UploadMode
        {
            Simple,
            Chunked
        }
    }
}
