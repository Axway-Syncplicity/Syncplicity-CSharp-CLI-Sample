using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class PasscodeFailures
    {
        [DataMember(EmitDefaultValue = true, Order = 0)]
        public int AllowablePasscodeFailures;
    }
}