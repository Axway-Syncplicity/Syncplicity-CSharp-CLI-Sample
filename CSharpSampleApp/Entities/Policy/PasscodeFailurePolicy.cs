using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// That ensures that if users type incorrect passcode “x” times, the user is logged out and cache is deleted.
    /// </summary>
    [DataContract(Namespace = "")]
    public enum PasscodeFailurePolicy : byte
    {
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Passcode Failure Policy is set OFF. It is default value
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Passcode Failure Policy is set ON.
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}