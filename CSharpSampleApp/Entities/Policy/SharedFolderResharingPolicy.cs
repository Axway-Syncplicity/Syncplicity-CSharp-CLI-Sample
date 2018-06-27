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
        /// Disable folder resharing for external Users. Folders can only be reshared by folks in the same company.
        /// </summary>
        [EnumMember]
        DisallowAll = 1,
        /// <summary>
        /// Folders can only be reshared by external users who has Editor permissions
        /// </summary>
        [EnumMember]
        ExternalEditorOnly = 2,
        /// <summary>
        /// Allow resharing by anyone (Consumer behavior)
        /// </summary>
        [EnumMember]
        AllowAll = 3
    }
}