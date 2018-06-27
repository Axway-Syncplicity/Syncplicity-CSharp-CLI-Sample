using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "", Name = "Auth")]
    public class StorageEndpointAuth
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string AccessKey;
    }
}