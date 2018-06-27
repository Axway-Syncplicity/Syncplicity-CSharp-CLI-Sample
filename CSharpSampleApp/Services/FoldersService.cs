using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to folder.svc
    /// </summary>
    public class FoldersService : APIGateway
    {
        #region Static Members

        private static string _FolderUrl = SyncAPIUrlPrefix + "folder.svc/{0}/folder/{1}";

        /// <summary>
        /// Gets url to Folder service.
        /// </summary>
        protected static string FolderUrl
        {
            get { return _FolderUrl; }
        }

        #endregion Static Members

        #region Methods

        /// <summary>
        /// Gets folder info by syncpoint Id and folder Id.
        /// </summary>
        /// <param name="syncpointId">Syncpoint Id.</param>
        /// <param name="folderId">Folder Id.</param>
        /// <param name="include">Include parameter.</param>
        /// <returns>Folder info.</returns>
        public static Folder GetFolder(int syncpointId, int folderId, string include = "active")
        {
            string url = string.Format(FolderUrl, syncpointId, folderId);
            if (!string.IsNullOrEmpty(include))
            {
                url += string.Format("?include={0}", include);
            }
            return HttpGet<Folder>(url);
        }

        #endregion Methods
    }
}
