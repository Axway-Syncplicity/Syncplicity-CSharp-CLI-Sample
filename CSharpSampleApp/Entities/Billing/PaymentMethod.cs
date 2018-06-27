using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum PaymentMethod : short
    {
        // A type wasn't provided.
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        CreditCard = 1,

        [EnumMember]
        PurchaseOrder = 2,

        [EnumMember]
        DebitCard = 3
    }
}