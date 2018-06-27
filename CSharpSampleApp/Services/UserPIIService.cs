using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpSampleApp.Entities.PII;

namespace CSharpSampleApp.Services
{
    public class UserPIIService : APIGateway
    {
        private static string UserPIIUrl { get { return RolAPIUrlPrefix + "users_pii.svc/"; } }

        public static PII GetUsersPiis(IEnumerable<Guid> uuidList, string bearer = null)
        {
			return HttpPost<PII, IEnumerable<Guid>>(UserPIIUrl, uuidList, bearer);
        }
    }
}
