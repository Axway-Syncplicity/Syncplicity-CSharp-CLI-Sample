using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class PolicySet
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public System.Guid Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name;

        [DataMember(EmitDefaultValue = true, Order = 2)]
        public int Priority;

        [DataMember(EmitDefaultValue = true, Order = 3)]
        public bool IsDefault;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public Policy Policy;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public Group[] Groups;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public EntitySetType EntitySetType;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public Guid CopiedFromSetId;
    }
}
