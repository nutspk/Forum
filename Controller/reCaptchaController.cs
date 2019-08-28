using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Selectcon.Models;
using AttributeRouting.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.IO;

namespace Selectcon
{
    public class reCaptchaController : ApiController
    {
  
        public class reCaptcha {
            [JsonProperty(PropertyName = "token")]
            public string token { get; set; }
        }

        public class reCaptchaResponse {
            [JsonProperty(PropertyName = "success")]
            public bool success { get; set; } = false;
            [JsonProperty(PropertyName = "challenge_ts")]
            public string challenge_ts { get; set; }
            [JsonProperty(PropertyName = "hostname")]
            public string hostname { get; set; }
            [JsonProperty(PropertyName = "error-codes")]
            public string errorcodes { get; set; }
        }

        public static bool IsReCaptchValid(string token)
        {
            var resp = new reCaptchaResponse() ;
            var client = new System.Net.WebClient();
            try
            {
                if (token == "") {
                    return false;
                }

                var responseKey = token;
                var secretKey = Project.PrivateKey;
                var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
                var requestUri = string.Format(apiUrl, secretKey, responseKey);

                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                var GoogleReply = client.DownloadString(requestUri);
                resp = JsonConvert.DeserializeObject<reCaptchaResponse>(GoogleReply);

            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
            }
            return resp.success;
        }


        
    }
}