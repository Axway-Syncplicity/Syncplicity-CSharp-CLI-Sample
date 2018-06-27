using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum EnforceClientMemberActiveDirectoryPolicy : byte
    {
        /// <summary>
        /// Default policy.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Policy is disabled.
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// All clients on platforms like Windows must be members of an active directory.
        /// </summary>
        [EnumMember]
        Liberal = 2,

        /// <summary>
        /// All clients, even those on platforms like OSX that don’t support active directory, 
        /// must be joined to a known active directory.
        /// </summary>
        [EnumMember]
        Strict = 3
    }
}
