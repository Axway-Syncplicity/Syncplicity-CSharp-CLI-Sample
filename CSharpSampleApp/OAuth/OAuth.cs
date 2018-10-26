using CSharpSampleApp.Services;
using CSharpSampleApp.Util;


namespace CSharpSampleApp.OAuth
{
    public class OAuth : ApiGateway
    {
        public static void Authenticate()
        {            
            var response = HttpPost<TokenResponse>(GolGateway.OAuthTokenUrl,
                                                             "grant_type=client_credentials&scope=read%20readwrite",
                                                             "application/x-www-form-urlencoded",
                                                             true);

            ApiContext.OAuthResponse = response;

            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                response = HttpPost<TokenResponse>(GolGateway.OAuthTokenUrl,
                "grant_type=client_credentials&scope=read%20readwrite",
                "application/x-www-form-urlencoded",
                true, true);
                
                ApiContext.MachineToken = response.AccessToken;
            }
        }

        /// <summary>
        /// This call will invalidate the current oauth and any refresh-tokens
        /// along with removing the grant of access to the application to the given user account.   
        /// </summary>
        public static void RevokeToken() {


            var response = HttpGet<TokenResponse>(GolGateway.OAuthRevokeTokenUrl);

            // Response will/should be null for revoke
            ApiContext.OAuthResponse = response;
	    }
	
	    public static void RefreshToken() {
		
		    // Note: technically refreshToken() which uses grant_type=client_credentials
		    // is the same behavior as just authenticating authenticate() for the first time.
		    // The name is just to be explicit in the use-case
		    Authenticate();
	    }
    }
}
