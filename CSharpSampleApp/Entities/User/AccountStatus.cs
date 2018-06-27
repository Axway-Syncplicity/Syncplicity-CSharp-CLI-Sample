using System.ComponentModel;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// User Account Status.
    /// </summary>
    [DataContract(Namespace = "")]
    public enum AccountStatus : byte
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Disabled = 1,

        [EnumMember]
        Enabled = 2,

        [EnumMember]
        Delinquent = 3,

        [EnumMember]
        [Description("Pending Activation")]
        PendingActivation = 4,

        [EnumMember]
        Unverified = 5,

        [EnumMember]
        Suspended = 6
    }
}
