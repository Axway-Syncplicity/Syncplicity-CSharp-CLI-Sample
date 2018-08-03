using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CSharpSampleApp.Entities.GOL;
using CSharpSampleApp.Util;
using JsonPrettyPrinterPlus;
using Newtonsoft.Json;

namespace CSharpSampleApp.Services
{
    public static class GolGateway
    {
        private static Rol[] _rols;
        public static Rol CurrentRol { get; private set; }

        public static void LoadRols(string email)
        {
            var uhid = HmacMd5(email);
            var url = $"{ConfigurationHelper.GolUrl}/api/v1/rols?uhid={uhid}";
            _rols = HttpGet<Rol[]>(url);
            CurrentRol = _rols.Single(x => x.IsHome);
        }

        public static string OAuthRevokeTokenUrl => $"{BaseApiEndpointUrl}/oauth/revoke";

        public static string OAuthTokenUrl => $"{BaseApiEndpointUrl}/oauth/token";

        public static string BaseApiEndpointUrl => CurrentRol.ApigeeUrl;

        /// <summary>
        /// Gets admin token for the home role
        /// </summary>
        /// <returns></returns>
        public static string SyncplicityAdminToken => ConfigurationHelper.GetSyncplicityAdminToken(CurrentRol.Id);

        /// <summary>
        /// Create GET HTTP request to url return deserialized object T.
        /// </summary>
        /// <typeparam name="T">The type of returned object.</typeparam>
        /// <param name="uri">The request url.</param>
        /// <returns>The object representation of received response or default of T if response is empty.</returns>
        private static T HttpGet<T>(string uri)
        {
            const string method = "GET";
            var request = CreateRequest(method, uri);
            return ReadResponse<T>(request);
        }

        /// <summary>
        /// Creates request object.
        /// </summary>
        /// <param name="method">The request's method.</param>
        /// <param name="uri">The url of request.</param>
        /// <returns>Created request.</returns>
        private static HttpWebRequest CreateRequest(string method, string uri)
        {
            Console.WriteLine("Creating {0} request to {1}", method.ToUpper(), uri);

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = method.ToUpper();
            request.Accept = "application/json";
            request.CookieContainer = new CookieContainer();
            request.Timeout = 15000;

            return request;
        }

        private static T ReadResponse<T>(WebRequest request)
        {
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
                                return JsonSerializer.Create(new JsonSerializerSettings())
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

                Console.WriteLine("Error {1} {2} occurs during request to {0}.", response.ResponseUri, (int)response.StatusCode, response.StatusDescription);

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

        public static void SwitchRol()
        {
            CurrentRol = _rols.First(x => x.Id != CurrentRol.Id);

            Console.WriteLine("ROL has been switched to '{0}'", CurrentRol.Name);
        }

        public static void SwitchRol(int id)
        {
            CurrentRol = _rols.First(x => x.Id == id);

            Console.WriteLine("ROL has been switched to '{0}'", CurrentRol.Name);
        }

        public static string HmacMd5(string value, string salt = "6adebb9f-21f9-49d8-95bf-b7007a208cd4")
        {
            var hmacMd5 = new HMACMD5(Encoding.UTF8.GetBytes(salt));

            var email = value.ToLower().Trim();

            // step 1, calculate MD5 hash from input
            var hash = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(email));

            // step 2, get MD5 hash as string
            var sb = new StringBuilder();
            hash.ToList().ForEach(h => sb.Append(h.ToString("X2")));

            return sb.ToString().ToLower();
        }
    }
}
