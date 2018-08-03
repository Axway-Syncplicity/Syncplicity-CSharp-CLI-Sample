using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// User Account Type
    /// </summary>
    [DataContract(Namespace = "")]
    public enum AccountType : byte
    {
        /// <summary>
        /// A type wasn't provided.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Free users have a limited storage for their own files.
        /// Shared files do not count against their quota if they're not the Virtual Folder owner.
        /// </summary>
        [EnumMember]
        Free = 1,

        /// <summary>
        /// An individual who is subscribed to Syncplicity.
        /// </summary>
        [EnumMember]
        PaidIndividual = 2,

        /// <summary>
        /// A user who is subscribed to Syncplicity as part of a business account.
        /// </summary>
        [EnumMember]
        PaidBusiness = 3,

        /// <summary>
        /// People who accepted an invite but have not yet provided their name and password.
        /// </summary>
        [EnumMember]
        LimitedFree = 6,

        /// <summary>
        /// Users who have been added to a company account but not yet provided their personal details
        /// </summary>
        [EnumMember]
        LimitedBusiness = 7,

        /// <summary>
        /// A user who is subscribed to Syncplicity as part of a personal account.
        /// </summary>
        [EnumMember]
        PaidPersonal = 13,

        /// <summary>
        /// A user who is subscribed to Syncplicity as part of a business trial account
        /// and hasn't previous subscription
        /// </summary>
        [EnumMember]
        TrialBusiness = 14,

        /// <summary>
        /// Reseller
        /// </summary>
        [EnumMember]
        Reseller = 15,

        /// <summary>
        /// A user who is suggested to be added to a company account, but not yet approved by a company administrator 
        /// </summary>
        [EnumMember]
        PendingBusiness = 16,
    }
}