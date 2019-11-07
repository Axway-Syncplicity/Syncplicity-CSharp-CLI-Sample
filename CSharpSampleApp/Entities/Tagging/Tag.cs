using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities.Tagging
{
    [DataContract(Namespace = "")]
    public class Tag
    {
        [DataMember(EmitDefaultValue = false, Name ="type", Order = 0)]
        public string Type { get; set; }

        [DataMember(EmitDefaultValue = false, Name ="name", Order = 1)]
        public string Name { get; set; }
    }
}
