using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Machine
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name;
    }
}
