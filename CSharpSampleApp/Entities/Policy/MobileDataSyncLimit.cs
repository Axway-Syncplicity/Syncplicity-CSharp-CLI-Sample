using System.Runtime.Serialization;

namespace CSharpSampleApp.Entities
{
    [DataContract(Namespace = "")]
    public class MobileDataSyncLimit
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public int TransferLimit;

        [DataMember(EmitDefaultValue = false, Order = 1)]
        public int BillingCycleResetDay;
    }
}
