using System;
using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Syncpoints service class.
    /// </summary>
    public class SyncPointsService : APIGateway
    {
        /// <summary>
        /// The includes enum.
        /// </summary>
        [Flags]
        public enum Include : byte
        {
            None = 0,
            Participants = 1,
            Inviter = 2,
            RemoteWipe = 4,
            Children = 8
        }

        #region Static Members

        private static string _SyncpointsUrl = SyncpointAPIUrlPrefix + "syncpoints.svc/";
        private static string _SyncpointUrl = SyncpointAPIUrlPrefix + "syncpoint.svc/{0}";
        private static string _LinksUrl = SyncpointAPIUrlPrefix + "links.svc/";
        private static string _SyncPointParticipants = SyncpointAPIUrlPrefix + "syncpoint_participants.svc/{0}/participants";

        /// <summary>
        /// Gets url to syncpoints service.
        /// </summary>
        protected static string SyncpointsUrl
        {
            get { return _SyncpointsUrl; }
        }

        /// <summary>
        /// Gets url to syncpoint service.
        /// </summary>
        protected static string SyncpointUrl
        {
            get { return _SyncpointUrl; }
        }

        /// <summary>
        /// Gets url to links service.
        /// </summary>
        protected static string LinksUrl
        {
            get { return _LinksUrl; }
        }

        /// <summary>
        /// Gets url to syncpoint participants service.
        /// </summary>
        protected static string SyncPointParticipants
        {
            get { return _SyncPointParticipants; }
        }

        #endregion Static Members

        #region Public Methods

        /// <summary>
        /// Gets synpoints.
        /// </summary>
        /// <param name="include">The include param.</param>
        /// <returns>The array of syncpoints.</returns>
        public static SyncPoint[] GetSyncpoints(Include include = Include.None)
        {
            string includeStr = FormatInclude(include);

            string url = string.IsNullOrWhiteSpace(includeStr)
                ? SyncpointsUrl
                : SyncpointsUrl + string.Format("?include={0}", includeStr);

            return HttpGet<SyncPoint[]>(url);
        }

        /// <summary>
        /// Gets syncpoint by Id.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="include">The include param.</param>
        /// <returns>The syncpoint object.</returns>
        public static SyncPoint GetSyncpoint(long syncpointId, Include include = Include.None)
        {
            string url = string.Format(SyncpointUrl, syncpointId);

            string includeStr = FormatInclude(include);
            if (!string.IsNullOrWhiteSpace(includeStr))
                url += string.Format("?include={0}", includeStr);

            return HttpGet<SyncPoint>(url);
        }

        /// <summary>
        /// Creates syncpoints.
        /// </summary>
        /// <param name="syncPoints">The array of new syncpoints.</param>
        /// <returns>The array of newly created syncpoints.</returns>
        public static SyncPoint[] CreateSyncpoints(SyncPoint[] syncPoints)
        {
            return HttpPost(SyncpointsUrl, syncPoints);
        }

        /// <summary>
        /// Creates sahred link..
        /// </summary>
        /// <param name="links">The array of new links.</param>
        /// <returns>The array of newly created links.</returns>
        public static Link[] CreateLinks(Link[] links)
        {
            return HttpPost(LinksUrl, links);
        }

        /// <summary>
        /// Creates new participants for syncpoint Id.
        /// </summary>
        /// <param name="syncpointId">The syncpoint Id.</param>
        /// <param name="participants">The array of new participants.</param>
        /// <returns>The array of newly created participants.</returns>
        public static Participant[] PostSyncPointParticipants(long syncpointId, Participant[] participants)
        {
            return HttpPost(string.Format(SyncPointParticipants, syncpointId), participants);
        }

        /// <summary>
        /// Updates a syncpoint.
        /// </summary>
        /// <param name="syncpoint">The syncpoint to update.</param>
        /// <param name="include">The include param.</param>
        /// <returns>The object of updates syncpoint.</returns>
        public static SyncPoint PutSyncpoint(SyncPoint syncpoint, Include include = Include.None)
        {
            string url = string.Format(SyncpointUrl, syncpoint.Id);

            string includeStr = FormatInclude(include);
            if (!string.IsNullOrWhiteSpace(includeStr))
                url += string.Format("?include={0}", includeStr);

            return HttpPut(url, syncpoint);
        }

        public static void DeleteSyncpoint(long syncpointId)
		{
			string url = string.Format(SyncpointUrl, syncpointId);

			HttpDelete<object>(url);
		}

        #endregion Public Methods

        #region Private Methods

        private static string FormatInclude(Include include)
        {
            if (include == Include.None)
                return string.Empty;

            return include.ToString().Replace(" ", "").ToLower();
        }

        #endregion
    }
}