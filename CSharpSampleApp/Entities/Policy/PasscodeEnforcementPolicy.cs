using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    /// <summary>
    /// Allow IT to enforce the use of a passcode, if enabled users will need to set a passcode on first use or next use.
    /// </summary>
    [DataContract(Namespace = "")]
    public enum PasscodeEnforcementPolicy : byte
    {
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Passcode Enforcement Policy is set OFF. It is default value
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Passcode Enforcement Policy is set ON.
        /// </summary>
        [EnumMember]
        Enabled = 2
    }
}