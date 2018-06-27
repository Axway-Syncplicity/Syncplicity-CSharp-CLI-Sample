using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public enum StoragePasswordComplexityPolicy
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Users have no restrictions when creating their password
        /// </summary>
        [EnumMember]
        Disabled = 1,
        /// <summary>
        /// Users are restricted to creating password of a specified minimum complexity
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
