using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Http;
using System.Web.UI;
using Selectcon.Models;
using System.Data;

namespace Selectcon
{
    public class Project
    {
        public enum RoleType : int {
            Admin = 1,
            SuperUser = 2,
            User = 3
        }

        public static dbContext dal = new dbContext();
        public static Encryptions enc = new Encryptions();
        public static SendMail mail = new SendMail();
        public static string DefaultAvatar;
        public static bool IsAdmin, IsSuperUser;
        public static string MailMode, SenderMail, SenderFrom, TestMail;
        public static string smtpHost, smtpPort, smtpUser, smtpPassword;
        public static string SiteKey, PrivateKey;

        public static Dictionary<string, string> config = new Dictionary<string, string>();

        public static bool Initialize() {
            try
            {
                ReadConfig();
                DefaultAvatar = HttpContext.Current.Server.MapPath(getConfig("DefaultAvatar"));
                MailMode = getConfig("MailMode");
                SenderMail = getConfig("SenderMail");
                SenderFrom = getConfig("SenderFrom");
                TestMail = getConfig("TestMail");
                smtpHost = getConfig("smtpHost");
                smtpPort = getConfig("smtpPort");
                smtpUser = getConfig("smtpUser");
                smtpPassword = getConfig("smtpPassword");
                SiteKey = getConfig("SiteKey");
                PrivateKey = getConfig("PrivateKey");
                IsAdmin = (IsMe().RoleID == (int)RoleType.Admin);
                IsSuperUser = (IsMe().RoleID == (int)RoleType.SuperUser);

                return true;
            }
            catch (Exception) {
                return false;
            }
            
            
        }

        public static string getConfig(string key) {
            if (config.Count == 0) {
                Project.ReadConfig();
            }
            return config.FirstOrDefault(r => r.Key.ToUpper() == key.ToUpper()).Value;
        }

        public static void ReadConfig() {
            DataTable dt = null;
            string sql = "";
            try {
                sql = "SELECT * FROM SYS_CONFIGS ";
                dt = Project.dal.QueryData(sql);
                if (dt != null && dt.Rows.Count > 0) {
                    foreach (DataRow dr in dt.Rows) {
                        config.Add(dr["CfgKey"] + "", dr["CfgValue"] + "");
                    }
                }

            } catch (Exception ex) {
                Project.GetErrorMessage(ex);
            }
        }

        public static void GetErrorMessage(string msg, string sql = "") {
            //write log 
            string sql2 = "INSERT INTO SYS_LOGS (Categories, Details) VALUES ('sql', N'" + sql + "')";
            Project.dal.ExecuteSQL(sql2);
        }

        public static void GetErrorMessage(Exception msg)
        {
            //write log 
        }

        public static Membership IsMe() {
            Membership m = new Membership();
            if (HttpContext.Current.Session["ISME"] != null)  {
                m = (Membership)HttpContext.Current.Session["ISME"];
            }

            return m;
            
        }

        public static void RunScript(Page page, string jsScript, string key = "showError")
        {
            ScriptManager.RegisterStartupScript(page, page.GetType(), key, jsScript, true);
        }

        public static string GetIp() {
            string ipaddr = "";
            try
            {
                ipaddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] + "";
                if (string.IsNullOrEmpty(ipaddr)) {
                    ipaddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "";
                }

            } catch (Exception ex) {
                ipaddr = "";
            }
            return ipaddr;
        }

    }
}