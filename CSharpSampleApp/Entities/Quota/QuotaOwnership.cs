using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum QuotaOwnership : byte
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Shared = 1,

        [EnumMember]
        Owned = 2
    }
}
