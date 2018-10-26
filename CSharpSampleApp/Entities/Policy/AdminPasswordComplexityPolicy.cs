using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum AdminPasswordComplexityPolicy
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Company administrators have no restrictions when creating their password
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Company administrators are restricted to creating password of a specified minimum complexity
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}
