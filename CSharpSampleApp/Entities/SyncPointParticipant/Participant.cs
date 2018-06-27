using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Participant
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public int SyncPointId;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public User User;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public User Inviter;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public SharingPermission Permission;

        [DataMember(EmitDefaultValue = true, Order = 5)]
        public bool Mapped;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public string SharingInviteNote;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public ShareWithNewParticipantOption NewParticipantOption;
    }
}