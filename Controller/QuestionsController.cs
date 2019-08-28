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

namespace Selectcon
{
    public class QuestionsController : ApiController
    {
        public HttpResponseMessage Msg;
        Questions Question = null;
        Membership IsMe = null;


        public QuestionsController() {
            Question = new Questions();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        [GET("api/v1/Question")]
        public HttpResponseMessage GetAllQuestions()
        {

            IList<Questions> q = null;
            try {

                if (IsMe.RoleID == (int)Project.RoleType.User) {
                    q = Question.GetQuestionByUser(IsMe.UserID);
                } else if (IsMe.RoleID == -1) {
                    q = Question.GetQuestionData().Where(r=>r.IsPublic == 1).Select(r=>r).ToList();
                } else {
                    q = Question.GetQuestionData();
                }

                if (q == null) {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    q = q.Select(r => new Questions
                    {
                        QuestionID = r.QuestionID,
                        CategoryID = r.CategoryID,
                        QuestionTitle = r.QuestionTitle,
                        QuestionDesc = r.QuestionDesc,
                        UserUpdated = r.UserUpdated,
                        DateUpdated = r.DateUpdated,
                        IpAddress = r.IpAddress,
                        TagList = r.TagList,
                        LikeCnt = r.LikeCnt,
                        AnswerCnt = r.AnswerCnt,
                        ViewCnt = r.ViewCnt,
                        IsPublic = r.IsPublic,
                        UserID = r.UserID
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

        [GET("api/v1/Question/{key}")]
        public HttpResponseMessage GetQuestionByID(int key)
        {
            try
            {
                //QuestionID QuestionDesc    UserUpdated DateUpdated IpAddress TagList LikeCnt AnswerCnt   Viewer
                var q = Question.GetQuestionData(Utilities.ToString(key));
                var gallery = new ImageGallery();
                
                Questions newQuestion = q.FirstOrDefault();

                if (IsMe.UserName != "") {
                    Question.MngQuestionData(dbUtilities.opUPDATE,ref newQuestion, true);
                }

                q = q.Select(r => new Questions
                {
                    QuestionID = r.QuestionID,
                    CategoryID = r.CategoryID,
                    QuestionTitle = r.QuestionTitle,
                    QuestionDesc = r.QuestionDesc,
                    UserID = r.UserID,
                    UserUpdated = r.UserUpdated,
                    Avatar = r.Avatar,
                    DateUpdated = r.DateUpdated,
                    IpAddress = r.IpAddress,
                    TagList = r.TagList,
                    IsPublic = r.IsPublic,
                    GalleyList = (r.IsPublic == 0) ? gallery.GetGalleryData(Utilities.ToString(r.QuestionID),"", true) : null,
                    UserList = r.UserList,
                    LikeCnt = r.LikeCnt,
                    AnswerCnt = r.AnswerCnt,
                    ViewCnt = r.ViewCnt,
                }).ToList<Questions>();

                Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(q));
            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [POST("api/v1/Question")]
        public HttpResponseMessage Post([FromBody]Questions value)
        {
            try
            {
                var token = Request.Headers.Authorization.Parameter;

                if (value == null || (IsMe != null && IsMe.UserID == ""))
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else if (!reCaptchaController.IsReCaptchValid(token)) {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
                } else {
                    if (Question.MngQuestionData(dbUtilities.opINSERT, ref value, true)) {
                        var qID = value.QuestionID;
                        Question.MngQuestionUser(dbUtilities.opDELETE, qID);
                        if (value.UserList != null) {
                            foreach (Membership mb in value.UserList)
                            {
                                Question.MngQuestionUser(dbUtilities.opINSERT, qID, mb.UserID);
                            }
                        }
                        

                        Msg = Request.CreateResponse(HttpStatusCode.Created, value);
                    }
                    else
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }


            } catch (Exception){
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [POST("api/v1/Like")]
        public HttpResponseMessage Likes([FromBody]Actions value)
        {
            try
            {

                if ((IsMe != null && IsMe.UserID == "")) {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
                } else if (value != null && value.QuestionID > 0) {

                    if (value.MngActionData(dbUtilities.opINSERT, value.QuestionID+"", "0", value.Likes+""))
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.OK);
                    } else {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }


            } catch (Exception) {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [PUT("api/v1/Question/edit/{key:int}")]
        public HttpResponseMessage Put(int key, [FromBody]Questions value)
        {
            int op = 0;
            try
            {

                var token = Request.Headers.Authorization.Parameter;

                if (value == null || (IsMe != null && IsMe.UserID == "")) {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                }else if (!reCaptchaController.IsReCaptchValid(token)) {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
                } else {
                    value.QuestionID = key;

                    var log = new PostLogs();
                    log.QuestionID = key;
                    log.OriginalTitile = value.QuestionTitle;
                    log.OriginalDesc = value.QuestionDesc;

                    if (Question.MngQuestionData(dbUtilities.opUPDATE, ref value, false))
                    {
                        log.MngLogData(dbUtilities.opINSERT, log);

                        var qID = value.QuestionID;
                        Question.MngQuestionUser(dbUtilities.opDELETE, qID);
                        foreach (Membership mb in value.UserList)
                        {
                            Question.MngQuestionUser(dbUtilities.opINSERT, qID, mb.UserID);
                        }

                        Msg = Request.CreateResponse(HttpStatusCode.Created, value);
                    }
                    else
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }

            }
            catch (Exception)
            {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        // DELETE api/<controller>/5
        [DELETE("api/v1/Question/{id}")]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                if (id == "")
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    var q = Question.GetQuestionData(id).FirstOrDefault();
                    if (q != null)
                    {
                        var a = (Membership)HttpContext.Current.Session["ISME"];
                        if (a.UserID == q.UserID || Project.IsAdmin)
                        {
                            q.ActiveFlag = 0;
                            Question.MngQuestionData(dbUtilities.opUPDATE, ref q, false);
                            Msg = Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else {
                            Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                        }
                    }
                    else {
                        Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                    }
                }
            }
            catch (Exception)
            {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }
    }
}