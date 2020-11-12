using CSharpSampleApp.Entities;
using System;

namespace CSharpSampleApp.Services
{
    public class CompanyService : ApiGateway
    {
        protected static string companyServiceUrl = ProvisioningAPIUrlPrefix + "company.svc/{0}";

        /// <summary>
        /// GET company.svc/{COMPANY_GUID}
        /// Get company information
        /// </summary>
        public static void GetCompany()
        {
            try
            {
                HttpGet<Company>(string.Format(companyServiceUrl, ApiContext.CompanyGuid));
            }
            catch (InvalidOperationException)
            {
                ValidateCompanyGuidMessage(ApiContext.CompanyGuid);
                throw;
            }
        }

        private static void ValidateCompanyGuidMessage(Guid? companyGuid)
        {
            if (!companyGuid.HasValue)
            {
                Console.WriteLine("ERROR: Company Giud has no value.");
            }
        }
    }
}
