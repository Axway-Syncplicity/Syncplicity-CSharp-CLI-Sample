using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [System.Xml.Serialization.XmlRoot(Namespace = "")]
    [DataContract(Namespace = "")]
    public class SyncPoint
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public long Id;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public SyncPointType Type;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Name;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public long RootFolderId;

        [DataMember(EmitDefaultValue = true, Order = 4)]
        public bool Mapped;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public string Path;

        [DataMember(EmitDefaultValue = true, Order = 6)]
        public bool DownloadEnabled;

        [DataMember(EmitDefaultValue = true, Order = 7)]
        public bool UploadEnabled;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public bool Shared;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public User Owner;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public SharingPermission Permission;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public Participant[] Participants;

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public Mapping[] Mappings;

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public SyncPointPolicy Policy;

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public bool RemoteWipe;

        [DataMember(EmitDefaultValue = false, Order = 15)]
        public System.Guid StorageEndpointId;

        [DataMember(EmitDefaultValue = false, Order = 16)]
        public SyncPoint Parent;

        [DataMember(EmitDefaultValue = false, Order = 17)]
        public SyncPoint[] Children;

        [DataMember(EmitDefaultValue = false, Order = 18)]
        public string PathToRoot;

        [DataMember(EmitDefaultValue = false, Order = 19)]
        public User Inviter;

        [DataMember(EmitDefaultValue = false, Order = 20)]
        public int ServerId;

        [DataMember(EmitDefaultValue = false, Order = 21)]
        public Server.Status ServerStatus;

        public override string ToString()
        {
            return string.Format("Syncpoint: Name: '{0}', Id: {1}", Name, Id);
        }
    }
}