using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum FolderStatus : byte
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Added = 1,

        [EnumMember]
        Removed = 4,

        [EnumMember]
        ConfirmedRemoved = 5
    }
}
