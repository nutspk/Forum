using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Selectcon
{
    public class Validations
    {
        public static string[] HazardStringList3 = { "\"", "'", "//", "<%", "%>", "{", "}", "/*", "*/", "</", "&quot", "&apos", "&amp", "&lt", "&gt", "onmouseover", "<script", "|", "?", ";", "[", "]", "*", "%0" };

        public static string GetParamStr(string ParamName, HtmlInputHidden hCtrl = null, string DefaultVal = "", bool AllowEmpty = true, bool IsEncoded = false)
        {
            System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
            string ParamData = null;
            string value = null;

            if (!Page.IsPostBack)
            {
                ParamData = Page.Request.QueryString[ParamName] + "";
                if (IsEncoded)
                    ParamData = DecodeParam(ParamData);
            }
            else
            {
                if ((hCtrl == null))
                {
                    ParamData = Page.Request.Form[ParamName];
                }
                else
                {
                    ParamData = hCtrl.Value;
                }
            }

            value = ValidateStr(ParamData, AllowEmpty, DefaultVal);
            if ((hCtrl != null))
                hCtrl.Value = value;
            return value;
        }

        public static string ValidateStr(string Data, bool AllowEmpty, object DefaultVal)
        {
            try
            {
                if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
                {
                    if (AllowEmpty)
                    {
                        if (DefaultVal != null)
                        {
                            return DefaultVal.ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else
                    {
                        throw new System.Exception("String is required");
                    }
                }
                else if (IsValidStr(Data, false))
                {
                    return Data;
                }
                else
                {
                    throw new System.Exception("Invalid string");
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Invalid string");
            }
        }

        public static bool IsValidStr(string Data, bool IgnoreException)
        {
            if (!HasHazardStr3(Data))
            {
                return true;
            }
            else if (IgnoreException)
            {
                return false;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }

        public static bool HasHazardStr3(string Data)
        {
            foreach (string hz in HazardStringList3)
            {
                if (Data.IndexOf(hz) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static string EncodeParam(string Data)
        {
            System.Text.ASCIIEncoding Encoder = new System.Text.ASCIIEncoding();
            try
            {
                if (Data != null && Data != "")
                {
                    return Convert.ToBase64String(Encoder.GetBytes(Data.ToCharArray()));
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Invalid parameter data");
            }
        }

        public static string DecodeParam(string EncodedData)
        {
            System.Text.ASCIIEncoding Encoder = new System.Text.ASCIIEncoding();
            try
            {
                if (EncodedData != null && EncodedData != "")
                {
                    return Encoder.GetString(Convert.FromBase64String(EncodedData));
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                throw new System.Exception("Invalid parameter data");
            }
        }


    }
}