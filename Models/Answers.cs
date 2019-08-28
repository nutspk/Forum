using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class Answers
    {
        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; }

        [JsonProperty(PropertyName = "AnswerID")]
        public int AnswerID { get; set; }

        [JsonProperty(PropertyName = "AnswerDesc")]
        public string AnswerDesc { get; set; }

        [JsonProperty(PropertyName = "ActiveFlag")]
        public int ActiveFlag { get; set; } = 1;

        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "ReplyToID")]
        public int ReplyToID { get; set; } = 0;

        [JsonProperty(PropertyName = "UserID")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonProperty(PropertyName = "Avatar")]
        public string Avatar { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }

        public IList<Answers> GetAnswerData(string QuestionID, string AnswerID = "")
        {
            DataTable dt = new DataTable();
            string sql = "";
            var Ans = new List<Answers>();

            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "A.QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "A.AnswerID", AnswerID, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT A.*, SU.UserID, SU.DisplayName , SU.Avatar FROM ANSWERS A LEFT OUTER JOIN SYS_USERS SU ON A.UserUpdated = SU.UserName ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += " ORDER BY A.AnswerID";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Ans = dt.AsEnumerable().Select(dr =>
                        new Answers
                        {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            AnswerID = Utilities.ToInt(dr["AnswerID"]),
                            AnswerDesc = dr["AnswerDesc"] + "",
                            ActiveFlag = Utilities.ToInt(dr["ActiveFlag"]),
                            IpAddress = Utilities.blindIpAddress(dr["IpAddress"] + ""),
                            ReplyToID = Utilities.ToInt(dr["ReplyToID"]),
                            UserID = dr["UserID"]+"",
                            UserUpdated = dr["DisplayName"]+"",
                            Avatar = dr["Avatar"] + "",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                        }
                    ).ToList<Answers>();
                }

                return Ans;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MngAnswerData(int op, Answers Ans, bool TimeStamp = true)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", Ans.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "AnswerID", Ans.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "QuestionID", Ans.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                        //Project.dal.AddSQL(op, ref SQL1, ref SQL2, "AnswerID", Ans.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                    }
                    
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "AnswerDesc", Ans.AnswerDesc, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ActiveFlag", Ans.ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ReplyToID", Ans.ReplyToID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IpAddress", Project.GetIp(), dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "ANSWERS", Criteria, TimeStamp);
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