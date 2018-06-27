using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class StorageQuota
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public System.Guid StorageEndpointId;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int StorageQuotaGb;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public StorageEndpoint StorageEndpoint;
    }
}
