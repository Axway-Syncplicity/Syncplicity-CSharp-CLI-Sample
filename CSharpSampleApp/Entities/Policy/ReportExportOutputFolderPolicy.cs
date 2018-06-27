using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// An enumeration of possible report export output policies for a company
    /// </summary>
    [DataContract(Namespace = "")]
    public enum ReportExportOutputFolderPolicy : byte
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Export to single virtual folder
        /// </summary>
        [EnumMember]
        SingleVirtualFolder = 1,

        /// <summary>
        /// Use separate virtual folder for each report type
        /// </summary>
        [EnumMember]
        SeparateVirtualFoldersByReportType = 2
    }
}
