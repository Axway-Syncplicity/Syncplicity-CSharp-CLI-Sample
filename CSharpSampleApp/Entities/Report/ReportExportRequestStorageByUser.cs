using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class ReportExportRequestStorageByUser : ReportExportRequestBase
    {
        [DataMember(Order = 0)]
        public User User;

        [DataMember(Order = 1)]
        public bool IncludeActive;

        [DataMember(Order = 2)]
        public bool IncludeDisabled;
    }
}
