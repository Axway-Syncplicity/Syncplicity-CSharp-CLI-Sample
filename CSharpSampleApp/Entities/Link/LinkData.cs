using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class LinkData
    {
        [DataMember(EmitDefaultValue = false, Name = "email", Order = 0)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "oldMessage", Order = 1)]
        public string OldMessage { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "newMessage", Order = 2)]
        public string NewMessage { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "fileName", Order = 3)]
        public string FileName { get; set; }
    }
}
