using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.FriendlyUrls;
using Selectcon.Models;

namespace Selectcon.Members
{
    public partial class User : System.Web.UI.Page
    {
        public string SiteKey = Project.getConfig("SiteKey");
        public string action;
        public string UserID = "";
        public Membership member;

        protected void Page_Load(object sender, EventArgs e) {

            
            action = Validations.GetParamStr("action");
            UserID = Validations.GetParamStr("k");
            member = (Membership)Session["ISME"];

            if (member != null && member.UserID != "") {
                if (UserID == member.UserID) UserID = "me";
            }

            if (UserID == "") UserID = "me";


            if (!IsPostBack) {
                Init();
            } else {

            }

           

            if (action != "") {
                UploadImage();
            }
        }

        private void Init() {
            try {
                liEdit.Visible = (UserID.ToUpper() == "ME");
                edit.Visible = liEdit.Visible;

            } catch (Exception ex) {
                Project.GetErrorMessage(ex);
            }
        }

        private void UploadImage() {
            
            try
            {
                if (member != null && member.UserName != "") {
                    string FileType = "|.jpg|.jpeg|.png|.gif|";
                    string strpath = "";

                    if (FileUpload.HasFile)
                    {
                        strpath = System.IO.Path.GetExtension(FileUpload.FileName);
                        if (FileType.IndexOf(strpath.ToLower()) >= 0)
                        {
                            FileUpload.SaveAs(Server.MapPath("~/Files/Upload/Avatar/" + member.UserName+strpath));
                            member.Avatar = member.UserName + strpath;
                            member.MngUserData(dbUtilities.opUPDATE, member);
                            Session["ISME"] = member;
                        }
                    }
                }
                
            } catch (Exception ex) {

                throw ex;
            }
        }
    }
}