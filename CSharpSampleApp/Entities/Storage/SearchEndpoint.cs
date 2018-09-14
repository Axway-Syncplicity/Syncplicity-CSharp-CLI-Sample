using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Describes a Search endpoint
    /// </summary>
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "")]
    public class SearchEndpoint
    {
        /// <summary>
        /// The endpoint unique Id
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid Id;

        /// <summary>
        /// The endpoint status.
        /// </summary>
        [DataMember(EmitDefaultValue = true, Order = 1)]
        public SearchEndpointStatus Status;

        /// <summary>
        /// The endpoint type.
        /// </summary>
        [DataMember(EmitDefaultValue = true, Order = 2)]
        public SearchEndpointType Type;

        /// <summary>
        /// The available base URLs of the endpoint.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public SearchEndpointUrl[] Urls;
    }
}