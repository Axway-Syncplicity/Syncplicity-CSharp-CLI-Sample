using CSharpSampleApp.Entities;
using System.Collections.Generic;

namespace CSharpSampleApp.Services
{
    public class LinksService : ApiGateway
    {
        /// <summary>
        /// Gets url to links service.
        /// </summary>
        protected static string LinksUrl => SyncpointAPIUrlPrefix + "links.svc/";

        /// <summary>
        /// POST syncpoint/links.svc/ (creates shared links)
        /// </summary>
        /// <param name="links">The array of new links.</param>
        /// <returns>Collection of newly created shared links.</returns>
        public static IEnumerable<Link> CreateSharedLinks(Link[] links)
        {
            var createdLinks = HttpPost(LinksUrl, links);
            return createdLinks;
        }

        /// <summary>
        /// GET syncpoint/links.svc/  (retrieves shared links)
        /// </summary>
        /// <returns>Collection of shared links.</returns>
        public static IEnumerable<Link> GetSharedLinks()
        {
            var links = HttpGet<List<Link>>(LinksUrl);
            return links;
        }
    }
}
