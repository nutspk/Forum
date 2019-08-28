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

namespace Selectcon.Controller
{
    public class TagsController : ApiController
    {
        public HttpResponseMessage Msg;
        Tags Tag = null;
        Membership IsMe = null;


        public TagsController()
        {
            Tag = new Tags();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        // GET api/<controller>
        [GET("api/v1/Tags")]
        public HttpResponseMessage Get()
        {
            try {

                var allTags = Tag.GetTagData();
                if (allTags == null) {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(allTags));
            }catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            
        }


        // POST api/<controller>
        [POST("api/v1/Tags/")]
        public HttpResponseMessage Post([FromBody]string value)
        {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, Tag);
            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        // PUT api/<controller>/5
        [PUT("api/v1/Tags/")]
        public HttpResponseMessage Put(int id, [FromBody]string value)
        {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, Tag);
            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

    }
}