using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "")]
    public class Mapping
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public long SyncPointId;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public Machine Machine { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Path;

        [DataMember(EmitDefaultValue = true, Order = 3)]
        public bool Mapped;

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public bool DownloadEnabled;

        [DataMember(EmitDefaultValue = true, Order = 5)]
        public bool UploadEnabled;
    }
}