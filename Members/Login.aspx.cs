using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Selectcon.Models;

namespace Selectcon.Members
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {

            }

            if (Validations.GetParamStr("action") != "") {
                Logon();
            }
            
        }

        private void Init() {

        }

        private void Logon() {
            string UserName = "", Password="";
            Membership member = new Membership();
            try
            {
                UserName = Validations.GetParamStr("txtUserName");
                Password = Validations.GetParamStr("txtPassword");
                var user = member.GetUserData("", UserName).FirstOrDefault();

                if (user != null)
                {
                    
                    if (Project.enc.VerifyPassword(Password, user.Password))
                    {
                        
                        Session["ISME"] = user;
                        Project.Initialize();
                        var log = new SysLogs();
                        log.MngLogData(dbUtilities.opINSERT, new SysLogs()
                        {
                            Category = "LOGIN",
                        });
                        Response.Redirect("~/area/Home.aspx");
                    } else {
                        Project.RunScript(this, "showError('Username หรือ Password ไม่ถูกต้อง');");
                    }
                }
                
                

            } catch (Exception ex) {
                Project.GetErrorMessage(ex);
            }
        }
    }
}