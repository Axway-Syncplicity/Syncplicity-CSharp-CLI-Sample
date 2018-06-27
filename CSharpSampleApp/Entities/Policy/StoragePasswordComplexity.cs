using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class StoragePasswordComplexity
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public PasswordComplexityOptions Options;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int MinimumLength;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public int? MaximumLength;
    }
}
