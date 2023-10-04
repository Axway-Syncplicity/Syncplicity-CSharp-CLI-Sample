using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum SyncPointType : byte
    {
        [EnumMember]
        MyDocuments = 1,

        [EnumMember]
        MyMusic = 2,

        [EnumMember]
        MyPictures = 3,

        [EnumMember]
        Desktop = 4,

        [EnumMember]
        Favorites = 5,

        [EnumMember]
        Custom = 6,

        [EnumMember]
        SyncplicityDrive = 8
    }
}