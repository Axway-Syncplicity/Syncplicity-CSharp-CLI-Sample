using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class ReportExportRequestBase
    {
        [DataMember(Order = 0)]
        public string ExportFileName;
    }
}
