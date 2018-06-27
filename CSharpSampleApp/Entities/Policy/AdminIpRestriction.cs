using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class AdminIpRestriction
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string AllowedIPs;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string Message;
    }
}
