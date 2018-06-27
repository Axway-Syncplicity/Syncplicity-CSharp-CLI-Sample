using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum FileExclusionPolicy : byte
    {
        /// <summary>
        /// Default policy.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// File Exclusion Policy is disabled.
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Exclusions are applied.
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
