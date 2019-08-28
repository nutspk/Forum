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
using System.Threading.Tasks;

namespace Selectcon.Controller
{
    public class GalleryController : ApiController
    {
        public HttpResponseMessage Msg;
        ImageGallery Gallery = null;
        Membership IsMe = null;


        public GalleryController()
        {
            Gallery = new ImageGallery();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        // GET api/<controller>
        [GET("api/v1/Galleries")]
        public HttpResponseMessage GetGallery()
        {
            try {

                var allGallery = Gallery.GetGalleryData("");
                if (allGallery == null) {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(allGallery));
            }catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        [GET("api/v1/Gallery/{id}")]
        public HttpResponseMessage GetGalleryByID(string id)
        {
            try{
                List<ImageGallery> allGallery = null;

                if (id != "") {
                    allGallery = Gallery.GetGalleryData(id, "", true);
                }
                
                if (allGallery == null) {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(allGallery));
            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }


        // POST api/<controller>
        [POST("api/v1/Gallery/{id}")]
        public HttpResponseMessage PostFormData(string id)
        {
            var img = new Images();
            string sql = "";
            try {
                if (id != "") {
                    sql = "UPDATE IMAGES SET ActiveFlag=0 WHERE ImageID = " + id;
                    Project.dal.ExecuteSQL(sql);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }


        // PUT api/<controller>/5
        [PUT("api/v1/Tags/")]
        public HttpResponseMessage Put(int id, [FromBody]string value)
        {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, value);
            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

    }
}