using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class ImageGallery
    {
        [JsonProperty(PropertyName = "QuestionID")]
        public int? QuestionID { get; set; } = null;

        [JsonProperty(PropertyName = "GalleryID")]
        public int? GalleryID { get; set; } = null;
        
        [JsonProperty(PropertyName = "GalleryDesc")]
        public string GalleryDesc { get; set; }

        [JsonProperty(PropertyName = "ImgList")]
        public List<Images> ImgList { get; set; } = new List<Images>();

        [JsonProperty(PropertyName = "ActiveFlag")]
        public int? ActiveFlag { get; set; } = null;

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }

        
        public List<ImageGallery> GetGalleryData(string QuestionID, string GalleryID="", bool IsActive = true)
        {
            DataTable dt = new DataTable();
            string sql = "";
            var Gallery = new List<ImageGallery>();
            var Img = new List<Images>();

            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "QuestionID", QuestionID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "GalleryID", GalleryID, dbUtilities.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "IActiveFlag", (IsActive)? 1 : 0, dbUtilities.FieldTypes.ftNumeric);

                sql = "SELECT * FROM V_GALLERIES ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += " ORDER BY DateUpdated, IDateUpdated";
                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {

                    Img = dt.AsEnumerable().Select(dr2 =>
                            new Images
                            {
                                GalleryID = Utilities.ToInt(dr2["GalleryID"]),
                                ImageID = Utilities.ToInt(dr2["ImageID"]),
                                ImageUrl = dr2["ImageUrl"] + "",
                                ShowFlag = Utilities.ToInt(dr2["ShowFlag"]),
                                ActiveFlag = Utilities.ToInt(dr2["ActiveFlag"]),
                                UserUpdated = dr2["IDisplayName"] + "",
                                DateUpdated = (dr2["IDateUpdated"] != DBNull.Value) ? (DateTime)dr2["IDateUpdated"] : default(Nullable<DateTime>),
                            }
                        ).ToList<Images>();

                    Gallery = dt.AsEnumerable().Select(dr =>
                    new ImageGallery
                    {
                            QuestionID = Utilities.ToInt(dr["QuestionID"]),
                            GalleryID = Utilities.ToInt(dr["GalleryID"]),
                            GalleryDesc = dr["GalleryDesc"] + "",
                            ActiveFlag = Utilities.ToInt(dr["ActiveFlag"]),
                            ImgList = Img.Where(r => r.GalleryID == Utilities.ToInt(dr["GalleryID"])).Select(r => r).ToList<Images>(),
                            UserUpdated = dr["DisplayName"] + "",
                            DateUpdated = (dr["DateUpdated"] != DBNull.Value) ? (DateTime)dr["DateUpdated"] : default(Nullable<DateTime>),
                        }
                    ).Distinct().ToList<ImageGallery>();
                }

                Gallery = Gallery.GroupBy(r => r.GalleryID).Select(g => g.First()) .ToList<ImageGallery>();

                return Gallery;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool MngGalleryData(int op,ref ImageGallery ig, dynamic conn = null, dynamic trans = null)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {
  

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "QuestionID", ig.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "GalleryID", ig.GalleryID, dbUtilities.FieldTypes.ftNumeric);
                    
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "QuestionID", ig.QuestionID, dbUtilities.FieldTypes.ftNumeric);
                        //Project.dal.AddSQL(op, ref SQL1, ref SQL2, "AnswerID", Ans.AnswerID, dbUtilities.FieldTypes.ftNumeric);
                    }

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "GalleryDesc", ig.GalleryDesc, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ActiveFlag", ig.ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!", "error");
                    
                }
                else
                {
                    if (op == dbUtilities.opINSERT)
                    {
                      
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "DateUpdated", System.DateTime.Now, dbUtilities.FieldTypes.ftDateTime);
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "UserUpdated", Project.IsMe().UserName.ToUpper(), dbUtilities.FieldTypes.ftText);

                        SQL = "INSERT INTO QUESTION_GALLERIES (" + SQL1 + ") OUTPUT INSERTED.GalleryID  VALUES (" + SQL2 + ")";

                        ig.GalleryID = (int)Project.dal.LookupSQL(SQL,conn,trans);
                    }
                    else
                    {
                        SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "QUESTION_GALLERIES", Criteria);

                        Project.dal.ExecuteSQL(SQL, conn, trans);
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

        public bool MngImageData(int op, ref Images im, dynamic conn = null, dynamic trans = null)
        {
            bool result = false;
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try
            {

                if (op != dbUtilities.opINSERT)
                {
                    Project.dal.AddCriteria(ref Criteria, "GalleryID", im.GalleryID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "ImageID", im.ImageID, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "GalleryID", im.GalleryID, dbUtilities.FieldTypes.ftNumeric);
                    }

                    //[ImageUrl] [ShowFlag] [ActiveFlag]
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ImageUrl", im.ImageUrl, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ShowFlag", im.ShowFlag, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ActiveFlag", im.ActiveFlag, dbUtilities.FieldTypes.ftNumeric);
                }

                if (op != dbUtilities.opINSERT && Criteria == "")
                {
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                }
                else
                {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "IMAGES", Criteria);
                    Project.dal.ExecuteSQL(SQL, conn, trans);
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

    public class Images {

        [JsonProperty(PropertyName = "GalleryID")]
        public int? GalleryID { get; set; } = null;

        [JsonProperty(PropertyName = "ImageID")]
        public int? ImageID { get; set; } = null;

        [JsonProperty(PropertyName = "ImageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "ShowFlag")]
        public int? ShowFlag { get; set; } = null;

        [JsonProperty(PropertyName = "ActiveFlag")]
        public int? ActiveFlag { get; set; } = null;

        [JsonProperty(PropertyName = "UserUpdated")]
        public string UserUpdated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")]
        public DateTime? DateUpdated { get; set; }
    }



}