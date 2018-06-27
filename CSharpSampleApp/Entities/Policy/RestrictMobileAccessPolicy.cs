using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum RestrictMobileAccessPolicy : byte
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
        Enabled = 2,

        /// <summary>
        /// Allow only Mdm devices
        /// </summary>
        [EnumMember]
        MdmOnly = 3
    }
}
