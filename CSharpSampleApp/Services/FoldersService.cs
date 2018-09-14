using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to folder.svc
    /// </summary>
    public class FoldersService : ApiGateway
    {
        #region Static Members

        /// <summary>
        /// Gets url to Folder service.
        /// </summary>
        protected static string FolderUrl => SyncAPIUrlPrefix + "folder.svc/{0}/folder/{1}";

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
            var url = string.Format(FolderUrl, syncpointId, folderId);
            if (!string.IsNullOrEmpty(include))
            {
                url += $"?include={include}";
            }
            return HttpGet<Folder>(url);
        }

        #endregion Methods
    }
}
