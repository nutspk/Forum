using Selectcon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Selectcon.Members
{
    public partial class Confirm : System.Web.UI.Page
    {
        public string SiteKey = Project.getConfig("SiteKey");
        public string type = "";
        public string value = "";
        public string key = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                type = Request.QueryString["k"] + "";
                value = Request.QueryString["val"] + "";
                key = Request.QueryString["reCaptchaToken"] + "";

            } catch (Exception){
                
            }
            
        }

        private void Register()
        {
            string[] param;
            Membership mb = new Membership();
            string email = "", hash = "";
            try
            {
                param = value.Split('|');
                if (param.Length == 2) {
                    email = param[0];
                    hash = param[1];
                }
                
                if (Project.enc.VerifyPassword(email, hash)) {
                    mb = mb.GetUserData("", email).FirstOrDefault();

                    if (mb != null && mb.UserID != "")
                    {
                        mb.IsApproved = 1;
                        mb.MngUserData(dbUtilities.opUPDATE, mb);
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }

        private void Recovery()
        {
            string[] param;
            Membership mb = new Membership();
            string email = "", hash = "";
            try
            {
                param = value.Split('|');
                if (param.Length == 2)
                {
                    email = param[0];
                    hash = param[1];
                }

                if (Project.enc.VerifyPassword(email, hash))
                {
                    mb = mb.GetUserData("", email).FirstOrDefault();

                    Session["ISME"] = mb;
                }

            }
            catch (Exception ex)
            {

            }

        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            if (type == "recovery")
            {
                Recovery();
                Response.Redirect("User");
            }
            else if (type == "confirm")
            {
                Register();
                Response.Redirect("Login");
            }
        }
    }
}