using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{

    /// <summary>
    /// An enumeration of possible shareable link policies
    /// </summary>
    [DataContract(Namespace = "")]
    public enum SharedFolderResharingPolicy : short
    {
        /// <summary>
        /// Default policy
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Disable folder re-sharing for external Users. Folders can only be re-shared by folks in the same company.
        /// </summary>
        [EnumMember]
        DisallowAll = 1,

        /// <summary>
        /// Folders can only be re-shared by external users who has Editor permissions
        /// </summary>
        [EnumMember]
        ExternalEditorOnly = 2,

        /// <summary>
        /// Allow re-sharing by anyone (Consumer behavior)
        /// </summary>
        [EnumMember]
        AllowAll = 3
    }
}