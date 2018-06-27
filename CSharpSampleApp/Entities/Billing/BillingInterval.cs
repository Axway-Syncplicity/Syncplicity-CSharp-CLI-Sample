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
        // A type wasn't provided.
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Monthly = 1,

        [EnumMember]
        Yearly = 2
    }
}