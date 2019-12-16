using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    public class LinkService : ApiGateway
    {
        /// <summary>
        /// Gets url to link service.
        /// </summary>
        protected static string LinkUrl => SyncpointAPIUrlPrefix + "link.svc/";

        /// <summary>
        /// GET /syncpoint/link.svc/{TOKEN}
        /// </summary>
        /// <param name="sharedLinkToken">Shared link token</param>
        /// <returns>Retrieves shared link</returns>
        public static Link GetSharedLink(string sharedLinkToken)
        {
            var url = $"{LinkUrl}{sharedLinkToken}";
            return HttpGet<Link>(url);
        }

        /// <summary>
        /// PUT /syncpoint/link.svc/{TOKEN}
        /// </summary>
        /// <param name="sharedLinkToken">Shared link token</param>
        /// <param name="link">existing shared link</param>
        /// <returns>Returns updated shared link</returns>
        public static Link UpdateSharedLink(string sharedLinkToken, Link link)
        {
            var url = $"{LinkUrl}{sharedLinkToken}";
            var createdLink = HttpPut(url, link);
            return createdLink;
        }

        /// <summary>
        /// DELETE /syncpoint/link.svc/{TOKEN}
        /// </summary>
        /// <param name="sharedLinkToken">Shared link token</param>
        public static void DeleteSharedLink(string sharedLinkToken)
        {
            var url = $"{LinkUrl}{sharedLinkToken}";
            HttpDelete<string>(url);
        }
    }
}
