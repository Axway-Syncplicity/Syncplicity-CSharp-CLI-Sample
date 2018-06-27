using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Company
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public User Owner;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string Address1;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public string Address2;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public string Address3;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public string City;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public string State;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public string ZipCode;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public byte CountryId;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public string Phone1;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public string Phone2;

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public string Phone3;

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public int Storage;

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public int Seats;

        [DataMember(EmitDefaultValue = false, Order = 15)]
        public SubscriptionPlan SubscriptionPlan;

        [DataMember(EmitDefaultValue = false, Order = 17)]
        public CompanyAccountType AccountType;

        [DataMember(EmitDefaultValue = false, Order = 18)]
        public int SeatsUsed;

        [DataMember(EmitDefaultValue = false, Order = 19)]
        public decimal StorageUsed;

        [DataMember(EmitDefaultValue = false, Order = 23)]
        public DateTime? TrialEndDateUtc;
    }
}