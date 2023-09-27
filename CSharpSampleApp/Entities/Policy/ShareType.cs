using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum ShareType : byte
    {
        /// <summary>
        /// Default link type.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Link for Outlook Plugin feature.
        /// </summary>
        [EnumMember]
        AttachmentLink = 1,

        /// <summary>
        /// Basic shareable link to file or Folder
        /// </summary>
        [EnumMember]
        ShareLink = 2,

        /// <summary>
        ///Shortcut to file or Folder
        /// </summary>
        [EnumMember]
        Shortcut = 3,

        /// <summary>
        ///Deep shareable link to file or Folder
        /// </summary>
        [EnumMember]
        DeepLink = 4
    }
}
