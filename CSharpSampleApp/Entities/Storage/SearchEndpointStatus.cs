using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    public enum SearchEndpointStatus
    {
        [EnumMember]
        Disabled = 0,

        [EnumMember]
        Enabled = 1
    }
}