using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "")]
    public class StorageEndpoint
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Description;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public Guid CompanyId;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public bool Active;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public StorageEndpointUrl[] Urls;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public StorageEndpointAuth[] Auths;

        [DataMember(EmitDefaultValue = true, Order = 7)]
        public int Version;

        [DataMember(EmitDefaultValue = true, Order = 8)]
        public int SizeGb;

        [DataMember(EmitDefaultValue = true, Order = 9)]
        public bool RequiresStorageAuthentication;

        [DataMember(EmitDefaultValue = true, Order = 10)]
        public bool UserHasStoragePassword;

        [DataMember(EmitDefaultValue = true, Order = 11)]
        public Company Company;

        [DataMember(EmitDefaultValue = true, Order = 13)]
        public bool Default;

        [DataMember(EmitDefaultValue = true, Order = 15)]
        public decimal ConsumedGb;
    }
}