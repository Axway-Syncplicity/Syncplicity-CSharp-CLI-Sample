namespace CSharpSampleApp.Services.Search
{
    public class FileHit : IEntityHit
    {
        /// <summary>
        /// ISO 8601 formatted modification date (UTC)
        /// </summary>
        public string DateModified;

        /// <summary>
        /// Folder Id of the folder where the file resides
        /// </summary>
        public long FolderId;

        /// <summary>
        /// Fields with highlighted fragments. Wrapped with &lt;em&gt; tag
        /// </summary>
        public Highlight Highlight;

        public long Id;

        public long LatestVersionId;

        /// <summary>
        /// File size (in bytes)
        /// </summary>
        public long Length;

        public string Name;

        /// <summary>
        /// Relevancy search hit of the file
        /// </summary>
        public double Rank;

        public int SyncpointId;

        /// <summary>
        /// Virtual Path of the folder where the file resides
        /// </summary>
        public string[] VirtualPath;

        double IEntityHit.Rank => Rank;

        string IEntityHit.Name => Name;
    }
}