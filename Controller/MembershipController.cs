using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Selectcon.Models;
using AttributeRouting.Web.Http;
using Newtonsoft.Json;

namespace Selectcon.Controller
{
    public class MembershipController : ApiController
    {

        Membership Member = new Membership();
        HttpResponseMessage Msg = new HttpResponseMessage();
        public Membership MyUser = null; 

        public MembershipController() {
            MyUser = (Membership)HttpContext.Current.Session["ISME"];
            if (MyUser == null) MyUser = new Membership();
        }

        public class jsonObj {
            [JsonProperty(PropertyName = "email")]
            public string Email { get; set; }
        }

        [POST("api/v1/Recovery/")]
        public HttpResponseMessage Recovery([FromBody]jsonObj obj)
        {
            Membership user = null;
            try
            {
                if (obj.Email != "") {
                    user = Member.GetUserData("", obj.Email).FirstOrDefault();
                } 
                
                if (user != null)
                {
                    Project.mail.RecoveryMail(user.Email);
                    Msg = Request.CreateResponse(HttpStatusCode.OK, "[{success: true}]");
 
                } else {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden, "[{success: false}]");
                }

                
            }
            catch (Exception ex)
            {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        // GET api/<controller>
        [GET("api/v1/Member/")]
        public HttpResponseMessage GetAllMember()
        {
            try {
                var AllMember = Member.GetUserData();

                AllMember = AllMember.Where(r => r.UserName != MyUser.UserName).Select(r => new Membership
                {
                    UserID = r.UserID,
                    DisplayName = r.DisplayName,
                    Comment = r.Comment,
                    Email = r.Email,
                    Tel = r.Tel,
                    Avatar = r.Avatar,
                    DateCreated = r.DateCreated,
                    LastLogin = r.LastLogin,
                    Address = r.Address,
                    Title = r.Title,
                    RoleName = r.RoleName,
                    RoleID = r.RoleID,
                    IsApproved = r.IsApproved,
                    IsBanned = r.IsBanned,
                    BannedReason = r.BannedReason
                }).OrderBy(r=>r.RoleID).ToList<Membership>();

                if (AllMember.Count >= 0)
                {
                    Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(AllMember));
                }
                else {
                    Msg = Request.CreateResponse(HttpStatusCode.NoContent);
                }
                
            }catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [GET("api/v1/Member/{id}")]
        public HttpResponseMessage GetMember(string id)
        {
            try
            {
 
                if (id.ToLower() == "me") {
                    Msg = me();
                } else {
                    var AllMember = Member.GetUserData(id);
                    if (AllMember == null || AllMember.Count <= 0)
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                    } else {
                        var member = AllMember
                            .Where(r => r.UserName != MyUser.UserName)
                            .Select(r => new Membership
                            {
                                DisplayName = r.DisplayName,
                                Comment = r.Comment,
                                Email = r.Email,
                                Tel = r.Tel,
                                Avatar = r.Avatar,
                                DateCreated = r.DateCreated,
                                LastLogin = r.LastLogin,
                                Address = r.Address,
                                Title = r.Title,
                                RoleID = r.RoleID,
                                IsApproved = r.IsApproved,
                                IsBanned = r.IsBanned,
                                BannedReason = r.BannedReason
                            }).ToList<Membership>().FirstOrDefault();

                        Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(member));
                    }
                }
                
            }
            catch (Exception ex)
            {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        public HttpResponseMessage me()
        {
            try
            {
                var AllMember = Member.GetUserData().Where(r=>r.UserName == MyUser.UserName).FirstOrDefault();

                if (AllMember == null) {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(AllMember));
                }
            } catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Msg;
        }

