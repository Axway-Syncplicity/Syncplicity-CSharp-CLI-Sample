using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class SubscriptionPlan
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public SubscriptionType Type;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public bool? PremiumSupport;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public BillingInterval BillingInterval;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string PromotionalCode;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public BillingInfo BillingInfo;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public DateTime? RenewalDateUtc;
    }
}