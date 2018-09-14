using System;
using System.Collections.Generic;
using CSharpSampleApp.Services.Search;

namespace CSharpSampleApp.Examples.Search
{
    /// <summary>
    /// Represents a logical pipeline that joins search result pages into a sequence of hits ordered by Rank
    /// </summary>
    internal class PagedSearchResultEnumerable
    {
        public static IEnumerable<IEntityHit> Create(SearchResult searchResult)
        {
            var currentPage = searchResult.FirstPage;
            do
            {
                Console.WriteLine("Paging: outputting current page");
                foreach (var hit in currentPage.OrderedHits)
                {
                    yield return hit;
                }

                if (searchResult.HasMorePages(currentPage))
                {
                    Console.WriteLine("Paging: Fetching next page");
                    currentPage = searchResult.GetNextPage(currentPage);
                }
                else
                {
                    currentPage = null;
                }

            } while (currentPage != null);
        }
    }
}
