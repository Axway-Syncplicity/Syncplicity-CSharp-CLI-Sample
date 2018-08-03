using System;
using System.Collections.Generic;
using CSharpSampleApp.Entities.PII;

namespace CSharpSampleApp.Services
{
    public class UserPiiService : ApiGateway
    {
        private static string UserPiiUrl => RolAPIUrlPrefix + "users_pii.svc/";

        public static PII GetUsersPiis(IEnumerable<Guid> uuidList, string bearer = null)
        {
			return HttpPost<PII, IEnumerable<Guid>>(UserPiiUrl, uuidList, bearer);
        }
    }
}
