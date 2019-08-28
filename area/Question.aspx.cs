using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Selectcon.Models;
using System.IO;
using Selectcon.Models;


namespace Selectcon
{
    public partial class Question : System.Web.UI.Page
    {
        public string SiteKey = Project.SiteKey;
        public string UserID;
        public bool Owner = false;
        public bool IsPrivate = false;
        public string action = "";
        public bool IsAdmin = false;
        protected void Page_Load(object sender, EventArgs e)
        {

            qID.Value = Request.QueryString["k"];
            action = Validations.GetParamStr("action");
            IsAdmin = Project.IsAdmin;

            if (!IsPostBack) {
                
            }

            Init();

            if (action == "CREATE") {
                CreateGallery();
            }

        }

        private void Init() {
            try
            {
                var me = (Membership)Session["ISME"];
                Questions q = new Questions();
                var question =  q.GetQuestionData(qID.Value).FirstOrDefault();
                string userList = "";

                if (question != null) {

                    Owner = ((me != null && me.UserID == question.UserID) || Project.IsAdmin) ? true : false;
                    

                    userList = string.Join(",", question.UserList.Select(r => r.UserID).ToArray());

                    if (question.IsPublic == 1)
                    {
                        divpost.Visible = (me != null && me.UserID != "");
                    } else {
                        IsPrivate = true;
                        // admin and super user
                        if (me != null)
                        {
                            bool expr1 = (me.UserID != "" && me.RoleID != (int)Project.RoleType.User) && me.RoleID != -1;
                            bool expr2 = ("," + userList + ",").IndexOf(","+me.UserID+",") >= 0;
                            divpost.Visible = (expr1 || expr2);
                        } else {
                            divpost.Visible = false;
                        }

                        if (divpost.Visible == false) {
                            Response.Redirect("../area/home");
                        }
                    }
                }

                UserID = (me != null) ? me.UserID : "";

                btnPost.Visible = divpost.Visible;
                btnDiscard.Visible = divpost.Visible;
                
            } catch (Exception ex) {
                Project.GetErrorMessage(ex);
            }
        }

        private void CreateGallery() {
            var ig = new ImageGallery();
            var im = new Images();
            string desc = "";
            string fileName = "";
            string filePath = "";
            string suffix = "";
            dynamic trans = null;
            dynamic conn = null;
            int op = 0;
            try
            {
                conn = Project.dal.OpenConn("");
                trans = Project.dal.BeginTrans(conn);

                var file = Request.Files;

                desc = Request.Form["txtDesc"] + "";
                

                var gList = ig.GetGalleryData(Request.QueryString["k"]+"");

                if (gList != null && gList.Count > 0) {
                    op = dbUtilities.opUPDATE;
                    ig.GalleryID = gList[0].GalleryID;
                }else {
                    op = dbUtilities.opINSERT;
                }

                ig.QuestionID = Utilities.ToInt(Request.QueryString["k"]);
                ig.GalleryDesc = desc;
                ig.ActiveFlag = 1;

                ig.MngGalleryData(op, ref ig, conn, trans);

                
                List<HttpPostedFile> lstFile = new List<HttpPostedFile>();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var fileUpload = Request.Files[i];
                    if ((fileUpload.ContentType.Contains("image") || fileUpload.ContentType.Contains("video")) && fileUpload.ContentLength > 0)
                    {
                        lstFile.Add(fileUpload);
                    }
                }

                filePath = Server.MapPath("~/Files/Upload/Images/");

                
                for (int i = 0; i < lstFile.Count; i++)
                {
                    suffix = i + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    HttpPostedFile FU = lstFile[i];
                    string ext = Path.GetExtension(FU.FileName);
                    fileName = ig.GalleryID + "_"+ suffix + "_"+ new Random().Next(0, 50) + ext;

                    im.GalleryID = ig.GalleryID;
                    im.ActiveFlag = 1;
                    im.ShowFlag = (i == lstFile.Count-1) ? 1 : 0;
                    im.ImageUrl = fileName;
                    ig.MngImageData(dbUtilities.opINSERT, ref im, conn, trans);

                    FU.SaveAs(filePath + fileName);
                }

                Project.dal.CommitTrans(ref trans);
            }catch (Exception ex){
                Project.dal.RollbackTrans(ref trans);
                throw ex;
            }finally {
                trans = null;
                Project.dal.CloseConn(ref conn);
            }
        }

    }
}