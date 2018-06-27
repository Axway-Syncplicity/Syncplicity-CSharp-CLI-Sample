using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum ShareWithNewParticipantOption : short
    {
        /// <summary>
        /// Create external accounts, not associated with business account, for new collaborators
        /// </summary>
        [EnumMember]
        CreateExternalAccount = 0,

        /// <summary>
        /// Add new collaborators to business account
        /// </summary>
        [EnumMember]
        CreateInternalAccount = 1
    }
}