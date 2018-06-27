using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum CompanyAccountType : byte
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        BusinessEdition = 1,

        [EnumMember]
        BusinessEditionWithPremiumSupport = 2,

        [EnumMember]
        EnterpriseEdition = 3,

        [EnumMember]
        AccessEdition = 4,

        [EnumMember]
        DepartmentEdition = 5
    }
}