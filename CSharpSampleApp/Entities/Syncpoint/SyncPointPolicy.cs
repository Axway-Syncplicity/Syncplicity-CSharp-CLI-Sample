using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class SyncPointPolicy
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public RemoteWipeSyncpointPolicy RemoteWipeSyncpointPolicy = RemoteWipeSyncpointPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public SharedFolderResharingPolicy ResharingPolicy = SharedFolderResharingPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public ShareLinkPolicy ShareLinkPolicy = ShareLinkPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public ShareLinkPasswordProtectedPolicy ShareLinkPasswordProtectedPolicy = ShareLinkPasswordProtectedPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public ShareLinkPasswordComplexity ShareLinkPasswordComplexity = ShareLinkPasswordComplexity.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public int ShareLinkPasswordLength;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public ShareLinkExpirationPolicy ShareLinkExpirationPolicy;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public int ShareLinkExpireInDays;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public StoragePasswordPolicy StoragePasswordPolicy = StoragePasswordPolicy.Enabled;

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public StoragePasswordComplexityPolicy StoragePasswordComplexityPolicy = StoragePasswordComplexityPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public StoragePasswordComplexity StoragePasswordComplexity = new StoragePasswordComplexity();

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public StorageCookiePersistancePolicy StorageCookiePersistancePolicy = StorageCookiePersistancePolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public int? StorageCookiePersistancePolicyLength;

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public DesktopShareLinkFlowPolicy DesktopShareLinkFlowPolicy = DesktopShareLinkFlowPolicy.Unknown;
    }
}