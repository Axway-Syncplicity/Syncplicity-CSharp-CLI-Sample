using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum QuotaAllocationType : byte
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        GroupQuota = 1,

        [EnumMember]
        StorageVaultQuota = 2
    }
}
