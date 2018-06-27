using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class BillingInfo
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public PaymentMethod PaymentMethod;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public CreditCard CreditCard;
    }
}