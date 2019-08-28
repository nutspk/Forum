using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class Categories
    {

        [JsonProperty(PropertyName = "CategoryID")]
        public int CategoryID { get; set; }

        [JsonProperty(PropertyName = "CategoryName")]
        public string CategoryName { get; set; }

        [JsonProperty(PropertyName = "CategoryDesc")]
        public string CategoryDesc { get; set; }

        [JsonProperty(PropertyName = "LatestQuestion")]
        public Questions LatestQuestion { get; set; }

        [JsonProperty(PropertyName = "IsPublic")]
        public int IsPublic { get; set; } = -1;

        [JsonProperty(PropertyName = "ActiveFlag")]
        public int ActiveFlag { get; set; } = -1;

        [JsonProperty(PropertyName = "Sequence")]
        public int Sequence { get; set; }


        public IList<Categories> GetCategories() {
            DataTable dt = new DataTable();
            string sql = "";
            var cate = new List<Categories>();
            try {

                sql = "SELECT * FROM V_CATEGORIES";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0) {
                    cate = dt.AsEnumerable().Select(dr =>
                        new Categories
                        {
                            CategoryID = Utilities.ToInt(dr["CategoryID"]),
                            CategoryName = dr["CategoryName"] + "",
                            CategoryDesc = dr["CategoryDesc"] + "",
                            LatestQuestion = new Questions() {
                                QuestionID = Utilities.ToInt(dr["QuestionID"]),
                                QuestionTitle = dr["QuestionTitle"] +"",
                                DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                                UserID = dr["UserID"]+"",
                                UserUpdated = dr["DisplayName"]+ ""
                            },
                            ActiveFlag = Utilities.ToInt(dr["ActiveFlag"]),
                            Sequence = Utilities.ToInt(dr["Sequence"]),
                        }
                    ).ToList<Categories>();
                }

                return cate;

            } catch (Exception ex) {
                throw ex;
            }
            
        }

        public bool MngCategoryData(int op, Categories Cate)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "CategoryID", Cate.CategoryID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        var seq = Project.dal.GenerateID2("CATEGORIES", "CategoryID");
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "Sequence", seq, dbUtilities.FieldTypes.ftNumeric);
                        //Project.dal.AddSQL(op, ref SQL1, ref SQL2, "AnswerID", Ans.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                    }

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "CategoryName", Cate.CategoryName, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "CategoryDesc", Cate.CategoryDesc, dbUtilities.FieldTypes.ftText);
                    if (Cate.IsPublic > -1) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IsPublic", Cate.IsPublic, dbUtilities.FieldTypes.ftNumeric);
                    if (Cate.ActiveFlag > -1)  Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ActiveFlag", Cate.ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CATEGORIES", Criteria);
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