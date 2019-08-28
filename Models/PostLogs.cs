using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class PostLogs
    {
        [JsonProperty(PropertyName = "LogID")]
        public int LogID { get; set; }

        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; }

        [JsonProperty(PropertyName = "AnswerID")]
        public int? AnswerID { get; set; } = null;

        [JsonProperty(PropertyName = "OriginalTitile")]
        public string OriginalTitile { get; set; }
            
        [JsonProperty(PropertyName = "OriginalDesc")]
        public string OriginalDesc { get; set; }

        [JsonProperty(PropertyName = "UserID")]
        public int UserID { get; set; }


        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }

        public IList<PostLogs> GetLogData(string QuestionID, string AnswerID = "")
        {
            DataTable dt = new DataTable();
            string sql = "";
            var Ans = new List<PostLogs>();

            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "E.QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "E.AnswerID", AnswerID, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT * FROM EDIT_LOG E LEFT OUTER JOIN SYS_USERS S ON E.UserUpdated=S.UserName ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += " ORDER BY LogID DESC";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Ans = dt.AsEnumerable().Select(dr =>
                        new PostLogs
                        {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            AnswerID = Utilities.ToInt(dr["AnswerID"]),
                            OriginalTitile = dr["OriginalTitile"] + "",
                            OriginalDesc = dr["OriginalDesc"]+"",
                            UserID = Utilities.ToInt(dr["UserID"]),
                            UserUpdated = dr["DisplayName"]+"",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                        }
                    ).ToList<PostLogs>();
                }

                return Ans;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MngLogData(int op, PostLogs logs)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", logs.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "AnswerID", logs.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "QuestionID", logs.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "AnswerID", logs.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                    }

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OriginalTitile", logs.OriginalTitile, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OriginalDesc", logs.OriginalDesc, dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                } else {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "EDIT_LOG", Criteria);
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