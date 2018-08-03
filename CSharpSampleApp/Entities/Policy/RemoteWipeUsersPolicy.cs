using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum RemoteWipeUsersPolicy : byte
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Policy is disabled
        /// </summary>
        [EnumMember]
        Disabled = 1,
        /// <summary>
        /// Policy is enabled
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
