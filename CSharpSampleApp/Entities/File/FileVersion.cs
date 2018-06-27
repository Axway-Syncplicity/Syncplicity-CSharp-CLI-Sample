using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class FileVersion
    {
        [DataMember(EmitDefaultValue = true, Order = 0)]
        public long SyncpointId;

        [DataMember(EmitDefaultValue = true, Order = 1)]
        public long Id;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string UserName;

        [DataMember(EmitDefaultValue = true, Order = 3)]
        public string DataSourceName;

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public int Action;

        [DataMember(EmitDefaultValue = true, Order = 5)]
        public long Length;

        [DataMember(EmitDefaultValue = true, Order = 6)]
        public string RevisionAge;
    }
}