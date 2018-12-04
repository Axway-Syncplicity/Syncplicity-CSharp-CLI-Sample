using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class LegalHold
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid? LegalHoldId { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public User User { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public DateTime? DateExpiredUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public User RequestedByUser { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public DateTime? DateCreatedUtc { get; set; }
    }
}