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
    public class CategoriesController : ApiController
    {
        public HttpResponseMessage Msg;
         Questions question = null;
        Categories Cate = null;
        Membership IsMe = null;


        public CategoriesController()
        {
            Cate = new Categories();
            question = new Questions();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        // GET api/<controller>
        [GET("api/v1/Categories")]
        public HttpResponseMessage Get()
        {
            try {
                var allCate = Cate.GetCategories().Where(r=>r.ActiveFlag == 1);

                if (allCate == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else {
                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(allCate));
                }

            }catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            
        }

        [GET("api/v1/Categories/{id}")]
        public HttpResponseMessage GetQuestionByCate(string id)
        {
            IList<Questions> q = null;
            try
            {
                    if (IsMe.RoleID == (int)Project.RoleType.User || IsMe.RoleID <= 0) {
                         q = question.GetQuestionByUser(IsMe.UserID, id);
                    } else {
                         q = question.GetQuestionData("", id);
                    }

                if (q != null) {
                    q = q.Select(r => new Questions
                    {
                        QuestionID = r.QuestionID,
                        CategoryID = r.CategoryID,
                        CategoryName = r.CategoryName,
                        QuestionTitle = r.QuestionTitle,
                        QuestionDesc = r.QuestionDesc,
                        UserUpdated = r.UserUpdated,
                        DateUpdated = r.DateUpdated,
                        IpAddress = r.IpAddress,
                        TagList = r.TagList,
                        LikeCnt = r.LikeCnt,
                        AnswerCnt = r.AnswerCnt,
                        ViewCnt = r.ViewCnt,
                        IsPublic = r.IsPublic
                    }).ToList<Questions>();

                    Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(q));
                }
                        
                
            }
            catch (Exception ex)
            {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;

        }


        // POST api/<controller>
        [POST("api/v1/Categories/")]
        public HttpResponseMessage Post([FromBody]string value)
        {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, Cate);
            } catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }

        // PUT api/<controller>/5
        [PUT("api/v1/Category/")]
        public HttpResponseMessage Put([FromBody]Categories value)
        {
            try {

                if (!Project.IsAdmin) {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                if (value == null || value.CategoryName == "") {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    if (Cate.MngCategoryData(dbUtilities.opUPDATE, value)) {
                        Msg = Request.CreateResponse(HttpStatusCode.OK);
                        } else {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

                Msg = Request.CreateResponse(HttpStatusCode.OK, Cate);
            } catch (Exception) {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Msg;
        }

       

    }
}