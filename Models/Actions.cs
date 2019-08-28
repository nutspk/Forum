using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class Actions
    {
        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; }

        [JsonProperty(PropertyName = "AnswerID")]
        public int AnswerID { get; set; }

        [JsonProperty(PropertyName = "Likes")]
        public int Likes { get; set; }

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonProperty(PropertyName = "UserID")]
        public string UserID { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }

        public IList<Actions> GetActionsData(string QuestionID, string AnswerID= "", string UID ="")
        {
            DataTable dt = new DataTable();
            string sql = "";
            var act = new List<Actions>();

            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "A.QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "A.AnswerID", AnswerID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "A.UserUpdated", UID, dbUtilities.FieldTypes.ftText);

                sql = "SELECT A.*, SU.UserID, SU.DisplayName , SU.Avatar FROM ACTION_COUNT A LEFT OUTER JOIN SYS_USERS SU ON A.UserUpdated = SU.UserName ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += " ORDER BY A.QuestionID, A.AnswerID";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    act = dt.AsEnumerable().Select(dr =>
                        new Actions
                        {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            AnswerID = Utilities.ToInt(dr["AnswerID"]),
                            Likes = Utilities.ToInt(dr["LIKES"]),
                            UserID = dr["UserID"]+"",
                            UserUpdated = dr["DisplayName"]+"",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                        }
                    ).ToList<Actions>();
                }

                return act;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MngActionData(int op, string QuestionId, string AnswerID = "0", string Cnt="")
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "AnswerID", AnswerID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "AnswerID", AnswerID, dbUtilities.FieldTypes.ftNumeric);
                    }
                    
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "LIKES", Cnt, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    try {
                        SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "ACTION_COUNT", Criteria);
                        Project.dal.ExecuteSQL(SQL, null, null);
                    } catch {
                        Project.dal.AddCriteria(ref Criteria, "DateUpdated", System.DateTime.Now, dbUtilities.FieldTypes.ftDateTime);
                        Project.dal.AddCriteria(ref Criteria, "UserUpdated", Project.IsMe().UserName.ToUpper(), dbUtilities.FieldTypes.ftText);

                        SQL = "UPDATE ACTION_COUNT SET LIKES = " + Cnt + " WHERE " + Criteria;
                        Project.dal.ExecuteSQL(SQL, null, null);
                    }
                    
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