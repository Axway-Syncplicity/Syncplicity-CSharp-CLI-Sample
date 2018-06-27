using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum SharingPermission
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        ReadWrite = 1,

        [EnumMember]
        Contributor = 2,

        [EnumMember]
        ReadOnly = 3
    }
}