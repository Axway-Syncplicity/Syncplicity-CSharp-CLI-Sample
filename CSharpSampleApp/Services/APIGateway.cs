﻿using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using CSharpSampleApp.Util;
using Newtonsoft.Json;
using JsonPrettyPrinterPlus;

namespace CSharpSampleApp.Services
{
    /// <summary>
    /// Base class for API services.
    /// </summary>
    public class APIGateway
    {
        const String JSON_CONTENT_TYPE = "application/json";

        #region Protected Properties

        /// <summary>
        /// Gets or sets the base url for auth endpoint.
        /// </summary>        
        public static string StorageAPIUrlPrefix 
        { 
            get { return GolGateway.BaseApiEndpointUrl + "/storage/"; }
        }
        public static string SyncAPIUrlPrefix 
        { 
            get { return GolGateway.BaseApiEndpointUrl + "/sync/"; }
        }

        public static string SyncpointAPIUrlPrefix 
        {
            get { return GolGateway.BaseApiEndpointUrl + "/syncpoint/"; }
        }
            /// <summary>
        /// Gets or sets the base url for auth endpoint.
        /// </summary>
        protected static string ProvisioningAPIUrlPrefix
        {
            get { return GolGateway.BaseApiEndpointUrl + "/provisioning/"; }
        }

        /// <summary>
        /// Gets or sets the base url for users_pii service.
        /// </summary>
        protected static string RolAPIUrlPrefix
        {
            get { return GolGateway.BaseApiEndpointUrl + "/rol/"; }
        }

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
                string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes
                (
                    ConfigurationHelper.ApplicationKey + ":" + ConfigurationHelper.ApplicationSecret
                ));

