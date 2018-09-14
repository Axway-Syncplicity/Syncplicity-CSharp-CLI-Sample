namespace CSharpSampleApp.Services.Search
{
    public class FolderHit : IEntityHit
    {
        /// <summary>
        /// Fields with highlighted fragments. Wrapped with &lt;em&gt; tag
        /// </summary>
        public Highlight Highlight;

        public long Id;

        public string Name;

        public long? ParentFolderId;

        /// <summary>
        /// Relevancy search hit of the file
        /// </summary>
        public double Rank;

        public int SyncpointId;

        public string[] VirtualPath;

        double IEntityHit.Rank => Rank;

        string IEntityHit.Name => Name;
    }
}