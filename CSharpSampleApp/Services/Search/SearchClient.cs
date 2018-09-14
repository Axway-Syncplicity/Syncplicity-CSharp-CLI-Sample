using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using CSharpSampleApp.Entities;
using CSharpSampleApp.Util;
using JsonPrettyPrinterPlus;
using Newtonsoft.Json;

namespace CSharpSampleApp.Services.Search
{
    internal class SearchClient
    {
        private const int ResponseTimeoutMilliseconds = 60 * 60 * 1000;
        private const int DefaultPageSize = 5;

        private readonly SearchEndpoint _searchEndpoint;

        private string _sessionKey;

        public SearchClient(SearchEndpoint searchEndpoint)
        {
            _searchEndpoint = searchEndpoint;
        }

        public SearchResult Search(string searchQuery = null)
        {
            Initialize();

            var searchParameters = new SearchParameters(searchQuery, DefaultPageSize, 0);
            return ExecuteSearchRequest(searchParameters);
        }

        private SearchResult ExecuteSearchRequest(SearchParameters searchParameters)
        {
            var request = CreateRequest(searchParameters);
            var response = ReadResponse<SearchResponse>(request);
            return CreateResult(response, searchParameters);
        }

        private SearchResult CreateResult(SearchResponse response, SearchParameters searchParameters)
        {
            var firstPage = CreateSearchResultPage(response, searchParameters.SkipRecords, searchParameters.PageSize);
            return new SearchResult(searchParameters, response.TotalResultsCount, firstPage, this);
        }

        private SearchResultPage CreateSearchResultPage(SearchResponse response, int skip, int pageSize)
        {
            return new SearchResultPage(skip, skip + pageSize - 1, response.Files, response.Folders);
        }

        private HttpWebRequest CreateRequest(SearchParameters parameters)
        {
            var searchUrl = BuildSearchUrl(parameters);
            Console.WriteLine("Search Url: {0}", searchUrl);

            var request = WebRequest.CreateHttp(searchUrl);

            request.Timeout = ResponseTimeoutMilliseconds;
            request.KeepAlive = true;
            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "Syncplicity Client";
            request.Headers.Add(HttpRequestHeader.Authorization, _sessionKey);

            return request;
        }

        private string BuildSearchUrl(SearchParameters parameters)
        {
            var searchQuery = parameters.Query;

            var queryParameters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(searchQuery)) queryParameters.Add("q", searchQuery);
            queryParameters.Add("skip", parameters.SkipRecords.ToString("D", CultureInfo.InvariantCulture));
            queryParameters.Add("take", parameters.PageSize.ToString("D", CultureInfo.InvariantCulture));

            var queryChunks = queryParameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}");
            var query = string.Join("&", queryChunks);

            var searchUrlToUse = _searchEndpoint.Urls[0].Url;
            var searchUrl = $"{searchUrlToUse}search";

            if (!string.IsNullOrEmpty(query)) searchUrl = searchUrl + "?" + query;

            return searchUrl;
        }

        private static T ReadResponse<T>(WebRequest request)
        {
            try
            {
                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        Console.WriteLine("Response wasn't received.");
                        return default(T);
                    }

                    return DeserializeResponseBody<T>(responseStream);
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse) e.Response;
                if (response == null)
                {
                    Console.WriteLine("Response not received.");
                    return default(T);
                }

                OutputErrorToConsole(response, e);
                throw;
            }
        }

        private static T DeserializeResponseBody<T>(Stream responseStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                responseStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var response = Encoding.UTF8.GetString(memoryStream.ToArray());
                Console.WriteLine("[Response] ");

                var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
                Console.WriteLine(pp.PrettyPrint(response));

                if (typeof(T) == typeof(string)) return (T) (object) response;

                memoryStream.Position = 0;

                using (var reader = new StreamReader(memoryStream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return JsonSerializer.Create(new JsonSerializerSettings())
                        .Deserialize<T>(jsonReader);
                }

            }
        }

        private static void OutputErrorToConsole(HttpWebResponse response, WebException e)
        {
            Console.WriteLine(
                $"Error {(int) response.StatusCode} {response.StatusDescription} occurs during request to {response.ResponseUri}.");

            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    using (var memStream = new MemoryStream())
                    {
                        stream.CopyTo(memStream);
                        memStream.Position = 0;
                        Console.WriteLine("Response body:");
                        Console.WriteLine(Encoding.UTF8.GetString(memStream.ToArray()));
                    }
                }
            }

            Console.WriteLine("WebException:");
            Console.WriteLine(e);
        }

        private void Initialize()
        {
            var token = ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                ? ApiContext.MachineToken
                : ApiContext.AccessToken;
            _sessionKey = $"Bearer {token}";

            Console.WriteLine("Session Key: {0}", _sessionKey);
        }

        internal SearchResultPage GetNextPage(SearchResult searchResult, SearchResultPage thisPage)
        {
            var skip = thisPage.From + searchResult.Parameters.PageSize;
            var nextPageSearchParameters = new SearchParameters(searchResult.Parameters.Query, searchResult.Parameters.PageSize, skip);
            var nextPageSearchResult = ExecuteSearchRequest(nextPageSearchParameters);

            return nextPageSearchResult.FirstPage;
        }
    }

    internal class SearchParameters
    {
        public string Query { get; }
        public int PageSize { get; }
        public int SkipRecords { get; }

        public SearchParameters(string query, int pageSize, int skipRecords)
        {
            Query = query;
            PageSize = pageSize;
            SkipRecords = skipRecords;
        }
    }

    internal class SearchResult
    {
        private readonly SearchClient _client;

        public SearchParameters Parameters { get; }

        public long TotalResultsCount { get; }

        public SearchResultPage FirstPage { get; }

        public SearchResult(SearchParameters parameters, long totalResultsCount, SearchResultPage firstPage, SearchClient client)
        {
            _client = client;
            Parameters = parameters;
            TotalResultsCount = totalResultsCount;
            FirstPage = firstPage;
        }

        public SearchResultPage GetNextPage(SearchResultPage thisPage)
        {
            return _client.GetNextPage(this, thisPage);
        }

        public bool HasMorePages(SearchResultPage page)
        {
            return TotalResultsCount > page.To + 1;
        }
    }

    internal class SearchResultPage
    {
        public int From { get; }

        public int To { get; }

        public ICollection<FileHit> Files { get; }

        public ICollection<FolderHit> Folders { get; }

        public IEnumerable<IEntityHit> Hits => Files.Cast<IEntityHit>().Concat(Folders);

        public IOrderedEnumerable<IEntityHit> OrderedHits => Hits.OrderByDescending(hit => hit.Rank);

        public SearchResultPage(int from, int to, ICollection<FileHit> files, ICollection<FolderHit> folders)
        {
            From = @from;
            To = to;
            Files = files;
            Folders = folders;
        }
    }
}
