using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum LinkPermissionType : byte
    {
        /// <summary>
        /// Default link permission type.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Link is with read-only rights.
        /// </summary>
        [EnumMember]
        Read = 1,

        /// <summary>
        /// Link is with contribute rights, downlaod should not be allowed.
        /// </summary>
        [EnumMember]
        Contribute = 2,

        /// <summary>
        /// Link is with edit rights.
        /// </summary>
        [EnumMember]
        Edit = 3,
    }
}
