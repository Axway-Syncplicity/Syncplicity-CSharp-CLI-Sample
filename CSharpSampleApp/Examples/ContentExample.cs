using System;
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
        private static SyncPoint currentSyncpoint = null;
        private static Folder currentFolder = null;

        private static Exception _lastException;
        private static readonly AutoResetEvent _uploadFinished = new AutoResetEvent(false);
        private static User oldOwner;
        private static File currentFile;

        /*
        * Content
        * - Creating a Syncpoint to allow uploads/downloads to folders
        * - Creating a folder
        * - Uploading a file into the folder
        * - Downloading the uploaded file        
        * - Removing the uploaded file
        * - Removing the folder
        * - Changing owner of the syncpoint
        */
        public static void execute()
        {
            createSyncpoint();
            if (APIContext.HasStorageEndpoint)
            {
                createFolder();
                uploadFile();
                downloadFile();
                removeFile();
                removeFolder();
                changeOwnerOfSyncpoint(ConfigurationHelper.NewSyncpointOwnerEmail);
            }
        }

        public static void executeObo()
        {
            if (APIContext.HasStorageEndpoint && onBehalOfPrepare())
            {
                createFolder();
                uploadFile();
                downloadFile();
                removeFile();
                removeFolder();
            }
        }

        private static void removeFolder()
        {
            SyncService.RemoveFolder(currentFolder.SyncpointId, currentFolder.FolderId, false);
        }

        private static void removeFile()
        {
            var fileToRemove = GetCurrentFile();
            SyncService.RemoveFile(fileToRemove.SyncpointId, fileToRemove.LatestVersionId, false);
        }

        private static void downloadFile()
        {
            var fileToDownload = GetCurrentFile();

            Console.WriteLine();
            Console.WriteLine("Start File Downloading...");

            DownloadClient downloadClient = new DownloadClient();

            Console.WriteLine();

            try
            {
                if (downloadClient.DownloadFile(fileToDownload, ConfigurationHelper.DownloadFolder))
                {
                    Console.WriteLine("Download of {0} to {1} is complete.", fileToDownload.Filename, ConfigurationHelper.DownloadFolder);
                }
                else
                {
                    Console.WriteLine("Download of {0} to {1} is failed.", fileToDownload.Filename, ConfigurationHelper.DownloadFolder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Download of {0} to {1} is failed.", fileToDownload.Filename, ConfigurationHelper.DownloadFolder);
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);
            }

        }

        private static File GetCurrentFile()
        {
            if (currentFile != null)
            {
                return currentFile;
            }
            //refresh folder info
            currentFolder = SyncService.GetFolder(currentSyncpoint.Id, currentFolder.FolderId);
            currentFile =
                currentFolder.Files.First(x => x.Filename == Path.GetFileName(ConfigurationHelper.UploadFile));
            return currentFile;
        }

        private static bool onBehalOfPrepare()
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
            createSyncpointInternal(user, true);

            return true;
        }

        private static void createSyncpoint()
        {
            Console.WriteLine();
            Console.WriteLine("Start Common Requests...");

            var user = UsersService.GetUser(ConfigurationHelper.OwnerEmail);
            createSyncpointInternal(user);
        }

        private static void createSyncpointInternal(User user, bool isObo = false)
        {
            Console.WriteLine();
            Console.WriteLine("Start SyncPoint Creation...");

            //get storage endpoint of current user need admin rights
            var storageEndpoint = StorageEndpointsService.GetStorageEndpoint(user.Id);

            if (storageEndpoint == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Unable to determine the user's storage endpoint.  Content apis will not be able to proceed.");
                return;
            }
            APIContext.HasStorageEndpoint = true;
            if (isObo)
                APIContext.OnBehalfOfUser = user.Id;

            Random random = new Random();
            SyncPoint newSyncPoint = new SyncPoint()
            {
                Name = ConfigurationHelper.SyncpointName + random.Next(),
                Type = SyncPointType.Custom,
                Path = "C:\\Synplicity",
                StorageEndpointId = storageEndpoint.Id,
                Mapped = true,
                DownloadEnabled = true,
                UploadEnabled = true
            };

            SyncPoint[] createdSyncPoints = SyncPointsService.CreateSyncpoints(new SyncPoint[] { newSyncPoint });

            Console.WriteLine();

            if (createdSyncPoints == null || createdSyncPoints.Length == 0)
            {
                Console.WriteLine("Error occured during creating SyncPoint.");
                return;
            }
            else
            {
                //currentSyncpoint = createdSyncPoints[0];

                //Need to call getSyncPoint to hydrate all the meta data of the syncpoint, in particular
                //we need RootFolderId so that we can create a folder next.
                currentSyncpoint = SyncPointsService.GetSyncpoint(createdSyncPoints[0].Id);
            }

            //map syncpoint to device
            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                Console.WriteLine("Mapping the syncpoint {0} to machine {1}", currentSyncpoint.Id,
                    ConfigurationHelper.MachineId);
                currentSyncpoint.Mappings = new Mapping[]
                {
                    new Mapping()
                    {
                        SyncPointId = currentSyncpoint.Id,
                        Mapped = true,
                        Machine = new Machine() {Id = ConfigurationHelper.MachineId}
                    }
                };
                currentSyncpoint = SyncPointsService.PutSyncpoint(currentSyncpoint);
            }

            Console.WriteLine("Finished SyncPoint Creation. Created new SyncPoint {0} with Id: {1}", currentSyncpoint.Name, currentSyncpoint.Id);
        }

        public static void createFolder()
        {
            if (currentSyncpoint == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Start Folder Creation...");

            long syncpointId = currentSyncpoint.Id;            // internal integer id of syncpoint

            Random random = new Random();
            Folder newFolder = new Folder()
            {
                VirtualPath = string.Format(@"\{0}{1}\", ConfigurationHelper.FolderName, random.Next()),
                Status = FolderStatus.Added,
                SyncpointId = syncpointId
            };

            Folder[] createdFolders = SyncService.CreateFolders(syncpointId, new [] { newFolder });

            Console.WriteLine();

            if (createdFolders == null || createdFolders.Length == 0)
            {
                Console.WriteLine("Error occured during creating new folder. Content apis will not be able to continue.");
                return;
            }
            else
            {
                currentFolder = createdFolders[0];
            }

            Console.WriteLine(String.Format("Finished Folder Creation. Created new Folder {0} with Id: {1}", createdFolders[0].Name, createdFolders[0].FolderId));
        }

        private static void uploadFile()
        {
            if (currentFolder == null)
            {
                return;
            }

            String localFileName = ConfigurationHelper.UploadFile;

            Console.WriteLine();

            if (!(new FileInfo(localFileName)).Exists)
            {
                Console.WriteLine("Unable to find local file {0}.  Content apis will not be able to continue.", localFileName);
                throw new ConfigurationErrorsException("Cannot find the file defined as the value of \"uploadFile\" configuration");
            }

            Console.WriteLine("Start File Uploading...");

            try
            {
                new UploadClient(currentFolder, localFileName, string.Empty, UploadCallBack).UploadFile();
                _uploadFinished.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Uploading file failed.");
                Console.WriteLine("Exception caught:");
                Console.WriteLine(e);
                _uploadFinished.Set();
                throw;
            }

            Console.WriteLine();

            if (_lastException != null)
            {
                Console.WriteLine("Uploading file failed.");
                throw _lastException;
            }

            Console.WriteLine("File {0} uploaded successfully to folder {1}.", localFileName, currentFolder.Name);
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
                _uploadFinished.Set();
            }
        }

        private static void changeOwnerOfSyncpoint(string newOwnerEmail)
        {
            if (string.IsNullOrEmpty(newOwnerEmail) || newOwnerEmail == "REPLACE_WITH_NEW_SYNCPOINT_OWNER_EMAIL")
            {
                Console.WriteLine();
                Console.WriteLine("New owner is not defined - skipping change syncpoint's owner");

                return;
            }
            Console.WriteLine();
            Console.WriteLine("Start changing the owner of syncpoint...");

            oldOwner = currentSyncpoint.Owner;
            currentSyncpoint.Owner = new User() {EmailAddress = newOwnerEmail };
            currentSyncpoint = SyncPointsService.PutSyncpoint(currentSyncpoint);
            ParticipantsService.RemoveParticipants(currentSyncpoint.Id, oldOwner.EmailAddress);

            Console.WriteLine();
            Console.WriteLine("Owner of syncpoint has been changed...");
        }
    }
}
