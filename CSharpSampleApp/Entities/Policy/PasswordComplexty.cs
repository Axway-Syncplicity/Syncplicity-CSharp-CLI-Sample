using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract]
    public class PasswordComplexty
    {
        [DataMember]
        public PasswordComplexityOptions Options;

        [DataMember]
        public string[] RestrictedPasswords;

        [DataMember]
        public int? MinimumLength;

        [DataMember]
        public int? MaximumLength;
    }
}
