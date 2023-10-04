using CSharpSampleApp.Entities;
using System;
using System.Linq;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Syncpoints service class.
    /// </summary>
    public class SyncPointsService : ApiGateway
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

        /// <summary>
        /// Gets url to syncpoints service.
        /// </summary>
        protected static string SyncpointsUrl => SyncpointAPIUrlPrefix + "syncpoints.svc/";

        /// <summary>
        /// Gets url to syncpoint service.
        /// </summary>
        protected static string SyncpointUrl => SyncpointAPIUrlPrefix + "syncpoint.svc/{0}";

        /// <summary>
        /// Gets url to syncpoint participants service.
        /// </summary>
        protected static string SyncPointParticipants => SyncpointAPIUrlPrefix + "syncpoint_participants.svc/{0}/participants";

        #endregion Static Members

        #region Public Methods

        /// <summary>
        /// Gets syncpoints.
        /// </summary>
        /// <param name="include">The include param.</param>
        /// <param name="includeType">If types are not passed, all types except SyncplicityDrive will be returned.</param>
        /// <returns>The array of syncpoints.</returns>
        public static SyncPoint[] GetSyncpoints(Include include = Include.None, params SyncPointType[] includeType)
        {
            var includeStr = FormatIncludeParameter(include);
            var includeTypeStr = FormatIncludeTypeParameter(includeType);
            var uri = SyncpointsUrl;
            uri = AppendQueryParam(includeStr, uri, includeTypeStr);

            return HttpGet<SyncPoint[]>(uri);
        }

        /// <summary>
        /// Gets syncpoint by Id.
        /// </summary>
        /// <param name="syncpointId">The Id of syncpoint.</param>
        /// <param name="include">The include param.</param>
        /// <returns>The syncpoint object.</returns>
        public static SyncPoint GetSyncpoint(long syncpointId, Include include = Include.None)
        {
            var url = string.Format(SyncpointUrl, syncpointId);

            var includeStr = FormatIncludeParameter(include);
            if (!string.IsNullOrWhiteSpace(includeStr))
                url += $"?include={includeStr}";

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
            var url = string.Format(SyncpointUrl, syncpoint.Id);

            var includeStr = FormatIncludeParameter(include);
            if (!string.IsNullOrWhiteSpace(includeStr))
                url += $"?include={includeStr}";

            return HttpPut(url, syncpoint);
        }

        public static void DeleteSyncpoint(long syncpointId)
        {
            var url = string.Format(SyncpointUrl, syncpointId);

            HttpDelete<object>(url);
        }

        #endregion Public Methods

        #region Private Methods

        private static string FormatIncludeParameter(Include include)
        {
            if (include == Include.None)
            {
                return string.Empty;
            }

            return include.ToString().Replace(" ", "").ToLower();
        }

        private static string AppendQueryParam(string includeStr, string uri, string includeTypeStr)
        {
            if (!string.IsNullOrEmpty(includeStr))
            {
                uri = string.Format("{0}?include={1}", uri, includeStr);
            }
            if (!string.IsNullOrEmpty(includeTypeStr))
            {
                uri = string.Format("{0}{1}includeType={2}", uri, uri.Contains('?') ? "&" : "?", includeTypeStr);
            }
            return uri;
        }

        private static string FormatIncludeTypeParameter(params SyncPointType[] includeType)
        {
            if (includeType == null)
            {
                return string.Empty;
            }

            return string.Join(",", includeType.Select(i => (int)i));
        }

        #endregion
    }
}