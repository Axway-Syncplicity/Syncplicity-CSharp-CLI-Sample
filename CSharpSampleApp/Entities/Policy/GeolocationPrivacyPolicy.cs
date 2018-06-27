using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum GeolocationPrivacyPolicy : byte
    {
        [EnumMember] 
        Unknown = 0,

        /// <summary>
        /// Disallow geo-location tracking
        /// </summary>
        [EnumMember]
        Disallow = 1,

        /// <summary>
        /// Allow the ability to track geo-location information
        /// </summary>
        [EnumMember]
        Allow = 2
    }
}
