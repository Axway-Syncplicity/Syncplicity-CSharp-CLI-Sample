using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum BranchingPolicy : byte
    {
        /// <summary>
        /// Default value
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Don't branch any files
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Branch all files
        /// </summary>
        [EnumMember]
        All = 2
    }
}
