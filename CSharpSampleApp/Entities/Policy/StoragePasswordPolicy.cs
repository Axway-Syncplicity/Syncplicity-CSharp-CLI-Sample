using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public enum StoragePasswordPolicy
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,
        /// <summary>
        /// Users can not create storage password 
        /// </summary>
        [EnumMember]
        Disabled = 1,
        /// <summary>
        /// Users can create storage password 
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
