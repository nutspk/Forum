using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Selectcon.Models;
namespace Selectcon
{
    public partial class Main : System.Web.UI.MasterPage
    {
 
        public bool IsAdmin = false;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (Validations.GetParamStr("action") == "LOGOUT") {
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();
                Response.Redirect("~/area/home.aspx");
            }

            if (!IsPostBack) {
                
            }

            Init();

        }

        private void Init() {
            Membership member = (Membership)Session["ISME"];
            try {

                pnlLogin.Visible = (member == null || member.UserName == "");
                pnlProfile.Visible = !pnlLogin.Visible;
                btnAsk.Visible = pnlLogin.Visible;
                if (member != null) {
                    IsAdmin = (member.RoleID == 1);
                    UserImage.Src = @"../Files/Upload/Avatar/" + member.Avatar;
                }
            }
            catch (Exception ex) {
                Project.GetErrorMessage(ex);
            }
        }
    }
}