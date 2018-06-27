using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract]
    public class PolicyPublic
    {
        [DataMember]
        public PasswordComplexty PasswordComplexity;
    }
}
