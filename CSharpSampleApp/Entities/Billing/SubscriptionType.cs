using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// User Account Type
    /// </summary>
    [DataContract(Namespace = "")]
    public enum SubscriptionType : byte
    {
        // A type wasn't provided.
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Subscription type includes Free and Paid Personal subscriptions for now
        /// </summary>
        [EnumMember]
        PersonalEdition = 1,

        /// <summary>
        /// Subscription type includes BT, Paid Business
        /// </summary>
        [EnumMember]
        BusinessEdition = 2,

        /// <summary>
        /// Subscription type includes ET, Paid Enterprise
        /// </summary>
        [EnumMember]
        EnterpriseEdition = 3,

        [EnumMember]
        DepartmentEdition = 4
    }
}