                Console.WriteLine("[Header] Authorization: Basic " + encoded + " (Base64 encoded combination of App key and App secret)");
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
                request.Headers.Add("Authorization", "Basic " + encoded);
            }
            else
            {
                Console.WriteLine("[Header] AppKey: " + ConfigurationHelper.ApplicationKey );
    			Console.WriteLine("[Header] Authorization: Bearer " +  APIContext.AccessToken );
                request.Headers.Add("AppKey", ConfigurationHelper.ApplicationKey);
                request.Headers.Add("Authorization", "Bearer " + (bearer ?? APIContext.AccessToken));
            }

            if (APIContext.OnBehalfOfUser.HasValue)
            {
                Console.WriteLine("[Header] As-User: " + APIContext.OnBehalfOfUser.Value.ToString("D"));
                request.Headers.Add("As-User", APIContext.OnBehalfOfUser.Value.ToString("D"));
            }

            return request;
        }

        /// <summary>
        /// Reads the response from the request and returns the received object.
        /// </summary>
        /// <typeparam name="T">The type of received object.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T ReadResponse<T>(HttpWebRequest request)
        {
            bool isNeededReAuth = false;
            return ReadResponse<T>(request, out isNeededReAuth);
        }

        /// <summary>
        /// Reads the response from the request and returns the received object.
        /// </summary>
        /// <typeparam name="T">The type of received object.</typeparam>
        /// <param name="request">The request object.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T ReadResponse<T>(HttpWebRequest request, out bool isNeededReAuth)
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

                        string response = Encoding.UTF8.GetString(memoryStream.ToArray());
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

                        return (T) ((object) response);
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

                Console.WriteLine("Error {1} {2} occurs during request to {0}.", response.ResponseUri, (int)response.StatusCode, response.StatusDescription);

                // it's needed to authorize again and then send the same request again
                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    (response.StatusCode == HttpStatusCode.Forbidden && response.StatusDescription == "Forbidden"))
                {
                    isNeededReAuth = true;
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
            }

            return default(T);
        }

        /// <summary>
        /// Creates request obejct.
        /// </summary>
        /// <param name="method">The request's method.</param>
        /// <param name="uri">The url of request.</param>
        /// <returns>Created request.</returns>
        private static HttpWebRequest CreateRequest(string method, string uri, bool isFirstAuthCall = false, bool isMachineAuthentication = false, string bearer = null)
        {
            Console.WriteLine("Creating {0} request to {1}", method.ToUpper(), uri);
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = method.ToUpper();
            request.Accept = JSON_CONTENT_TYPE;
            request.CookieContainer = new CookieContainer();
            request.Timeout = 30000;
            return ApplyConsumerCredentials(request, isFirstAuthCall, isMachineAuthentication, bearer:bearer);
        }

        /// <summary>
        /// Writes the body to the request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="body">The string representation of body.</param>
        private static void WriteBody(HttpWebRequest request, string body)
        {
            var pp = new JsonPrettyPrinter(new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext());
            Console.WriteLine("[Body] " );
            Console.WriteLine( pp.PrettyPrint(body) );

            byte[] data = Encoding.ASCII.GetBytes(body);
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
            string method = "GET";
            var request = CreateRequest(method, uri);

            bool isNeededReAuth = false;
            var response = ReadResponse<T>(request, out isNeededReAuth);

            if (isNeededReAuth)
            {
                Console.WriteLine();
                Console.WriteLine("Trying to re-authenticate using the same credentials.");

                // it's needed to authorize again
                // trying to do it and then re-send the initial request
                OAuth.OAuth.refreshToken();

                Console.WriteLine();
                if (!APIContext.Authenticated)
                {
                    Console.WriteLine("The OAuth authentication has failed, GET request can't be performed.");
                    return default(T);
                }

                Console.WriteLine("Authentication was successful. Trying to send GET request again for the last time.");

                request = CreateRequest(method, uri);
                response = ReadResponse<T>(request);
            }

            return response;
        }


        /// <summary>
        /// Create POST HTTP request to url with body and return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <param name="body">The request body.</param>
        /// <param name="isMachineAuthentication">Set to true if machine authentication is reqired instead of regular user authentication.
        /// As a result different headers will be used</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpPost<T>(string uri, string body, string contentType, bool isFirstAuthCall = false, bool isMachineAuthentication = false, string bearer = null)
        {
            string method = "POST";
            var request = CreateRequest(method, uri, isFirstAuthCall, isMachineAuthentication, bearer: bearer);

            request.ContentType = contentType;
            WriteBody(request, body);

            bool isNeededReAuth = false;
            var response = ReadResponse<T>(request, out isNeededReAuth);

            if (!isFirstAuthCall && isNeededReAuth)
            {
                Console.WriteLine();
                Console.WriteLine("Trying to re-authenticate using the same credentials.");

                // it's needed to authorize again
                // trying to do it and then re-send the initial request
                OAuth.OAuth.refreshToken();

                Console.WriteLine();
                if (!APIContext.Authenticated)
                {
                    Console.WriteLine("The OAuth authentication has failed, POST request can't be performed.");
                    return default(T);
                }

                Console.WriteLine("Authentication was successful. Trying to send POST request again for the last time.");

                request = CreateRequest(method, uri, isFirstAuthCall, isMachineAuthentication);
                request.ContentType = contentType;
                WriteBody(request, body);

                response = ReadResponse<T>(request);
            }

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
            return HttpPost<T>(uri, Serialization.JSONSerizalize(body), JSON_CONTENT_TYPE);
        }

        protected static TResult HttpPost<TResult, TBody>(string uri, TBody body, string bearer = null)
            where TResult : class
            where TBody : class
        {
            return HttpPost<TResult>(uri, Serialization.JSONSerizalize(body), JSON_CONTENT_TYPE, bearer: bearer);
        }


        /// <summary>
        /// Create DELETE HTTP request to url return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        protected static T HttpDelete<T>(string uri, object body = null)
        {
            string method = "DELETE";
            var request = CreateRequest(method, uri);
            if (body != null)
            {
                request.ContentType = JSON_CONTENT_TYPE;
                WriteBody(request, Serialization.JSONSerizalize(body));
            }
            bool isNeededReAuth = false;
            var response = ReadResponse<T>(request, out isNeededReAuth);

            if (isNeededReAuth)
            {
                Console.WriteLine();
                Console.WriteLine("Trying to re-authenticate using the same credentials.");

                // it's needed to authorize again
                // trying to do it and then re-send the initial request
                OAuth.OAuth.refreshToken();

                Console.WriteLine();
                if (!APIContext.Authenticated)
                {
                    Console.WriteLine("The OAuth authentication has failed, DELETE request can't be performed.");
                    return default(T);
                }

                Console.WriteLine("Authentication was successful. Trying to send DELETE request again for the last time.");

                request = CreateRequest(method, uri);
                response = ReadResponse<T>(request);
            }

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
            string method = "PUT";
            var request = CreateRequest(method, uri);
            request.ContentType = contentType;
            WriteBody(request, body);
            
            bool isNeededReAuth = false;
            var response = ReadResponse<T>(request, out isNeededReAuth);

            if (isNeededReAuth)
            {
                Console.WriteLine();
                Console.WriteLine("Trying to re-authenticate using the same credentials.");

                // it's needed to authorize again
                // trying to do it and then re-send the initial request
                OAuth.OAuth.refreshToken();

                Console.WriteLine();
                if (!APIContext.Authenticated)
                {
                    Console.WriteLine("The OAuth authentication has failed, PUT request can't be performed.");
                    return default(T);
                }

                Console.WriteLine("Authentication was successful. Trying to send PUT request again for the last time.");

                request = CreateRequest(method, uri);
                request.ContentType = contentType;
                WriteBody(request, body);

                response = ReadResponse<T>(request);
            }

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
            return HttpPut<T>(uri, Serialization.JSONSerizalize(body), JSON_CONTENT_TYPE);
        }

        #endregion Protected Methods
    }
}