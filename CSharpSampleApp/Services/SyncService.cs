using CSharpSampleApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSampleApp.Services
{
    public class SyncService : APIGateway
    {
        #region Static Members

        public static string _FolderUrl = SyncAPIUrlPrefix + "folder.svc/{0}/folder/{1}";
        public static string _FileUrl = SyncAPIUrlPrefix + "file.svc/{0}/file/{1}";
        public static string _FoldersUrl = SyncAPIUrlPrefix + "folders.svc/{0}/folders?virtual_path={1}";
        public static string _FolderFoldersUrl = SyncAPIUrlPrefix + "folder_folders.svc/{0}/folder/{1}/folders";
        public static string _FoldersPOSTUrl = SyncAPIUrlPrefix + "folders.svc/{0}/folders";

        #endregion Static Members

        #region Protected Properties

        /// <summary>
        /// Gets the url to FolderFoldersService.
        /// </summary>
        protected static string FolderFoldersUrl
        {
            get { return _FolderFoldersUrl; }
        }

        /// <summary>
        /// Gets the url to FolderService.
        /// </summary>
        protected static string FolderUrl
        {
            get { return _FolderUrl; }
        }

        public static string FileUrl
        {
            get { return _FileUrl; }
        }

        /// <summary>
        /// Gets the url to FoldersService.
        /// </summary>
        protected static string FoldersUrl
        {
            get { return _FoldersUrl; }
        }

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
        /// Ctreates new folders in syncpoint.
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
        /// Ctreates new folders in syncpoint.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="folderId">The Id of parent folder or Id of root folder of syncpoint.</param>
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
            string uri = string.Format(baseUrl, syncpointId, folderId);

            if (!String.IsNullOrEmpty(include) && !String.IsNullOrEmpty(modifier))
            {
                uri = String.Format("{0}?include={1}&modifier={2}", uri, include, modifier);
            }
            else if (!String.IsNullOrEmpty(include))
            {
                uri = String.Format("{0}?include={1}", uri, include);
            }
            else if (!String.IsNullOrEmpty(modifier))
            {
                uri = String.Format("{0}?modifier={1}", uri, modifier);
            }

            return uri;
        }

        #endregion Private Methods

        public static void RemoveFolder(long syncpointId, long folderId, bool removePermanently)
        {
            HttpDelete<Folder>(GetUrl(syncpointId, folderId, String.Empty,
                removePermanently ? "permanently" : String.Empty, FolderUrl));
        }

        public static void RemoveFile(long syncpointId, long fileId, bool removePermanently)
        {
            HttpDelete<File>(GetUrl(syncpointId, fileId, String.Empty,
                removePermanently ? "permanently" : String.Empty, FileUrl));
        }
    }
}
