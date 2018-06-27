using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class User
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public Guid Id { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string EmailAddress { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string FirstName { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string LastName { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 4)]
        public string Password { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 5)]
        public bool? Disabled { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 7)]
        public int[] Roles { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 8)]
        public AccountType AccountType { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 11)]
        public AccountStatus Status { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 13)]
        public DateTime? LastLoginDateUtc { get; set; }

        [DataMember(EmitDefaultValue = false, Order = 14)]
        public DateTime CreatedDateUtc { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 15)]
		public int ? OriginalRolId { get; set; }
    }
}