using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum EntitySetType : byte
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Entity set is PolicySet
        /// </summary>
        [EnumMember]
        PolicySet = 1,
        /// <summary>
        /// Entity set is StorageSet
        /// </summary>
        [EnumMember]
        StorageSet = 2,
        /// <summary>
        /// Entity set is HomeDirectorySet
        /// </summary>
        [EnumMember]
        HomeDirectorySet = 3
    }
}
