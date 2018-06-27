using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class HomeDirectoryPolicy
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public Guid ConnectorId;
        
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string ConnectorUrl;

    }
}
