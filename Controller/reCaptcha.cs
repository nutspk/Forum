using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Selectcon.Models;
using AttributeRouting.Web.Http;
using Newtonsoft.Json;
using System.Web;
using System.IO;

namespace Selectcon
{
    public class reCaptchaController : ApiController
    {
        public HttpResponseMessage Msg;


        public class reCaptcha
        {
            [JsonProperty(PropertyName = "token")]
            public string token { get; set; }
        }

        public class reCaptchaResponse
        {
            [JsonProperty(PropertyName = "success")]
            public bool success { get; set; } = false;
            [JsonProperty(PropertyName = "challenge_ts")]
            public string challenge_ts { get; set; }
            [JsonProperty(PropertyName = "hostname")]
            public string hostname { get; set; }
            [JsonProperty(PropertyName = "error-codes")]
            public string errorcodes { get; set; }
        }

        [POST("api/v1/reCaptcha/")]
        public HttpResponseMessage IsReCaptchValid([FromBody] reCaptcha re)
        {
            string result = "";
            var resp = new reCaptchaResponse();
            try
            {
                if (re.token == "") {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                var responseKey = re.token;
                var secretKey = Project.PrivateKey;
                var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
                var requestUri = string.Format(apiUrl, secretKey, responseKey);

                var request = (HttpWebRequest)WebRequest.Create(requestUri);

                using (WebResponse response = request.GetResponse()) {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                        resp = JsonConvert.DeserializeObject<reCaptchaResponse>(stream.ReadToEnd());
                    }
                }

                Msg = Request.CreateResponse(HttpStatusCode.OK, resp);
            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

    }
}