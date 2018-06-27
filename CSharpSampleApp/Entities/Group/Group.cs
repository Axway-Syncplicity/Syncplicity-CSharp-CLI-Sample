using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Group
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public System.Guid Id { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public Company Company { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public User Owner { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public User[] Members { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public StorageQuota[] StorageQuotas { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public PolicySet PolicySet { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public PolicySet[] PolicySets { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public SyncPoint[] Syncpoints { get; set; }

    }
}
