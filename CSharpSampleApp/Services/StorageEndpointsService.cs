using System;
using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Class for requests to storageendpoint.svc and storageendpoints.svc
    /// </summary>
    public class StorageEndpointsService : APIGateway
    {
        #region Static Members

        private static string _StorageEndpointsUrl = StorageAPIUrlPrefix + "storageendpoints.svc/";
        private static string _StorageEndpointUrl = StorageAPIUrlPrefix + "storageendpoint.svc/?user={0}";

        /// <summary>
        /// Gets url to storage endpoints service.
        /// </summary>
        protected static string StorageEndpointsUrl
        {
            get { return _StorageEndpointsUrl; }
        }

        /// <summary>
        /// Gets url to storage endpoint service.
        /// </summary>
        protected static string StorageEndpointUrl
        {
            get { return _StorageEndpointUrl; }
        }

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
        /// Gets storage endpoint for the user.
        /// </summary>
        /// <returns>The storage endpoint of the user.</returns>
        public static StorageEndpoint GetStorageEndpoint(Guid userId)
        {
            return HttpGet<StorageEndpoint>(String.Format(StorageEndpointUrl, userId));
        }

        #endregion Methods
    }
}