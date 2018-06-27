using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum ConcurrentUseNotificationPolicy : byte
    {
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Don't send notification if multiple sessions was detected.
        /// </summary>
        [EnumMember]
        Disable = 1,

        /// <summary>
        /// Notify only company admins if multiple sessions was detected.
        /// </summary>
        [EnumMember]
        OnlyAdmins = 2,

        /// <summary>
        /// Notify company admins and user if multiple sessions was detected.
        /// </summary>
        [EnumMember]
        AdmnsAndEndUser = 3
    }
}
