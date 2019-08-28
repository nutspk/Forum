using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selectcon
{
    public class Utilities
    {
        public static Boolean IsDBNull(dynamic value)
        {
            return (value == DBNull.Value);
        }

        public static Boolean IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            if (Expression != null)
            {
                isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            }
            else
            {
                isNum = false;
            }
            return isNum;
        }

        public static Boolean IsDate(object Expression)
        {
            bool isDate;
            DateTime retDate;

            if (Expression != null)
            {
                isDate = DateTime.TryParse(Convert.ToString(Expression), out retDate);
            }
            else
            {
                isDate = false;
            }
            return isDate;
        }

        public static String ToString(Object val)  {
            if (val == null) {
                return "";
            } else {
                return (val + "").ToString();
            }
        }

        public static Decimal ToNum(Object N) {
            Decimal num;
            try {
                num = Convert.ToDecimal(N);
            } catch {
                num = 0;
            }
            return num;
        }

        public static int ToInt(Object N)
        {
            int num;
            try
            {
                num = Convert.ToInt32(N);
            }
            catch
            {
                num = 0;
            }
            return num;
        }

        public static DateTime AppDateValue(object D, string fmt="")
        {
            System.Globalization.CultureInfo Culture = new System.Globalization.CultureInfo("en-US");

            DateTime DT = new DateTime();
            String S = "";
            try
            {
                if (D + "" != "")
                {
                    S = D.ToString();
                    if (fmt == "")
                    {
                        if (S.Length > 10)
                        {
                            DT = DateTime.ParseExact(S, "dd/MM/yyyy HH:mm:ss", Culture);
                        }
                        else
                        {
                            DT = DateTime.ParseExact(S, "dd/MM/yyyy", Culture);
                        }
                    }
                    else
                    {
                        DT = DateTime.ParseExact(S, fmt, Culture);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    DT = DateTime.Parse(S);
                }
                catch (Exception ex2)
                {
                    DT = DT;
                }
            }

            return DT;
        }

        public static String AppFormatDateTime(object D, string fmt="", string LangType = "EN")
        {
            string result = "";
            System.Globalization.CultureInfo Culture;

            try
            {
                if (ToString(D) != "")
                {
                    if (LangType == "EN")
                        Culture = new System.Globalization.CultureInfo("en-US");
                    else
                        Culture = new System.Globalization.CultureInfo("th-TH");

                    if (fmt == "")
                        result = string.Format(Culture, @"{0:dd/MM/yyyy HH:mm:ss}", D);
                    else 
                        result = string.Format(Culture, @"{0:"+ fmt + "}", D);
                }
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        public static string blindIpAddress(string ip) {
            int start = 0;
            string newIp = "";
            try {
                start = ip.LastIndexOf(".");
                newIp = ip.Substring(0, start) + ".xxx";
            }
            catch (Exception ex) {
                newIp = "";
                Project.GetErrorMessage(ex);
            }
            return newIp;
        }


    }
}