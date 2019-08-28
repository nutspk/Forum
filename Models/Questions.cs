using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;

namespace Selectcon.Models
{
    public class Questions
    {
        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; }

        [JsonProperty(PropertyName = "CategoryID")]
        public int CategoryID { get; set; }

        [JsonProperty(PropertyName = "CategoryName")]
        public string CategoryName { get; set; } = "";

        [JsonProperty(PropertyName = "QuestionTitle")]
        public string QuestionTitle { get; set; }
        
        [JsonProperty(PropertyName = "QuestionDesc")]
        public string QuestionDesc { get; set; }

        [JsonProperty(PropertyName = "LikeCnt")]
        public int LikeCnt { get; set; } = 0;

        [JsonProperty(PropertyName = "ViewCnt")]
        public int ViewCnt { get; set; } = -1;

        [JsonProperty(PropertyName = "AnswerCnt")]
        public int AnswerCnt { get; set; } = -1;

        [JsonProperty(PropertyName = "UserID")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonProperty(PropertyName = "Avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "DateUpdated")]
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        public DateTime? DateUpdated { get; set; }

        [JsonProperty(PropertyName = "IsPublic")]
        public int IsPublic { get; set; } = -1;

        [JsonProperty(PropertyName = "ActiveFlag")]
        public int ActiveFlag { get; set; } = -1;

        [JsonProperty(PropertyName = "IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "TagList")]
        public List<Tags> TagList { get; set; }

        [JsonProperty(PropertyName = "GalleyList")]
        public List<ImageGallery> GalleyList { get; set; }

        [JsonProperty(PropertyName = "UserList")]
        public List<Membership> UserList { get; set; }

        [JsonProperty(PropertyName = "ImageID")]
        public Images Image { get; set; }


        public IList<Questions> GetQuestionData(string QuestionID = "", string CategoryID = "")
        {
            DataTable dt = new DataTable();
            string sql = "", ActiveFlag = "1";
            var Question = new List<Questions>();
           
            string Criteria = "";
            try
            {
                if (Project.IsAdmin) ActiveFlag = "";

                Project.dal.AddCriteria(ref Criteria, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "ActiveFlag", ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "CategoryID", CategoryID, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT * FROM V_QUESTIONS ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += "";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Question = dt.AsEnumerable().Select(dr =>
                        new Questions
                        {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            CategoryID = Utilities.ToInt(dr["CategoryID"]),
                            CategoryName = dr["CategoryName"]+"",
                            QuestionTitle = dr["QuestionTitle"] + "",
                            QuestionDesc = dr["QuestionDesc"] + "",
                            IsPublic = Utilities.ToInt(dr["IsPublic"]),
                            ActiveFlag = Utilities.ToInt(dr["ActiveFlag"]),
                            UserID = dr["UserID"] + "",
                            UserUpdated = dr["DisplayName"] + "",
                            Avatar = dr["Avatar"] + "",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                            IpAddress = Utilities.blindIpAddress(dr["IpAddress"] + ""),
                            LikeCnt = Utilities.ToInt(dr["LikeCnt"]),
                            AnswerCnt = Utilities.ToInt(dr["AnswerCnt"]),
                            ViewCnt = Utilities.ToInt(dr["ViewCnt"]),
                            //TagList = GetTagList(dr["TagList"] +""),
                            TagList = null,
                            GalleyList = null,
                            UserList = GetMemberList(dr["QuestionID"] +"")
                        }
                    ).ToList<Questions>();
                }

                return Question;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Questions> GetQuestionByUser(string UserID, string CategoryID = "")
        {
            DataTable dt = new DataTable();
            string sql = "";
            var Question = new List<Questions>();

            string Criteria = "";
            try
            {
                Project.dal.AddCriteria(ref Criteria, "V.CategoryID", CategoryID, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT DISTINCT * FROM V_QUESTIONS V LEFT OUTER JOIN QUESTION_USERS Q ON V.QuestionID = Q.QuestionID ";
                sql += " WHERE (V.IsPublic = 1 OR (V.IsPublic = 0 AND Q.UserID = '"+ UserID + "')) ";
                if (Criteria != "") sql += " AND " + Criteria;
                sql += "";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Question = dt.AsEnumerable().Select(dr =>
                        new Questions
                        {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            CategoryID = Utilities.ToInt(dr["CategoryID"]),
                            CategoryName = dr["CategoryName"] + "",
                            QuestionTitle = dr["QuestionTitle"] + "",
                            QuestionDesc = dr["QuestionDesc"] + "",
                            IsPublic = Utilities.ToInt(dr["IsPublic"]),
                            ActiveFlag = Utilities.ToInt(dr["ActiveFlag"]),
                            UserID = dr["UserID"] + "",
                            UserUpdated = dr["DisplayName"] + "",
                            Avatar = dr["Avatar"] + "",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                            IpAddress = Utilities.blindIpAddress(dr["IpAddress"] + ""),
                            LikeCnt = Utilities.ToInt(dr["LikeCnt"]),
                            AnswerCnt = Utilities.ToInt(dr["AnswerCnt"]),
                            ViewCnt = Utilities.ToInt(dr["ViewCnt"]),
                            //TagList = GetTagList(dr["TagList"] + ""),
                            TagList = null,
                            GalleyList = null,
                            UserList = GetMemberList(dr["QuestionID"] + "")
                        }
                    ).ToList<Questions>();
                }

                return Question;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<Tags> GetTagList(string TagList) {
            var ret = new List<Tags>();
            try
            {
                var tags = new Tags().GetTagData().ToList<Tags>();
                if (!string.IsNullOrEmpty(TagList)) {
                    List<string> tl = TagList.Split(',').ToList();
                    if (tl.Count > 0)
                    {
                        foreach (string s in tl)
                        {
                            var a = tags.Where(r => r.TagID == Utilities.ToInt(s)).FirstOrDefault();
                            ret.Add(a);
                        }
                    }
                }
            } catch (Exception) {
                
            }

            return ret;
        }

        private List<Membership> GetMemberList(string QuestionID) {
            DataTable dt = new DataTable();
            try {
                dt = Project.dal.QueryData("SELECT Q.*, S.DisplayName FROM QUESTION_USERS Q LEFT OUTER JOIN SYS_USERS S ON Q.UserUpdated=S.UserName WHERE Q.QuestionID=" + QuestionID +"");

              var mb = dt.AsEnumerable().Select(r => new Membership
                {
                    UserID = r["UserID"]+"",
                    DisplayName = r["DisplayName"] + "",
              }).ToList<Membership>();

                return mb;
            }
            catch (Exception ex ) {
                return null;
                throw ex ;
            }
            
        }

        public bool MngQuestionData(int op,ref Questions Q, bool View = true)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            string tagList = "";
            try
            {

                if (Q.TagList != null) {
                    tagList = String.Join(",", Q.TagList.Select(m => m.TagID).ToArray());
                }

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", Q.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "CategoryID", Q.CategoryID, dbUtilities.FieldTypes.ftNumeric);
                    }

                    if (View)  Q.ViewCnt += 1;

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QuestionTitle", Q.QuestionTitle, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QuestionDesc", Q.QuestionDesc, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "TagList", tagList, dbUtilities.FieldTypes.ftText);
                    if (Q.ViewCnt > -1) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Viewer", Q.ViewCnt, dbUtilities.FieldTypes.ftNumeric);
                    if (Q.IsPublic > -1) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IsPublic", Q.IsPublic, dbUtilities.FieldTypes.ftNumeric);
                    if (Q.ActiveFlag > -1)  Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ActiveFlag", Q.ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IpAddress", Project.GetIp(), dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    if (op == dbUtilities.opINSERT) {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "DateUpdated", System.DateTime.Now, dbUtilities.FieldTypes.ftDateTime);
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "UserUpdated", Project.IsMe().UserName.ToUpper(), dbUtilities.FieldTypes.ftText);

                        SQL = "INSERT INTO QUESTIONS (" + SQL1 + ") OUTPUT INSERTED.QuestionID  VALUES (" + SQL2 + ")";
                        Q.QuestionID = (int)Project.dal.LookupSQL(SQL);
                    } else {
                        SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "QUESTIONS", Criteria, !View);
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

        public bool MngQuestionUser(int op, int QuestionID, string UserID = "") {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {
                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "UserID", UserID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UserID", UserID, dbUtilities.FieldTypes.ftNumeric);

                if (op != dbUtilities.opINSERT && Criteria == "") {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                } else {
                    result = true;
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "QUESTION_USERS", Criteria);
                    Project.dal.ExecuteSQL(SQL, null, null);
                }
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