using CSharpSampleApp.Entities.Tagging;
using System.Collections.Generic;

namespace CSharpSampleApp.Services
{
    public class TagService : ApiGateway
    {
        /// <summary>
        /// Tags for Folder in Syncpoint
        /// </summary>
        protected static string FolderTagsUrl => SyncpointAPIUrlPrefix + "{0}/folder/{1}/tags";

        /// <summary>
        /// Tags for File in Syncpoint
        /// </summary>
        protected static string FileTagsUrl => SyncpointAPIUrlPrefix + "{0}/file/{1}/tags";

        /// <summary>
        /// Adds tags to a folder.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="folderId">Id of the folder to get tags</param>
        /// <param name="tags">A list of tags to add to a folder.</param>
        /// <returns>Collection of the successfully added tags to the folder if any</returns>
        public static List<Tag> PostTagsForFolder(long syncpointId, long folderId, List<Tag> tags)
        {
            return HttpPost(string.Format(FolderTagsUrl, syncpointId, folderId), tags);
        }

        /// <summary>
        /// Retrieves all tags defined for a folder.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="folderId">Id of the folder to get tags</param>
        /// <returns>Collection of tags for the folder if any</returns>
        public static List<Tag> GetTagsForFolder(long syncpointId, long folderId)
        {
            return HttpGet<List<Tag>>(string.Format(FolderTagsUrl, syncpointId, folderId));
        }

        /// <summary>
        /// Deletes a set of tags from a folder.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="folderId">Id of the folder to delete tags</param>
        /// <param name="tags">The list of tags to delete.</param>
        public static void DeleteTagsForFolder(long syncpointId, long folderId, List<Tag> tags)
        {
            HttpDelete<List<Tag>>(string.Format(FolderTagsUrl, syncpointId, folderId), tags);
        }

        /// <summary>
        /// Adds tags to a file.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="fileId">Id of the folder to get tags</param>
        /// <param name="tags">A list of tags to add to a folder.</param>
        /// <returns>Collection of the successfully added tags to the file if any</returns>
        public static List<Tag> PostTagsForFile(long syncpointId, long fileId, List<Tag> tags)
        {
            return HttpPost(string.Format(FileTagsUrl, syncpointId, fileId), tags);
        }

        /// <summary>
        /// Retrieves all tags defined for a file.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="fileId">Id of the file to get tags</param>
        /// <returns>Collection of tags for the file if any</returns>
        public static List<Tag> GetTagsForFile(long syncpointId, long fileId)
        {
            return HttpGet<List<Tag>>(string.Format(FileTagsUrl, syncpointId, fileId));
        }

        /// <summary>
        /// Deletes a set of tags from a file.
        /// </summary>
        /// <param name="syncpointId">Id of the syncpoint containing the target folder</param>
        /// <param name="fileId">Id of the file to delete tags</param>
        /// <param name="tags">The list of tags to delete.</param>
        public static void DeleteTagsForFile(long syncpointId, long fileId, List<Tag> tags)
        {
            HttpDelete<List<Tag>>(string.Format(FileTagsUrl, syncpointId, fileId), tags);
        }
    }
}
