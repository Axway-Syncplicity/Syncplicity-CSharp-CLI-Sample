using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Billing period (Monthly, Yearly). 
    /// Please note that enum values DO NOT mean specific number of years, month, days or anything else.
    /// </summary>
    [DataContract(Namespace = "")]
    public enum BillingInterval : short
    {
        /// <summary>
        /// A type wasn't provided.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Monthly = 1,

        [EnumMember]
        Yearly = 2
    }
}