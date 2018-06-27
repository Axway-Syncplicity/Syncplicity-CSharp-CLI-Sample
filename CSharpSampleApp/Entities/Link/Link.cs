using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Link
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string Token;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int SyncPointId;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string VirtualPath;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string LandingPageUrl;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public string DownloadUrl;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public string Password;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public string Message;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public File File;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public DateTime SharedDateUtc;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public int NumDownloads;

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public User[] Users;

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public LinkUsage[] LinkUsage;

        [DataMember(EmitDefaultValue = false, Order = 16)]
        public int LinkExpireInDays;

        [DataMember(EmitDefaultValue = false, Order = 19)]
        public string StorageSignature;

        [DataMember(EmitDefaultValue = false, Order = 20)]
        public string DownloadToken;

        [DataMember(EmitDefaultValue = false, Order = 21)]
        public string ThumbnailName;
    }
}