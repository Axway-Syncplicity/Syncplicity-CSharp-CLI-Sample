using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class Policy
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public ClientAutoUpdatePolicy ClientAutoUpdatePolicy;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public ShareableLinkPolicy ShareableLinkPolicy;

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public ClientPreconfiguredPolicy ClientPreconfiguredPolicy;

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public SharedFolderPolicy SharedFolderPolicy;

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public IncludeOwnerInFolderNamePolicy IncludeOwnerInFolderNamePolicy;

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public RemoteWipeSyncpointPolicy RemoteWipeSyncpointPolicy = RemoteWipeSyncpointPolicy.Unknown;

        [DataMember(EmitDefaultValue = false, Order = 6)]
        public MobileSyncPolicy MobileSyncPolicy;

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public MobileUnencryptedSyncPolicy MobileUnencryptedSyncPolicy;

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public MobileSyncLimitsPolicy MobileSyncLimitsPolicy;

        [DataMember(EmitDefaultValue = false, Order = 9)]
        public MobileDataSyncLimit MobileDataSyncLimit = new MobileDataSyncLimit();

        [DataMember(EmitDefaultValue = false, Order = 10)]
        public ShareLinkPasswordProtectedPolicy ShareLinkPasswordProtectedPolicy;

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public ShareLinkPasswordComplexity ShareLinkPasswordComplexity;

        [DataMember(EmitDefaultValue = false, Order = 12)]
        public int ShareLinkPasswordLength;

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public ShareLinkExpirationPolicy ShareLinkExpirationPolicy;

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public int ShareLinkExpireInDays;

        //old clients before implementing VRI feature use ShareableLinkPolicy
        //new clients that support VRI use ShareLinkPolicy
        [DataMember(EmitDefaultValue = false, Order = 15)]
        public ShareLinkPolicy ShareLinkPolicy;

        [DataMember(EmitDefaultValue = false, Order = 16)]
        public DesktopShareLinkFlowPolicy DesktopShareLinkFlowPolicy;

        [DataMember(EmitDefaultValue = false, Order = 17)]
        public ReportExportOutputFolderPolicy ReportExportOutputFolderPolicy;

        [DataMember(EmitDefaultValue = false, Order = 18)]
        public AdminIpRestrictionPolicy AdminIpRestrictionPolicy;

        [DataMember(EmitDefaultValue = false, Order = 19)]
        public AdminIpRestriction AdminIpRestriction = new AdminIpRestriction();

        [DataMember(EmitDefaultValue = false, Order = 20)]
        public AdminPasswordComplexityPolicy AdminPasswordComplexityPolicy;

        [DataMember(EmitDefaultValue = false, Order = 21)]
        public AdminPasswordComplexity AdminPasswordComplexity = new AdminPasswordComplexity();

        [DataMember(EmitDefaultValue = false, Order = 22)]
        public RestrictMobileAccessPolicy RestrictMobileAccessPolicy;

        [DataMember(EmitDefaultValue = false, Order = 23)]
        public RestrictWebsiteAccessPolicy RestrictWebsiteAccessPolicy;

        [DataMember(EmitDefaultValue = false, Order = 24)]
        public RssNewsFeedPolicy RssNewsFeedPolicy;

        [DataMember(EmitDefaultValue = false, Order = 25)]
        public BranchingPolicy BranchingPolicy;

        [DataMember(EmitDefaultValue = false, Order = 26)]
        public RemoteWipeUsersPolicy RemoteWipeUsersPolicy;

        [DataMember(EmitDefaultValue = false, Order = 27)]
        public RemoteWipeEndpointPolicy RemoteWipeEndpointPolicy;

        [DataMember(EmitDefaultValue = false, Order = 28)]
        public EnforceClientMemberActiveDirectoryPolicy EnforceClientMemberActiveDirectoryPolicy;

        [DataMember(EmitDefaultValue = false, Order = 29)]
        public string EnforceClientMemberActiveDirectoryPolicyDomains;
        
        [DataMember(EmitDefaultValue = false, Order = 31)]
        public System.Guid DefaultStorageEndpointId;

        [DataMember(EmitDefaultValue = false, Order = 32)]
        public PasscodeEnforcementPolicy PasscodeEnforcementPolicy;

        [DataMember(EmitDefaultValue = false, Order = 33)]
        public PasscodeFailurePolicy PasscodeFailurePolicy;

        [DataMember(EmitDefaultValue = false, Order = 34)]
        public PasscodeFailures PasscodeFailures = new PasscodeFailures();

        [DataMember(EmitDefaultValue = false, Order = 35)]
        public OpenInPolicy OpenInPolicy;

        [DataMember(EmitDefaultValue = false, Order = 36)]
        public SharedFolderResharingPolicy SharedFolderResharingPolicy;

        [DataMember(EmitDefaultValue = false, Order = 37)]
        public GeolocationPrivacyPolicy GeolocationPrivacyPolicy;

    	[DataMember(EmitDefaultValue = false, Order = 44)]
        public HomeDirectoryPolicy HomeDirectoryPolicy;

        [DataMember(EmitDefaultValue = false, Order = 45)]
        public FileExclusionPolicy FileExclusionPolicy;

        [DataMember(EmitDefaultValue = false, Order = 46)]
        public FileExclusionType[] ExclusionTypes;
    }
}
