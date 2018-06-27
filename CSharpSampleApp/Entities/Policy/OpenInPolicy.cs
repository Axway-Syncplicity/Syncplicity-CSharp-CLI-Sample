using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Allow IT to disable the ability for users to open files into other apps.
    /// </summary>
    [DataContract(Namespace = "")]
    public enum OpenInPolicy : byte
    {
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Open-In Policy is set OFF. It is default value
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Open-In Policy is set ON.
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}