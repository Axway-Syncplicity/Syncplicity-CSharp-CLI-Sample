using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CSharpSampleApp.OAuth
{
    [DataContract]
    public class TokenResponse
    {
        #region Public Properties

        [DataMember(Name = "access_token"), JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "refresh_token"), JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "user_email"), JsonProperty("user_email")]
        public string UserEmail { get; set; }

        [DataMember(Name = "user_company_id"), JsonProperty("user_company_id")]
        public Guid? CompanyGuid { get; set; }

        [DataMember(Name = "issued_at"), JsonProperty("issued_at")]
        public long IssuedAt { get; set; }

        #endregion Public Properties
    }
}