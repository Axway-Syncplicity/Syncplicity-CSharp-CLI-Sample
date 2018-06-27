using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Country
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string CountryName;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string ISO2;
    }
}
