namespace CSharpSampleApp.Services.Search
{
    public class SearchResponse
    {
        /// <summary>
        /// List of found files.
        /// </summary>
        public FileHit[] Files;

        /// <summary>
        /// List of found folders.
        /// </summary>
        public FolderHit[] Folders;

        /// <summary>
        /// Indicates partial results
        /// </summary>
        public bool TimedOut;

        /// <summary>
        /// Indicates time taken to execute the search request (in ms)
        /// </summary>
        public long TimeTaken;

        /// <summary>
        /// The total number of files and folders found by the search endpoint.
        /// </summary>
        /// <remarks>
        /// This property specifies the total number of files and folders that have been found for a given search query
        /// on the search endpoint. Search results are returned in pages with maximum page size of 100.
        /// This property allows to know if there are more pages than the first one, and how many results have been found in total.
        /// Keep in mind that a search query might be executed on multiple search endpoints,
        /// and this property will contain the number of hits found on a given search endpoint.
        /// It does not tell the number of hits found on all endpoints aggregated - just on the endpoint that returned the value.
        /// </remarks>
        public long TotalResultsCount;
    }
}
