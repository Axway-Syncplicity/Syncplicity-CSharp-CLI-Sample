using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Share link expiration option
    /// </summary>
    [DataContract(Namespace = "")]
    public enum ShareLinkExpirationPolicy
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Expiration policy is set on
        /// </summary>
        [EnumMember]
        Enabled = 1,

        /// <summary>
        /// Expiration policy is disabled
        /// </summary>
        [EnumMember]
        Disabled = 2
    }
}
