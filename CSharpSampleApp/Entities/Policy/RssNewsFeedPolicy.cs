using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum RssNewsFeedPolicy : byte
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Company users will not be able to subscribe to any RSS feeds
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Company users will be able to subscribe to any RSS feeds
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
