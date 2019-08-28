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
    public class LogController : ApiController
    {
        public HttpResponseMessage Msg;
        PostLogs Log = null;
        Membership IsMe = null;

        public LogController() {
            Log = new PostLogs();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        [GET("api/v1/edit/")]
        public HttpResponseMessage GetLogByID(string qID="", string aID = "")
        {
            try
            {
                var log = Log.GetLogData(qID, aID);

                log = log.Select(r => new PostLogs
                {
                    QuestionID = r.QuestionID,
                    AnswerID = r.AnswerID,
                    OriginalTitile = r.OriginalTitile,
                    OriginalDesc = r.OriginalDesc,
                    UserID = r.UserID,
                    UserUpdated = r.UserUpdated,
                    DateUpdated = r.DateUpdated
                }).ToList<PostLogs>();

                Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(log));
            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        // POST api/<controller>
        [POST("api/v1/edit")]
        public  HttpResponseMessage Post([FromBody]PostLogs value)
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
                    var questionID = value.QuestionID;
                    var answerID = value.AnswerID;
                    if (answerID == null || answerID == 0) {
                        var q = (new Questions()).GetQuestionData(Utilities.ToString(questionID)).FirstOrDefault();
                        if (q == null) q = new Questions();
                        q.QuestionID = questionID;
                        q.QuestionTitle = value.OriginalTitile;
                        q.QuestionDesc = value.OriginalDesc;
                        bool stamp = (IsMe.UserID == q.UserID);
                        q.MngQuestionData(dbUtilities.opUPDATE, ref q, stamp);
                    } else {
                        var a = (new Answers()).GetAnswerData(Utilities.ToString(questionID), Utilities.ToString(answerID)).FirstOrDefault();
                        if (a == null) a = new Answers();
                        a.QuestionID = questionID;
                        a.AnswerID = Utilities.ToInt(answerID);
                        a.AnswerDesc = value.OriginalDesc;
                        bool stamp = (IsMe.UserID == a.UserID);
                        a.MngAnswerData(dbUtilities.opUPDATE, a, stamp);
                    }

                    if (Log.MngLogData(dbUtilities.opINSERT, value))
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.Created);
                    } else {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }


            } catch (Exception){
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [POST("api/v1/del")]
        public HttpResponseMessage Del([FromBody]PostLogs value)
        {
            try
            {
                var token = Request.Headers.Authorization.Parameter;

                if (value == null || (IsMe != null && IsMe.UserID == ""))
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else if (!reCaptchaController.IsReCaptchValid(token))
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var questionID = value.QuestionID;
                    var answerID = value.AnswerID;
                    if (answerID == null || answerID == 0)
                    {
                        //var q = (new Questions()).GetQuestionData(Utilities.ToString(questionID)).FirstOrDefault();
                        //if (q == null) q = new Questions();
                        //q.QuestionID = questionID;
                        //q.QuestionTitle = value.OriginalTitile;
                        //q.QuestionDesc = value.OriginalDesc;
                        //bool stamp = (IsMe.UserID == q.UserID);
                        //q.MngQuestionData(dbUtilities.opUPDATE, ref q, stamp);
                    }
                    else
                    {
                        var a = (new Answers()).GetAnswerData(Utilities.ToString(questionID), Utilities.ToString(answerID)).FirstOrDefault();
                        if (a == null) a = new Answers();
                        a.QuestionID = questionID;
                        a.AnswerID = Utilities.ToInt(answerID);
                        a.ActiveFlag = 0;
                        bool stamp = (IsMe.UserID == a.UserID);
                        a.MngAnswerData(dbUtilities.opUPDATE, a, stamp);
                    }

                    if (Log.MngLogData(dbUtilities.opINSERT, value))
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.Created);
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
    }
}