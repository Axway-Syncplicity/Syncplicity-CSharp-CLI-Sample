using System;
using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    [Flags]
    public enum PasswordComplexityOptions
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Number = 1,
        [EnumMember]
        Lower = 2,
        [EnumMember]
        Upper = 4,
        [EnumMember]
        SpecialCharacter = 8,
        [EnumMember]
        NumberOrSpecialCharacter = 16,
        [EnumMember]
        NotDictionaryWord = 32
    }
}
