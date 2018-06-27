namespace CSharpSampleApp.Entities
{
#if !DOT_NET_2_0
    using System;
    using System.Runtime.Serialization;
#endif

    [DataContract(Namespace = "")]
    public class Machine
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public System.Guid Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name;
    }
}
