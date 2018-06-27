﻿using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public enum FileStatus : byte
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Added = 1,

        [EnumMember]
        Updated = 2,

        [EnumMember]
        Removed = 3,

        [EnumMember]
        Ignored = 4,

        [EnumMember]
        ConfirmedRemoved = 5
    }
}