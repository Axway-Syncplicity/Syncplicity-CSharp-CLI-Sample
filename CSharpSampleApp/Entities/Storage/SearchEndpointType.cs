using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum SearchEndpointType : byte
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        MetadataOnly = 2,

        [EnumMember]
        ContentAndMetadata = 3
    }
}