using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class CreditCard
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Address;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string City;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public string Country;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public string StateOrProvince;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public string ZipOrPostalCode;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public string Number;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public string VerificationNumber;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public string ExpirationMonth;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public string ExpirationYear;
    }
}