using CSharpSampleApp.Entities.Events;
using System.Collections.Generic;

namespace CSharpSampleApp.Services
{
    public class EventsService : ApiGateway
    {
        /// <summary>
        /// News feed events
        /// </summary>
        protected static string EventsUrl => GolGateway.BaseApiEndpointUrl + "/events/company";

        /// <summary>
        /// Retrieves all events related to user.
        /// </summary>
        /// <returns>Collection of news feed events if any</returns>
        public static IEnumerable<NewsFeedEvent> GetNewsFeedEventsForUser()
        {
            return HttpGet<List<NewsFeedEvent>>(EventsUrl);
        }
    }
}
