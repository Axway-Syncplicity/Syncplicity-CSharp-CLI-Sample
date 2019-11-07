using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using CSharpSampleApp.Util;
using Newtonsoft.Json;
using JsonPrettyPrinterPlus;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Base class for API services.
    /// </summary>
    public class ApiGateway
    {
        private const string JsonContentType = "application/json";
        private static JsonSerializerSettings _serializerSettings;

        #region Protected Properties

        /// <summary>
        /// Gets or sets the base url for auth endpoint.
        /// </summary>        
        public static string StorageAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/storage/";

        public static string SyncAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/sync/";

        public static string SyncpointAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/syncpoint/";

        /// <summary>
        /// Gets or sets the base url for auth endpoint.b
        /// </summary>
        protected static string ProvisioningAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/provisioning/";

        /// <summary>
        /// Gets or sets the base url for users_pii service.
        /// </summary>
        protected static string RolAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/rol/";

        #endregion

        static ApiGateway()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }

        #region Private Methods

        /// <summary>
        /// Applies the current credentials to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The current request.</returns>
        private static HttpWebRequest ApplyBearerAuthorization(HttpWebRequest request)
        {
            Console.WriteLine($"[Header] AppKey: {ConfigurationHelper.ApplicationKey}");
            Console.WriteLine($"[Header] Authorization: Bearer {ApiContext.AccessToken}");
            request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
            request.Headers.Add("Authorization", $"Bearer {ApiContext.AccessToken}");

            if (ApiContext.OnBehalfOfUser.HasValue)
            {
                Console.WriteLine($"[Header] As-User: {ApiContext.OnBehalfOfUser.Value:D}");
                request.Headers.Add("As-User", ApiContext.OnBehalfOfUser.Value.ToString("D"));
            }

            return request;
        }

        /// <summary>
        /// Applies the current credentials to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="isMachineAuthentication">Set to true if machine authentication is required instead of regular user authentication.
        /// As a result different headers will be used</param>
        /// <returns>The current request.</returns>
        private static TokenResponse CreateToken(string body, Action<HttpWebRequest> prepareRequest = null)
        {
            // If this is the first OAuth authentication call, then we don't have an OAuth Bearer token (access token).
            // So we will use the Application Key and Application Secret as the consumer credentials for the application.
            // However, once we've successfully connected to the api gateway for the first time,
            // we will receive an OAuth access token (Bearer token).
            // We will need to manage the bearer token and use it for subsequent calls to the API gateway.

            const string contentType = "application/x-www-form-urlencoded";

            var request = CreateRequest("POST", GolGateway.OAuthTokenUrl);
            var basicAuthRawToken = $"{ConfigurationHelper.ApplicationKey}:{ConfigurationHelper.ApplicationSecret}";
            var basicAuthToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(basicAuthRawToken));
            Console.WriteLine($"[Header] Authorization: Basic {basicAuthToken} (Base64 encoded combination of App key and App secret)");
            request.Headers.Add("Authorization", $"Basic {basicAuthToken}");

            prepareRequest?.Invoke(request);

            request.ContentType = contentType;
            WriteBody(request, body);

            var response = ReadFirstAttemptResponse<TokenResponse>(request, out var isNeededReAuth);
            return response;
        }

        /// <summary>
        /// Reads the response from the request and returns the received object.
        /// </summary>
        /// <typeparam name="T">The type of received object.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T ReadLastAttemptResponse<T>(WebRequest request)
        {
            bool isNeededReAuth;
            return ReadResponse<T>(request, 1, out isNeededReAuth);
        }

        /// <summary>
        /// Reads the response from the request and returns the received object.
        /// </summary>
        /// <typeparam name="T">The type of received object.</typeparam>
        /// <param name="request">The request object.</param>
        /// <param name="isNeededReAuth">
        /// Outputs a value that determines if re-authentication should be performed with a subsequent additional request attempt.
        /// </param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T ReadFirstAttemptResponse<T>(WebRequest request, out bool isNeededReAuth)
        {
            return ReadResponse<T>(request, 0, out isNeededReAuth);
        }


        /// <summary>
        /// Reads the response from the request and returns the received object.
        /// </summary>
        /// <typeparam name="T">The type of received object.</typeparam>
        /// <param name="request">The request object.</param>
        /// <param name="requestAttemptIndex">The index of this attempt to execute request.</param>
        /// <param name="isNeededReAuth">
        /// Outputs a value that determines if re-authentication should be performed with a subsequent additional request attempt.
        /// </param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T ReadResponse<T>(WebRequest request, int requestAttemptIndex, out bool isNeededReAuth)
        {
            isNeededReAuth = false;

            try
            {
                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        Console.WriteLine("Response wasn't received.");
                        return default(T);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        responseStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        var response = Encoding.UTF8.GetString(memoryStream.ToArray());
                        Console.WriteLine("[Response] ");

                        var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
                        Console.WriteLine(pp.PrettyPrint(response));

                        if (typeof(T) != typeof(string))
                        {
                            memoryStream.Position = 0;

                            using (var reader = new StreamReader(memoryStream))
                            using (var jsonReader = new JsonTextReader(reader))
                            {
                                return JsonSerializer.Create(_serializerSettings)
                                    .Deserialize<T>(jsonReader);
                            }
                        }

                        return (T)(object)response;
                    }
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse)e.Response;
                if (response == null)
                {
                    Console.WriteLine("Response not received.");
                    return default(T);
                }

                Console.WriteLine($"Error {(int)response.StatusCode} {response.StatusDescription} occurs during request to {response.ResponseUri}.");

                // Need to authenticate again and re-send the request.
                var unauthorizedProbablyBecauseOfExpiredToken = response.StatusCode == HttpStatusCode.Unauthorized ||
                        response.StatusCode == HttpStatusCode.Forbidden && response.StatusDescription == "Forbidden";
                var isFirstRequestAttempt = requestAttemptIndex == 0;
                if (unauthorizedProbablyBecauseOfExpiredToken && isFirstRequestAttempt)
                {
                    isNeededReAuth = true;
                    return default(T);
                }

                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var memStream = new MemoryStream())
                        {
                            stream.CopyTo(memStream);
                            memStream.Position = 0;
                            Console.WriteLine("Response body:");
                            Console.WriteLine(Encoding.UTF8.GetString(memStream.ToArray()));
                        }
                    }
                }
                Console.WriteLine("WebException:");
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Creates request object.
        /// </summary>
        /// <param name="method">The request's method.</param>
        /// <param name="uri">The url of request.</param>
        /// <param name="isMachineAuthentication">Set to true if machine authentication is required instead of regular user authentication.
        /// As a result different headers will be used</param>
        /// <returns>Created request.</returns>
        private static HttpWebRequest CreateRequest(string method, string uri)
        {
            Console.WriteLine($"Creating {method.ToUpper()} request to {uri}");
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = method.ToUpper();
            request.Accept = JsonContentType;
            request.CookieContainer = new CookieContainer();
            request.Timeout = 30000;
            return request;
        }

        /// <summary>
        /// Writes the body to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="body">The string representation of body.</param>
        private static void WriteBody(WebRequest request, string body)
        {
            var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
            Console.WriteLine("[Body] ");
            Console.WriteLine(pp.PrettyPrint(body));

            var data = Encoding.ASCII.GetBytes(body);
            request.ContentLength = data.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }
        }

        private static string JsonSerizalize<T>(T entity) where T : class
        {
            if (entity == null) return null;

            return JsonConvert.SerializeObject(entity, _serializerSettings);
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Create GET HTTP request to url return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpGet<T>(string uri)
        {
            const string method = "GET";
            var request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);

            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // We need to authenticate again.
            // Trying to do it and then re-send the initial request:
            RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, GET request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send GET request again for the last time.");

            request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            response = ReadLastAttemptResponse<T>(request);

            return response;
        }


        /// <summary>
        /// Create POST HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <param name="contentType">The type of request content.</param>
        /// <param name="isMachineAuthentication">Set to true if machine authentication is required instead of regular user authentication.
        /// As a result different headers will be used</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPost<T>(string uri, string body, string contentType)
        {
            const string method = "POST";
            var request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);

            request.ContentType = contentType;
            WriteBody(request, body);

            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // We need to authenticate again.
            // Trying to do it and then re-send the initial request:
            RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, POST request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send POST request again for the last time.");

            request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            request.ContentType = contentType;
            WriteBody(request, body);

            response = ReadLastAttemptResponse<T>(request);

            return response;
        }


        /// <summary>
        /// Create POST HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPost<T>(string uri, T body)
            where T : class
        {
            return HttpPost<T>(uri, JsonSerizalize(body), JsonContentType);
        }


        /// <summary>
        /// Create DELETE HTTP request to url return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpDelete<T>(string uri, object body = null)
        {
            const string method = "DELETE";
            var request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            if (body != null)
            {
                request.ContentType = JsonContentType;
                WriteBody(request, JsonSerizalize(body));
            }
            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // We need to authenticate again.
            // Trying to do it and then re-send the initial request:
            RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, DELETE request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send DELETE request again for the last time.");

            request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            response = ReadLastAttemptResponse<T>(request);

            return response;
        }

        /// <summary>
        /// Create PUT HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <param name="contentType">The type of request content.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPut<T>(string uri, string body, string contentType)
        {
            const string method = "PUT";
            var request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            request.ContentType = contentType;
            WriteBody(request, body);

            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // We need to authenticate again.
            // Trying to do it and then re-send the initial request:
            RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, PUT request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send PUT request again for the last time.");

            request = CreateRequest(method, uri);
            ApplyBearerAuthorization(request);
            request.ContentType = contentType;
            WriteBody(request, body);

            response = ReadLastAttemptResponse<T>(request);

            return response;
        }

        /// <summary>
        /// Create PUT HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPut<T>(string uri, T body)
            where T : class
        {
            return HttpPut<T>(uri, JsonSerizalize(body), JsonContentType);
        }

        #endregion Protected Methods

        #region Public Methods
        public static void Authenticate()
        {
            const string authRequestBody = "grant_type=client_credentials&scope=read%20readwrite";

            var tokenResponse = CreateToken(authRequestBody, request =>
            {
                Console.WriteLine($"[Header] Sync-App-Token: {ApiContext.SyncplicityUserAppToken}");
                request.Headers.Add("Sync-App-Token", ApiContext.SyncplicityUserAppToken);
            });

            ApiContext.OAuthResponse = tokenResponse;

            if (ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
            {
                tokenResponse = CreateToken(authRequestBody, request =>
                {
                    Console.WriteLine($"[Header] Sync-Machine-Token: {ConfigurationHelper.SyncplicityMachineToken}");
                    request.Headers.Add("Sync-Machine-Token", ConfigurationHelper.SyncplicityMachineToken);
                });

                ApiContext.MachineToken = tokenResponse.AccessToken;
            }
        }

        public static string CreateSst(Guid storageEndpointId)
        {
            var authRequestBody = string.Format("grant_type={0}&token={1}&resource={2}",
                    Uri.EscapeDataString("urn:syncplicity:oauth:grant-type:access-token"),
                    Uri.EscapeDataString(ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled
                        ? ApiContext.MachineToken
                        : ApiContext.AccessToken),
                    Uri.EscapeDataString($"urn:syncplicity:resources:storage:{storageEndpointId}"));

            var tokenResponse = CreateToken(authRequestBody, request => { });

            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// This call will invalidate the current oauth and any refresh-tokens
        /// along with removing the grant of access to the application to the given user account.   
        /// </summary>
        public static void RevokeToken()
        {


            var response = HttpGet<TokenResponse>(GolGateway.OAuthRevokeTokenUrl);

            // Response will/should be null for revoke
            ApiContext.OAuthResponse = response;
        }

        public static void RefreshToken()
        {

            // Note: technically refreshToken() which uses grant_type=client_credentials
            // is the same behavior as just authenticating authenticate() for the first time.
            // The name is just to be explicit in the use-case
            Authenticate();
        }
        #endregion
    }
}