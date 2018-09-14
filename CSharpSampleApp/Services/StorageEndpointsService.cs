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
        /// Gets url to storage endpoints service.
        /// </summary>
        protected static string StorageEndpointsUrl => StorageAPIUrlPrefix + "storageendpoints.svc/";

        /// <summary>
        /// Gets url to retrieve storage endpoint for a given user.
        /// </summary>
        protected static string StorageEndpointByUserUrl => StorageAPIUrlPrefix + "storageendpoint.svc/?user={0}";

        #endregion Static Members

        #region Methods

        /// <summary>
        /// Gets storage endpoints.
        /// </summary>
        /// <returns>The array of storage endpoints.</returns>
        public static StorageEndpoint[] GetStorageEndpoints()
        {
            return HttpGet<StorageEndpoint[]>(StorageEndpointsUrl);
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
        /// Get storage endpoint by id.
        /// </summary>
        /// <param name="storageEndpointId">The id of the target storage endpoint.</param>
        /// <returns>The storage endpoint identified by <paramref name="storageEndpointId"/>.</returns>
        public static StorageEndpoint GetStorageEndpoint(Guid storageEndpointId)
        {
            var url = StorageAPIUrlPrefix + $"storageendpoint.svc/{storageEndpointId:D}";
            return HttpGet<StorageEndpoint>(url);
        }

        /// <summary>
        /// Gets storage endpoint for the user.
        /// </summary>
        /// <returns>The storage endpoint of the user.</returns>
        public static StorageEndpoint GetStorageEndpointByUser(Guid userId)
        {
            return HttpGet<StorageEndpoint>(string.Format(StorageEndpointByUserUrl, userId));
        }

        #endregion Methods
    }
}