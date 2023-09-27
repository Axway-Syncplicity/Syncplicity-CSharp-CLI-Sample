using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    public class SyncService : ApiGateway
    {
        #region Static Members

        private static string _FoldersPOSTUrl = SyncAPIUrlPrefix + "folders.svc/{0}/folders";

        #endregion Static Members

        #region Protected Properties

        /// <summary>
        /// Gets the url to FolderFoldersService.
        /// </summary>
        protected static string FolderFoldersUrl => SyncAPIUrlPrefix + "folder_folders.svc/{0}/folder/{1}/folders";

        /// <summary>
        /// Gets the url to FolderService.
        /// </summary>
        protected static string FolderUrl => SyncAPIUrlPrefix + "folder.svc/{0}/folder/{1}";

        /// <summary>
        /// Gets the url to FolderService.
        /// </summary>
        protected static string FolderUrlPut => SyncAPIUrlPrefix + "folder.svc/{0}/folder";

        /// <summary>
        /// Gets the url to FoldersService.
        /// </summary>
        protected static string FoldersUrl => SyncAPIUrlPrefix + "folders.svc/{0}/folders?virtual_path={1}";

        /// <summary>
        /// Gets the url to FileService.
        /// </summary>
        public static string FileUrl => SyncAPIUrlPrefix + "file.svc/{0}/file/{1}";

        /// <summary>
        /// Gets the url to FileService.
        /// </summary>
        public static string FileUrlPut => SyncAPIUrlPrefix + "file.svc/{0}/file";

        public static string FolderFilesUrl => SyncAPIUrlPrefix + "folder_files.svc/{0}/folder/{1}/files";

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Gets Folder object by syncpoint Id and folder Id.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of folder.</param>
        /// <param name="deleted">If set then return deleted files.</param>
        /// <returns></returns>
        public static Folder GetFolder(long syncpointId, long folderId, bool deleted = false)
        {
            var includes = "active";
            if (deleted)
            {
                includes += ",deleted";
            }
            return HttpGet<Folder>(GetUrl(syncpointId, folderId, includes, string.Empty, FolderUrl));
        }

        /// <summary>
        /// Gets root folder of syncpoint.
        /// </summary>
        /// <param name="syncpoint">The syncpoint object.</param>
        /// <returns></returns>
        public static Folder GetFolder(SyncPoint syncpoint)
        {
            return GetFolder(syncpoint.Id, syncpoint.RootFolderId);
        }

        /// <summary>
        /// Creates new folders in parent folder in syncpoint.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of parent folder or Id of root folder of syncpoint.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] CreateFolderFolders(long syncpointId, long folderId, Folder[] folders)
        {
            return HttpPost(string.Format(FolderFoldersUrl, syncpointId, folderId), folders);
        }

        /// <summary>
        /// Get folders in parent folder in syncpoint.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of parent folder or Id of root folder of syncpoint.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] GetFolderFolders(long syncpointId, long folderId)
        {
            return GetFolders(syncpointId, folderId);
        }

        /// <summary>
        /// Gets Folders array object by syncpoint Id and folder Id.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of folder.</param>
        /// <param name="deleted">If set then return deleted files.</param>
        /// <returns></returns>
        public static Folder[] GetFolders(long syncpointId, long folderId, bool deleted = false)
        {
            var includes = "active";
            if (deleted)
            {
                includes += ",deleted";
            }
            return HttpGet<Folder[]>(string.Format(FolderFoldersUrl, syncpointId, folderId));
        }

        /// <summary>
        /// Creates new folders in syncpoint.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] CreateFolders(long syncpointId, Folder[] folders)
        {
            return HttpPost(string.Format(_FoldersPOSTUrl, syncpointId), folders);
        }

        /// <summary>
        /// Renames folder in syncpoint
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint</param>
        /// <param name="folder">The folder to rename</param>
        /// <returns></returns>
        public static Folder UpdateFolder(long syncpointId, Folder folder)
        {
            return HttpPut(string.Format(FolderUrlPut, syncpointId), folder);
        }

        /// <summary>
        /// Creates new folder into the root folder of syncpoint.
        /// </summary>
        /// <param name="syncpoint">The syncpoint object.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] CreateFolderFolders(SyncPoint syncpoint, Folder[] folders)
        {
            return CreateFolderFolders(syncpoint.Id, syncpoint.RootFolderId, folders);
        }

        /// <summary>
        /// Creates new folders into the folder.
        /// </summary>
        /// <param name="folder">The parent folder object.</param>
        /// <param name="folders">The created folders.</param>
        /// <returns></returns>
        public static Folder[] CreateFolderFolders(Folder folder, Folder[] folders)
        {
            return CreateFolderFolders(folder.SyncpointId, folder.FolderId, folders);
        }

        /// <summary>
        /// Returns a file by given syncpoint id and file id
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint</param>
        /// <param name="fileId">The Id of file</param>
        /// <returns></returns>
        public static File GetFile(long syncpointId, long fileId)
        {
            return HttpGet<File>(string.Format(FileUrl, syncpointId, fileId));
        }

        public static File[] GetFilesInFolder(long syncpointId, long folderId)
        {
            return HttpGet<File[]>(string.Format(FolderFilesUrl, syncpointId, folderId));
        }

        /// <summary>
        /// Renames file in folder
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint</param>
        /// <param name="file">The file to rename</param>
        /// <return></returns>
        public static File RenameFile(long syncpointId, File file)
        {
            return HttpPut(string.Format(FileUrlPut, syncpointId), file);
        }

        #endregion

        #region Private Methods

        private static string GetUrl(long syncpointId, long folderId, string include, string modifier, string baseUrl)
        {
            var uri = string.Format(baseUrl, syncpointId, folderId);

            if (!string.IsNullOrEmpty(include) && !string.IsNullOrEmpty(modifier))
            {
                uri = $"{uri}?include={include}&modifier={modifier}";
            }
            else if (!string.IsNullOrEmpty(include))
            {
                uri = $"{uri}?include={include}";
            }
            else if (!string.IsNullOrEmpty(modifier))
            {
                uri = $"{uri}?modifier={modifier}";
            }

            return uri;
        }


        #endregion Private Methods

        public static void RemoveFolder(long syncpointId, long folderId, bool removePermanently)
        {
            HttpDelete<Folder>(GetUrl(syncpointId, folderId, string.Empty,
                removePermanently ? "permanently" : string.Empty, FolderUrl));
        }

        public static Folder[] RemoveFolderFolders(long syncpointId, long folderId, Folder[] folders)
        {
            return HttpDelete<Folder[]>(string.Format(FolderFoldersUrl, syncpointId, folderId), folders);
        }

        public static void RemoveFile(long syncpointId, long fileId, bool removePermanently)
        {
            HttpDelete<File>(GetUrl(syncpointId, fileId, string.Empty,
                removePermanently ? "permanently" : string.Empty, FileUrl));
        }
    }
}
