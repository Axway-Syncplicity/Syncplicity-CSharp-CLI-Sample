using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum ConcurrentUseReactionModePolicy : byte
    {
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Default value.
        /// Do not disable the user’s account when concurrent use of a device from multiple sessions is detected.
        /// </summary>
        [EnumMember]
        Disable = 1,

        /// <summary>
        /// Disable the user’s account when concurrent use of a device from multiple sessions is detected.
        /// </summary>
        [EnumMember]
        Enable = 2
    }
}
