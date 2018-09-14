using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Describes a base URL on which a Search endpoint is available.
    /// </summary>
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "")]
    public class SearchEndpointUrl
    {
        /// <summary>
        /// The base URL
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string Url { get; set; }
    }
}