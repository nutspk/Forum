using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class SysLogs
    {
        [JsonProperty(PropertyName = "TransID")]
        public int TransID { get; set; }

        [JsonProperty(PropertyName = "Category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "Details")]
        public string Details { get; set; }

        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }

        public IList<SysLogs> GetAnswerData(string QuestionID)
        {
            DataTable dt = new DataTable();
            string sql = "";
            var log = new List<SysLogs>();

            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "A.QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT * FROM SYS_LOGS ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += " ORDER BY TransDate DESC";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    log = dt.AsEnumerable().Select(dr =>
                        new SysLogs
                        {
                            TransID = Utilities.ToInt(dr["TransID"]),
                            Category = dr["Category"] + "",
                            Details = dr["Details"] + "",
                            IpAddress = dr["IpAddress"] + "",
                            UserUpdated = dr["DisplayName"]+"",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                        }
                    ).ToList<SysLogs>();
                }

                return log;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MngLogData(int op, SysLogs log)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "Categories", log.Category, dbUtilities.FieldTypes.ftText);
                    }
                    
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ReplyToID", log.Details, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IpAddress", Project.GetIp(), dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "SYS_LOGS", Criteria);
                    Project.dal.ExecuteSQL(SQL, null, null);
                }

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
                Project.GetErrorMessage(ex);
            }
            return result;
        }


    }
}