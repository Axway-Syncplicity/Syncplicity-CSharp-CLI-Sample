using System;
using System.Collections.Generic;
using System.Linq;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Examples.Search;
using CSharpSampleApp.Services;
using CSharpSampleApp.Services.Search;
using CSharpSampleApp.Util;

namespace CSharpSampleApp.Examples
{
    internal static class SearchExample
    {
        private static List<SearchEndpoint> _searchEndpoints;
        private static List<SearchResult> _searchResults;

        /// <summary>
        /// Executes Search Example
        /// </summary>
        internal static void Execute()
        {
            DiscoverSearchEndpoints();
            ExecuteSearchRequests();
            MergeSearchResults();
        }

        private static void DiscoverSearchEndpoints()
        {
            _searchEndpoints = new List<SearchEndpoint>();

            var storageEndpoints = StorageEndpointsService.GetStorageEndpointsWithSearchEndpoints();

            var searchEndpoints = storageEndpoints
                .Select(PickSearchEndpoint)
                .Where(url => url != null);

            foreach (var searchEndpoint in searchEndpoints)
            {
                AddIfNotYetPresent(searchEndpoint);
            }
        }

        private static void AddIfNotYetPresent(SearchEndpoint searchEndpoint)
        {
            if (_searchEndpoints.Any(e => e.Id == searchEndpoint.Id)) return;

            _searchEndpoints.Add(searchEndpoint);
        }

        private static SearchEndpoint PickSearchEndpoint(StorageEndpoint storageEndpoint)
        {
            var searchCapability = storageEndpoint.Search;
            if (searchCapability == null) return null;

            var enabledEndpoints = searchCapability.Endpoints
                .Where(e => e.Status == SearchEndpointStatus.Enabled)
                .ToList();

            var contentAndMetadataEndpoint =
                enabledEndpoints.FirstOrDefault(e => e.Type == SearchEndpointType.ContentAndMetadata);
            if (contentAndMetadataEndpoint != null) return contentAndMetadataEndpoint;

            var metadataEndpoint = enabledEndpoints
                .FirstOrDefault(e => e.Type == SearchEndpointType.MetadataOnly);
            return metadataEndpoint;
        }

        private static void ExecuteSearchRequests()
        {
            var searchClients = _searchEndpoints.Select(e => new SearchClient(e));

            _searchResults = new List<SearchResult>();
            foreach (var searchClient in searchClients)
            {
                var searchResult = searchClient.Search(ConfigurationHelper.SearchQuery);
                _searchResults.Add(searchResult);
            }
        }

        private static void MergeSearchResults()
        {
            var combinedResults = SearchResultsMergingEnumerable.Create(
                _searchResults.Select(PagedSearchResultEnumerable.Create)
            );

            var searchOutput = new List<IEntityHit>();
            foreach (var entityHit in combinedResults)
            {
                // This line prints each search hit as we retrieve it.
                // The output of the line to console will be interleaved with HTTP requests to search endpoints.
                PrintHit(entityHit);

                searchOutput.Add(entityHit);
            }

            Console.WriteLine();
            PrintFinalSearchResult(searchOutput);
        }

        private static void PrintFinalSearchResult(List<IEntityHit> searchOutput)
        {
            // Here we print search results one more time, but this time as a clean plain list, without unwanted lines.
            Console.WriteLine("The search results output is:");
            foreach (var entityHit in searchOutput)
            {
                PrintHit(entityHit);
            }
        }

        private static void PrintHit(IEntityHit entityHit)
        {
            var file = entityHit as FileHit;
            if (file != null)
            {
                var virtualPath = file.VirtualPath != null ? string.Join("\\", file.VirtualPath) : null;
                var filePathString = virtualPath != null ? $"{virtualPath}\\{file.Name}" : file.Name;
                Console.WriteLine($"File: {filePathString} (Id: {file.Id}, Rank: {file.Rank})");

                return;
            }

            var folder = entityHit as FolderHit;
            if (folder != null)
            {
                var virtualPath = folder.VirtualPath != null ? string.Join("\\", folder.VirtualPath) : null;
                var folderPath = virtualPath != null ? $"{virtualPath}\\{folder.Name}" : folder.Name;
                Console.WriteLine($"Folder: {folderPath} (Id: {folder.Id}, Rank: {folder.Rank})");

                return;
            }
        }
    }
}