using System;
using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to storageendpoint.svc and storageendpoints.svc
    /// </summary>
    public class StorageEndpointsService : ApiGateway
    {
        #region Static Members

        /// <summary>
        /// Gets url to storage endpoints (plural) service. The service allows to get a list of storage endpoints available to a user.
        /// </summary>
        protected static string StorageEndpointsUrl => StorageAPIUrlPrefix + "storageendpoints.svc/";

        /// <summary>
        /// Gets url to storage endpoint (singular) service. The service allows to get default storage endpoint to a user.
        /// </summary>
        protected static string StorageEndpointUrl => StorageAPIUrlPrefix + "storageendpoint.svc/";

        /// <summary>
        /// Gets param to retrieve storage endpoint or storage endpoint list for a given user.
        /// </summary>
        protected static string ByUserParam => "?user={0}";

        #endregion Static Members

        #region Methods

        /// <summary>
        /// Gets storage endpoints available to the logged in user or user specified in <paramref name="userGuid"/>.
        /// The call with specified <paramref name="userGuid"/> parameter requires administrator permissions.
        /// </summary>
        /// <returns>The array of storage endpoints.</returns>
        public static StorageEndpoint[] GetStorageEndpoints(Guid? userGuid = null)
        {
            string url = StorageEndpointsUrl;
            if (userGuid.HasValue)
            {
                url += string.Format(ByUserParam, userGuid.Value.ToString("D"));
            }
            return HttpGet<StorageEndpoint[]>(url);
        }

        /// <summary>
        /// Gets storage endpoints metadata including search endpoints.
        /// </summary>
        /// <returns>The array of storage endpoints.</returns>
        public static StorageEndpoint[] GetStorageEndpointsWithSearchEndpoints()
        {
            return HttpGet<StorageEndpoint[]>(StorageEndpointsUrl + "?include=search");
        }

        /// <summary>
        /// Get default storage endpoint or the one specified in <paramref name="storageEndpointId"/> parameter.
        /// </summary>
        /// <param name="storageEndpointId">The id of the target storage endpoint.</param>
        /// <returns>The default storage endpoint or storage endpoint identified by <paramref name="storageEndpointId"/>.</returns>
        public static StorageEndpoint GetStorageEndpoint(Guid? storageEndpointId = null)
        {
            var url = StorageEndpointUrl;
            if (storageEndpointId.HasValue)
            {
                url += $"{storageEndpointId:D}";
            }

            return HttpGet<StorageEndpoint>(url);
        }

        /// <summary>
        /// Gets default storage endpoint for the user specified in <paramref name="userGuid"/>.
        /// Requires administrator permissions.
        /// </summary>
        /// <returns>Default storage endpoint of the user.</returns>
        public static StorageEndpoint GetStorageEndpointByUser(Guid userGuid)
        {
            var url = StorageEndpointUrl + string.Format(ByUserParam, userGuid.ToString("D"));
            return HttpGet<StorageEndpoint>(url);
        }

        #endregion Methods
    }
}