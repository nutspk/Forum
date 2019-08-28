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
    public class AnswerController : ApiController
    {
        public HttpResponseMessage Msg;
        Answers Answer = null;
        Membership IsMe = null;

        public AnswerController() {
            Answer = new Answers();
            IsMe = (Membership)HttpContext.Current.Session["ISME"];
            if (IsMe == null) IsMe = new Membership();
        }

        [GET("api/v1/Answer/{key}")]
        public HttpResponseMessage GetAnswerByID(int key)
        {
            try
            {
                var Ans = Answer.GetAnswerData(Utilities.ToString(key));

                Ans = Ans.Select(r => new Answers
                {
                    QuestionID = r.QuestionID,
                    AnswerID = r.AnswerID,
                    AnswerDesc = (r.ActiveFlag != 0) ? r.AnswerDesc : "ความคิดเห็นนี้ถูกลบโดย Admin",
                    ActiveFlag = r.ActiveFlag,
                    IpAddress = r.IpAddress,
                    ReplyToID = r.ReplyToID,
                    UserID = r.UserID,
                    UserUpdated = r.UserUpdated,
                    Avatar = r.Avatar,
                    DateUpdated = r.DateUpdated
                }).ToList<Answers>();

                Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(Ans));
            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        // POST api/<controller>
        [POST("api/v1/Answer")]
        public  HttpResponseMessage Post([FromBody]Answers value)
        {
            try
            {
                if (value == null || (IsMe != null && IsMe.UserID == ""))
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else
                {
                    if (Answer.MngAnswerData(dbUtilities.opINSERT, value))
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.Created);
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

        // PUT api/<controller>/5
        [PUT("api/v1/Answer/edit/{key}")]
        public HttpResponseMessage Put(int key, [FromBody]Answers value)
        {
            int op = 0;
            try
            {
                if (key <= 0 && value == null) {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    op = (key == 0) ? dbUtilities.opINSERT : dbUtilities.opUPDATE;
                    var ans = Answer.GetAnswerData("",Utilities.ToString(key)).FirstOrDefault();

                    if (ans != null && ans.AnswerID != 0)
                    {
                        if (Answer.MngAnswerData(op, value))
                        {
                            var log = new PostLogs();
                            log.QuestionID = ans.QuestionID;
                            log.AnswerID = ans.AnswerID;
                            log.OriginalDesc = ans.AnswerDesc;

                            Msg = Request.CreateResponse(HttpStatusCode.OK);
                            
                            log.MngLogData(dbUtilities.opINSERT, log);
                        }
                        else
                        {
                            Msg = Request.CreateResponse(HttpStatusCode.NotModified);

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

        // DELETE api/<controller>/5
        [DELETE("api/v1/Answer/{key:int}")]
        public HttpResponseMessage Delete(int key)
        {
            var Ans = new Answers();
            try
            {
                if (key == 0)
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else
                {
                    if (Project.IsAdmin)
                    {
                        Ans.AnswerID = key;
                        if (Answer.MngAnswerData(dbUtilities.opDELETE, Ans))
                        {
                            Msg = Request.CreateResponse(HttpStatusCode.Created);
                        }
                        else
                        {
                            Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.NonAuthoritativeInformation);
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