using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum ShareResourceType : byte
    {
        /// <summary>
        /// Default link resource.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Link to a File.
        /// </summary>
        [EnumMember]
        File = 1,

        /// <summary>
        /// Link to a Folder
        /// </summary>
        [EnumMember]
        Folder = 2
    }
}
