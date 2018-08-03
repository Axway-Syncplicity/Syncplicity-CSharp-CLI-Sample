using CSharpSampleApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected static string FolderFoldersUrl { get; } = SyncAPIUrlPrefix + "folder_folders.svc/{0}/folder/{1}/folders";

        /// <summary>
        /// Gets the url to FolderService.
        /// </summary>
        protected static string FolderUrl { get; } = SyncAPIUrlPrefix + "folder.svc/{0}/folder/{1}";

        public static string FileUrl { get; } = SyncAPIUrlPrefix + "file.svc/{0}/file/{1}";

        /// <summary>
        /// Gets the url to FoldersService.
        /// </summary>
        protected static string FoldersUrl { get; } = SyncAPIUrlPrefix + "folders.svc/{0}/folders?virtual_path={1}";

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Gets Folder object by syncpoint Id and folder Id.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of folder.</param>
        /// <returns></returns>
        public static Folder GetFolder(long syncpointId, long folderId)
        {
            return HttpGet<Folder>(GetUrl(syncpointId, folderId, "active", string.Empty, FolderUrl));
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
        /// Creates new folders in syncpoint.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of parent folder or Id of root folder of syncpoint.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] CreateFolders(long syncpointId, long folderId, Folder[] folders)
        {
            return HttpPost(string.Format(FolderFoldersUrl, syncpointId, folderId), folders);
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
        /// Creates new folder into the root folder of syncpoint.
        /// </summary>
        /// <param name="syncpoint">The syncpoint object.</param>
        /// <param name="folders">The created folder.</param>
        /// <returns></returns>
        public static Folder[] CreateFolders(SyncPoint syncpoint, Folder[] folders)
        {
            return CreateFolders(syncpoint.Id, syncpoint.RootFolderId, folders);
        }

        /// <summary>
        /// Creates new folders into the folder.
        /// </summary>
        /// <param name="folder">The parent folder object.</param>
        /// <param name="folders">The created folders.</param>
        /// <returns></returns>
        public static Folder[] CreateFolders(Folder folder, Folder[] folders)
        {
            return CreateFolders(folder.SyncpointId, folder.FolderId, folders);
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

        public static void RemoveFile(long syncpointId, long fileId, bool removePermanently)
        {
            HttpDelete<File>(GetUrl(syncpointId, fileId, string.Empty,
                removePermanently ? "permanently" : string.Empty, FileUrl));
        }
    }
}