        [GET("api/v1/User/Login")]
        public HttpResponseMessage GetMe(string UserName, string Password)
        {
            HttpResponseMessage Msg = new HttpResponseMessage();
            try
            {
                var CurrentUser = Member.GetUserData("", UserName).SingleOrDefault();

                if (CurrentUser == null || CurrentUser.UserName == "")
                {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                }else {
                    if (CurrentUser.IsApproved != 1)
                    {
                        Msg = Request.CreateResponse(HttpStatusCode.Unauthorized, "IsApproved != 1");
                    } else if (CurrentUser.IsBanned == 1) {
                        Msg = Request.CreateResponse(HttpStatusCode.NotFound, CurrentUser.BannedReason + "IsBanned == 1");

                    }else if (Project.enc.VerifyPassword(Password, CurrentUser.Password)) {
                        Msg = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(CurrentUser));
                    } else {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
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
        [POST("api/v1/Member/regis")]
        public HttpResponseMessage Post([FromBody]Membership value)
        {
            string encPassword = "";
            try {
                if (value == null) {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                } else {
                    encPassword = Project.enc.HashPassword(value.Password);
                    value.Password = encPassword;

                    var mb = Member.GetUserData("", value.Email).FirstOrDefault();
                    if (mb != null) {
                        value.Email = "";
                        return Request.CreateResponse(HttpStatusCode.BadRequest, value);
                    }

                     mb = Member.GetUserData("", value.UserName).FirstOrDefault();
                    if (mb != null)
                    {
                        value.UserName = "";
                        return Request.CreateResponse(HttpStatusCode.BadRequest, value);
                    }

                    if (Member.MngUserData(dbUtilities.opINSERT, value))
                    {
                        SendMail sm = new SendMail();
                        if (sm.RegisterMail(value.Email) == "")
                        {
                            Msg = Request.CreateResponse(HttpStatusCode.OK, value);
                        }else {
                            Msg = Request.CreateResponse(HttpStatusCode.OK, value.Email);
                        }
                        
                        //var myaccount = Member.GetUserData("", value.UserName).FirstOrDefault();
                        //HttpContext.Current.Session["ISME"] = myaccount;

                        var log = new SysLogs();
                        log.MngLogData(dbUtilities.opINSERT, new SysLogs()
                        {
                            Category = "REGISTER",
                        });

                    } else {
                        Msg = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                
                
            } catch (Exception) {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }


        // PUT api/<controller>/5
        [PUT("api/v1/Member/Me")]
        public HttpResponseMessage Put([FromBody]Membership value)
        {
            try {
                var mb = (Membership)HttpContext.Current.Session["ISME"];
                if (mb != null)
                {
                    mb.DisplayName = value.DisplayName;
                    mb.Title = value.Title;
                    mb.Address = value.Address;
                    mb.Email = value.Email;
                    if (value.Password != "")
                    {
                        mb.Password = Project.enc.HashPassword(value.Password);
                    }
                    mb.Comment = value.Comment;
                }
                else {
                    Msg = Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                if (Member.MngUserData(dbUtilities.opUPDATE, mb)) {
                    Msg = Request.CreateResponse(HttpStatusCode.OK, Member);
                }
               
            } catch (Exception) {
                Msg = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Msg;
        }

        [PUT("api/v1/User")]
        public HttpResponseMessage Put2([FromBody]Membership value)
        {
            try
            {
                if (Project.IsAdmin && value != null) {
                    var targetUser = Member.GetUserData(value.UserID).SingleOrDefault();

                    if (targetUser != null && targetUser.UserID != "")
                    {
                        targetUser.RoleID = value.RoleID;

                        if (value.IsApproved == 1)
                        {
                            targetUser.IsApproved = 1;
                            targetUser.IsBanned = 0;
                            targetUser.BannedReason = " ";
                        } else if(value.IsBanned == 1) {
                            targetUser.IsApproved = 0;
                            targetUser.IsBanned = 1;
                            targetUser.BannedReason = value.BannedReason;
                        }

                        if (Member.MngUserData(dbUtilities.opUPDATE, targetUser))
                        {
                            Msg = Request.CreateResponse(HttpStatusCode.OK);
                        }

                    } else {
                        Msg = Request.CreateResponse(HttpStatusCode.NotFound);
                    }


                } else {
                    Msg = Request.CreateResponse(HttpStatusCode.Unauthorized);
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