﻿using System;
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
        const string JsonContentType = "application/json";

        #region Protected Properties

        /// <summary>
        /// Gets or sets the base url for auth endpoint.
        /// </summary>        
        public static string StorageAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/storage/";

        public static string SyncAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/sync/";

        public static string SyncpointAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/syncpoint/";

        /// <summary>
        /// Gets or sets the base url for auth endpoint.
        /// </summary>
        protected static string ProvisioningAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/provisioning/";

        /// <summary>
        /// Gets or sets the base url for users_pii service.
        /// </summary>
        protected static string RolAPIUrlPrefix => GolGateway.BaseApiEndpointUrl + "/rol/";

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the current credentials to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The current request.</returns>
        private static HttpWebRequest ApplyConsumerCredentials(HttpWebRequest request, bool isFirstAuthCall = false, bool isMachineAuthentication = false, string bearer = null)
        {
            //If this is the first OAuth authentication call, then we don't have an OAuth Bearer token (access token), so we will use the
            //Application Key and Application Secret as the consumer credentials for the application.  However, once we've successfully
            //connected to the api gateway for the first time, we will receive an OAuth access token (Bearer token), you will
            //need to manage that bearer token and use it for subsequent calls to the API gateway.
            if (isFirstAuthCall)
            {
                var basicAuthRawToken = ConfigurationHelper.ApplicationKey + ":" + ConfigurationHelper.ApplicationSecret;
                var basicAuthToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(basicAuthRawToken));
                Console.WriteLine("[Header] Authorization: Basic " + basicAuthToken + " (Base64 encoded combination of App key and App secret)");
                request.Headers.Add("Authorization", "Basic " + basicAuthToken);

                if (isMachineAuthentication)
                {
                    if (!ConfigurationHelper.SyncplicityMachineTokenAuthenticationEnabled)
                    {
                        throw new ConfigurationErrorsException("machineToken key value should be defined in configuration");
                    }
                    Console.WriteLine("[Header] Sync-Machine-Token: " + ConfigurationHelper.SyncplicityMachineToken);
                    request.Headers.Add("Sync-Machine-Token", ConfigurationHelper.SyncplicityMachineToken);
                }
                else
                {
                    Console.WriteLine("[Header] Sync-App-Token: " + ConfigurationHelper.GetSyncplicityAdminToken(GolGateway.CurrentRol.Id));
                    request.Headers.Add("Sync-App-Token", ConfigurationHelper.GetSyncplicityAdminToken(GolGateway.CurrentRol.Id));
                }
            }
            else
            {
                Console.WriteLine("[Header] AppKey: " + ConfigurationHelper.ApplicationKey );
    			Console.WriteLine("[Header] Authorization: Bearer " +  ApiContext.AccessToken );
                request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
                request.Headers.Add("Authorization", "Bearer " + (bearer ?? ApiContext.AccessToken));
            }

            if (ApiContext.OnBehalfOfUser.HasValue)
            {
                Console.WriteLine("[Header] As-User: " + ApiContext.OnBehalfOfUser.Value.ToString("D"));
                request.Headers.Add("As-User", ApiContext.OnBehalfOfUser.Value.ToString("D"));
            }

            return request;
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
                        Console.WriteLine("[Response] " );

                        var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
                        Console.WriteLine(pp.PrettyPrint(response));

                        if (typeof(T) != typeof(string))
                        {
                            memoryStream.Position = 0;

                            using (var reader = new StreamReader(memoryStream))
                            using (var jsonReader = new JsonTextReader(reader))
                            {
                                return JsonSerializer.Create(new JsonSerializerSettings())
                                    .Deserialize<T>(jsonReader);
                            }
                        }

                        return (T) (object) response;
                    }
                }
            }
            catch (WebException e)
            {
                var response = (HttpWebResponse) e.Response;
                if (response == null)
                {
                    Console.WriteLine("Response not received.");
                    return default(T);
                }

                Console.WriteLine($"Error {(int)response.StatusCode} {response.StatusDescription} occurs during request to {response.ResponseUri}.");

                // it's needed to authorize again and then send the same request again
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
        /// <returns>Created request.</returns>
        private static HttpWebRequest CreateRequest(string method, string uri, bool isFirstAuthCall = false, bool isMachineAuthentication = false, string bearer = null)
        {
            Console.WriteLine("Creating {0} request to {1}", method.ToUpper(), uri);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = method.ToUpper();
            request.Accept = JsonContentType;
            request.CookieContainer = new CookieContainer();
            request.Timeout = 30000;
            return ApplyConsumerCredentials(request, isFirstAuthCall, isMachineAuthentication, bearer:bearer);
        }

        /// <summary>
        /// Writes the body to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="body">The string representation of body.</param>
        private static void WriteBody(WebRequest request, string body)
        {
            var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
            Console.WriteLine("[Body] " );
            Console.WriteLine( pp.PrettyPrint(body) );

            var data = Encoding.ASCII.GetBytes(body);
            request.ContentLength = data.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }
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

            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // it's needed to authorize again
            // trying to do it and then re-send the initial request
            OAuth.OAuth.RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, GET request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send GET request again for the last time.");

            request = CreateRequest(method, uri);
            response = ReadLastAttemptResponse<T>(request);

            return response;
        }


        /// <summary>
        /// Create POST HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <param name="isMachineAuthentication">Set to true if machine authentication is required instead of regular user authentication.
        /// As a result different headers will be used</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPost<T>(string uri, string body, string contentType, bool isFirstAuthCall = false, bool isMachineAuthentication = false, string bearer = null)
        {
            const string method = "POST";
            var request = CreateRequest(method, uri, isFirstAuthCall, isMachineAuthentication, bearer: bearer);

            request.ContentType = contentType;
            WriteBody(request, body);

            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (isFirstAuthCall || !isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // it's needed to authorize again
            // trying to do it and then re-send the initial request
            OAuth.OAuth.RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, POST request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send POST request again for the last time.");

            request = CreateRequest(method, uri, false, isMachineAuthentication);
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
            return HttpPost<T>(uri, Serialization.JSONSerizalize(body), JsonContentType);
        }

        protected static TResult HttpPost<TResult, TBody>(string uri, TBody body, string bearer = null)
            where TResult : class
            where TBody : class
        {
            return HttpPost<TResult>(uri, Serialization.JSONSerizalize(body), JsonContentType, bearer: bearer);
        }


        /// <summary>
        /// Create DELETE HTTP request to url return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpDelete<T>(string uri, object body = null)
        {
            const string method = "DELETE";
            var request = CreateRequest(method, uri);
            if (body != null)
            {
                request.ContentType = JsonContentType;
                WriteBody(request, Serialization.JSONSerizalize(body));
            }
            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // it's needed to authorize again
            // trying to do it and then re-send the initial request
            OAuth.OAuth.RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, DELETE request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send DELETE request again for the last time.");

            request = CreateRequest(method, uri);
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
        protected static T HttpPut<T>(string uri, string body, string contentType)
        {
            const string method = "PUT";
            var request = CreateRequest(method, uri);
            request.ContentType = contentType;
            WriteBody(request, body);
            
            bool isNeededReAuth;
            var response = ReadFirstAttemptResponse<T>(request, out isNeededReAuth);
            if (!isNeededReAuth) return response;

            Console.WriteLine();
            Console.WriteLine("Trying to re-authenticate using the same credentials.");

            // it's needed to authorize again
            // trying to do it and then re-send the initial request
            OAuth.OAuth.RefreshToken();

            Console.WriteLine();
            if (!ApiContext.Authenticated)
            {
                Console.WriteLine("The OAuth authentication has failed, PUT request can't be performed.");
                return default(T);
            }

            Console.WriteLine("Authentication was successful. Trying to send PUT request again for the last time.");

            request = CreateRequest(method, uri);
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
            return HttpPut<T>(uri, Serialization.JSONSerizalize(body), JsonContentType);
        }

        #endregion Protected Methods
    }
}