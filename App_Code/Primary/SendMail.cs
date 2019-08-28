using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Selectcon
{
    public class SendMail
    {
        public string SendMailData(string Subject, string Message, string Sender, string Receiver, string MailCC = "", string MailBCC = "", string[] Filename = null)
        {
            SmtpClient SMTPMail = null;
            MailMessage objMail = null;
            MailAddress CcMail = null;
            MailAddress Tomail = null;
            MailAddress Frommail = null;

            string MailFrom = "";
            string MailTo = "";
            string ErrMsg = "";
            string[] OneMail;
            string[] OnemailTo;
            MailAddress BCcmail = null;
            string[] OneBccmail;
            int i;
            string TestMsg = "";
            string FN = "";
            string FNlist = "";

            try
            {
                if (Project.MailMode == "0")
                {
                    ErrMsg = "Mail Mode is closed status.";
                }
                else
                {
                    if (Project.MailMode == "1" || Project.MailMode == "3")
                    {
                        MailFrom = Sender == "" ? Project.SenderMail : Sender;
                        if (Project.TestMail != "")
                        {
                            TestMsg += " To : " + Receiver + "<br/>";
                            if (MailCC != "") { TestMsg += " Cc : " + MailCC + " <br/>"; }
                            if (MailBCC != "") { TestMsg += " Bcc : " + MailBCC + " <br/>"; }
                            Message = "This is for test system :  <br/>" + TestMsg + "<br/>" + Message;
                            Subject = "[TEST MAIL] " + Subject;
                            MailTo = Project.TestMail;
                            MailCC = "";
                            MailBCC = "";
                        }
                        else
                        {
                            MailTo = Receiver;
                        }


                        objMail = new MailMessage();
                        if (Project.SenderFrom != "") {
                            Frommail = new MailAddress(MailFrom, Project.SenderFrom);
                        } else {
                            Frommail = new MailAddress(MailFrom);
                        }
                        objMail.From = Frommail;
                        objMail.Subject = Subject;
                        objMail.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                        objMail.Body = Message;
                        objMail.IsBodyHtml = true;
                        if (MailTo != "")
                        {
                            OnemailTo = MailTo.Split(';');
                            for (i = 0; i < OnemailTo.Length; i++)
                            {
                                try
                                {
                                    Tomail = new MailAddress(OnemailTo[i]);
                                    objMail.To.Add(Tomail);
                                }
                                catch { }
                            }
                        }

                        objMail.IsBodyHtml = true;
                        SMTPMail = new SmtpClient();
                        SMTPMail.Host = Project.smtpHost;
                        SMTPMail.EnableSsl = true;


                        if (Project.smtpPort != "") SMTPMail.Port = Utilities.ToInt(Project.smtpPort);

                        if (Project.smtpUser != "")
                        {
                            SMTPMail.Credentials = new System.Net.NetworkCredential(Project.smtpUser, Project.smtpPassword);
                        }

                        SMTPMail.Send(objMail);
                        ErrMsg = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }

            return ErrMsg;
        }

        public string RecoveryMail(string MailTo) {
            string Subject = "Complete Registration With Selectcon Forum";
            string Msg = "";
            string enc = "";
            string hash = "";
            try
            {
                hash = Project.enc.HashPassword(MailTo);

                Msg += "<div width='100%' style='margin:0;padding:0!important;background:#f3f3f5'> ";
                Msg += "<center style='width:100%;background:#f3f3f5'> ";
                Msg += "<div class='' style='max-width:680px;margin:0 auto;'> ";
                Msg += "<table border='0' cellpadding='0' cellspacing='0' role='presentation' style='max-width:680px;width:100%;background-color:#ffffff;'> ";
                Msg += "<tbody> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:20px 30px;text-align:left'> ";
                Msg += "<h1>Confirm your email address on Selectcon forum</h1> ";
                Msg += "<p>Hello! We just need to verify that "+ MailTo + " is your email address ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:15px 15px;font-family:arial,sans-serif;font-size:15px;line-height:21px;color:#54595f;text-align:left'> ";

                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:0 0 15px 0;'> ";
                Msg += "<table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation'> ";
                Msg += "<tbody> ";
                Msg += "<tr> ";
                Msg += "<td style='border-radius:4px;background:#0095ff'> ";
                Msg += "<a ";
                Msg += "style='min-width: 196px;border-top: 13px solid;border-bottom: 13px solid;border-right: 24px solid;border-left: 24px solid;border-color: #2ea664;border-radius: 4px;background-color: #2ea664;color: #ffffff;font-size: 18px;line-height: 18px;word-break: break-word;display: inline-block;text-align: center;font-weight: 900;text-decoration: none!important;' ";
                Msg += "href='https://selectcon.com/bbs/Members/Confirm?k=recovery&val="+ MailTo+ "|" + hash +"'";
                Msg += "> ";
                Msg += "Confirm email address ";
                Msg += "</a> ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "</tbody> ";
                Msg += "</table> ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "</tbody> ";
                Msg += "</table> ";
                Msg += "</div> ";
                Msg += "</center> ";
                Msg += "</div> ";
            }
            catch (Exception ex) {

                throw ex; 
            }
            return SendMailData(Subject, Msg , "", MailTo);
        }

        public string RegisterMail(string MailTo)
        {
            string Subject = "Complete Registration With Selectcon Forum";
            string Msg = "";
            string enc = "";
            string hash = "";
            try
            {
                hash = Project.enc.HashPassword(MailTo);

                Msg += "<div width='100%' style='margin:0;padding:0!important;background:#f3f3f5'> ";
                Msg += "<center style='width:100%;background:#f3f3f5'> ";
                Msg += "<div class='' style='max-width:680px;margin:0 auto;'> ";
                Msg += "<table border='0' cellpadding='0' cellspacing='0' role='presentation' style='max-width:680px;width:100%;background-color:#ffffff;'> ";
                Msg += "<tbody> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:20px 30px;text-align:left'> ";
                Msg += "<h1>Confirm your email address on Selectcon forum</h1> ";
                Msg += "<p>Hello! We just need to verify that " + MailTo + " is your email address ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:15px 15px;font-family:arial,sans-serif;font-size:15px;line-height:21px;color:#54595f;text-align:left'> ";

                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "<tr> ";
                Msg += "<td style='padding:0 0 15px 0;'> ";
                Msg += "<table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation'> ";
                Msg += "<tbody> ";
                Msg += "<tr> ";
                Msg += "<td style='border-radius:4px;background:#0095ff'> ";
                Msg += "<a ";
                Msg += "style='min-width: 196px;border-top: 13px solid;border-bottom: 13px solid;border-right: 24px solid;border-left: 24px solid;border-color: #2ea664;border-radius: 4px;background-color: #2ea664;color: #ffffff;font-size: 18px;line-height: 18px;word-break: break-word;display: inline-block;text-align: center;font-weight: 900;text-decoration: none!important;' ";
                Msg += "href='https://selectcon.com/bbs/Members/Confirm?k=confirm&val=" + MailTo + "|" + hash + "'";
                Msg += "> ";
                Msg += "Confirm email address ";
                Msg += "</a> ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "</tbody> ";
                Msg += "</table> ";
                Msg += "</td> ";
                Msg += "</tr> ";
                Msg += "</tbody> ";
                Msg += "</table> ";
                Msg += "</div> ";
                Msg += "</center> ";
                Msg += "</div> ";
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return SendMailData(Subject, Msg, "", MailTo);
        }

    }
}