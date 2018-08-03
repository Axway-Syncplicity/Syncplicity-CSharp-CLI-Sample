using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Mobile change password expiration option
    /// </summary>
    [DataContract(Namespace = "")]
    public enum CachedPasswordExpirationPolicy
    {
        /// <summary>
        /// Default value
        ///</summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Expiration policy is set on and measure interval is in Days
        /// </summary>
        [EnumMember]
        EnabledInDays = 1,
        /// <summary>
        /// Expiration policy is set on and measure interval is in Hours
        /// </summary>
        [EnumMember]
        EnabledInHours = 2,
        /// <summary>
        /// Expiration policy is set on and measure interval is in Minutes
        /// </summary>
        [EnumMember]
        EnabledInMinutes = 3,
        /// <summary>
        /// Expiration policy is disabled
        /// </summary>
        [EnumMember]
        Disabled = 4
    }
}
