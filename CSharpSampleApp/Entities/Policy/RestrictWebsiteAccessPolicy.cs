using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum RestrictWebsiteAccessPolicy : byte
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Restriction policy is set off
        /// </summary>
        [EnumMember]
        Disabled = 1,
        /// <summary>
        /// Restriction policy is enabled
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
