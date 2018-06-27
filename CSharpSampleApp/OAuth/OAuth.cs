using System;

using CSharpSampleApp.Services;
using CSharpSampleApp.Util;


namespace CSharpSampleApp.OAuth
{
    public class OAuth : APIGateway
    {
        public static void authenticate()
        {            
            TokenResponse response = HttpPost<TokenResponse>(GolGateway.OAuthTokenUrl,
                                                             "grant_type=client_credentials&scope=read%20readwrite",
                                                             "application/x-www-form-urlencoded",
                                                             true);

            APIContext.OAuthResponse = response;

            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                response = HttpPost<TokenResponse>(GolGateway.OAuthTokenUrl,
                "grant_type=client_credentials&scope=read%20readwrite",
                "application/x-www-form-urlencoded",
                true, true);
                
                APIContext.MachineToken = response.AccessToken;
            }
        }

        //This call will invalidate the current oauth 
        //and any refresh-tokens along with removing
        //the grant of access to the application to 
        //the given user account.   
        public static void revokeToken() {


            TokenResponse response = HttpGet<TokenResponse>(GolGateway.OAuthRevokeTokenUrl);

            //response will/should be null for revoke
            APIContext.OAuthResponse = response;
	    }
	
	    public static void refreshToken() {
		
		    //Note: technically refreshToken() which uses grant_type=client_credentials is the same
		    //      behavior as just authenticating authenticate() for the first time.  The name is 
		    //      just to be explicit in the use-case
		    authenticate();
	    }
    }
}
