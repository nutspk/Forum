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
using System.Threading.Tasks;
using System.IO;

namespace Selectcon.Controller
{
    public class FileuploadController : ApiController
    {
        public HttpResponseMessage Msg;

        public class Base64Image {
            [JsonProperty("base64enc")]
            public string base64enc { get; set; }
        }

        [POST("api/v1/Upload/{base64enc}")]
        public bool foo(Base64Image img)
        {
            bool ret = false;
            try
            {
                var bytes = Convert.FromBase64String(img.base64enc);

                using (var imageFile = new FileStream(@"~/Files/Upload/Avatar", FileMode.CreateNew))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                throw ex;
            }
           
            return ret;
        }



    }
}