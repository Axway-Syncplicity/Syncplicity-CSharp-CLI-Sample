using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Quota
    {
        [DataMember(EmitDefaultValue = true, Order = 0)]
        public long ActiveBytes;

        [DataMember(EmitDefaultValue = true, Order = 1)]
        public long AvailableBytes;

        [DataMember(EmitDefaultValue = true, Order = 2)]
        public Guid StorageEndpointId;

        [DataMember(EmitDefaultValue = true, Order = 3)]
        public long PreviousVersionBytes;

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public long DeletedBytes;

        [DataMember(EmitDefaultValue = true, Order = 5)]
        public Guid UserId;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public QuotaOwnership QuotaOwnership;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public QuotaAllocationType QuotaAllocationType;

        [DataMember(EmitDefaultValue = true, Order = 8)]
        public long StorageCapacityInBytes;
    }
}