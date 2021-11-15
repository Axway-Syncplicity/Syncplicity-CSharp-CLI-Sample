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

        [DataMember(EmitDefaultValue = false, Name = "shareLinkPolicy", Order = 4)]
        public ShareLinkPolicy SharedLinkPolicy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "passwordProtectedPolicy", Order = 5)]
        public ShareLinkPasswordProtectedPolicy PasswordProtectPolicy { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "shareType", Order = 6)]
        public ShareType ShareType { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "linkPermissionType", Order = 7)]
        public LinkPermissionType LinkPermissionType { get; set; }
    }
}
