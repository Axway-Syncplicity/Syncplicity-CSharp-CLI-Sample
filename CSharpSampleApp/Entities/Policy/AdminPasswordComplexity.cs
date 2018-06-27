using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class AdminPasswordComplexity
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public PasswordComplexityOptions AdminPasswordComplexityOptions;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int AdminPasswordComplexityMinimumLength;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int? AdminPasswordComplexityMaximumLength;
    }
}
