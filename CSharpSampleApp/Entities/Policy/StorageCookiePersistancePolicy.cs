using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public enum StorageCookiePersistancePolicy
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Users can not persist storage cookie 
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Users can persist storage cookie
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
