using System;
using CSharpSampleApp.Entities;

namespace CSharpSampleApp.Services
{
    public class LegalHoldsService : ApiGateway
    {
        #region Static Members

        /// <summary>
        /// Gets url to LegalHolds service.
        /// </summary>
        protected static string LegalHoldsUrl => ProvisioningAPIUrlPrefix + "legal_holds.svc/";

        #endregion Static Members

        public static void PutOnLegalHold(Guid custodianGuid, TimeSpan? period = null)
        {
            var legalHold = new LegalHold
            {
                DateExpiredUtc = period.HasValue ? DateTime.UtcNow + period.Value : (DateTime?)null,
                User = new User
                {
                    Id = custodianGuid
                }
            };
            HttpPost(LegalHoldsUrl, new[] { legalHold });
        }
    }
}