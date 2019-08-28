using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class Membership
    {
        //UserName, Password, PasswordSalt, qwerty, DisplayName, Comment, RoleID, Email, Tel, PasswordResetToken, PasswordResetTokenTime, IsApproved, IsBanned, Avatar, 
        [JsonProperty(PropertyName = "UserID")] public string UserID { get; set; } = "";
        [JsonProperty(PropertyName = "UserName")] public string UserName { get; set; } = "";
        [JsonProperty(PropertyName = "Password")] public string Password { get; set; }
        [JsonProperty(PropertyName = "DisplayName")] public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "Comment")] public string Comment { get; set; }
        [JsonProperty(PropertyName = "RoleID")] public int RoleID { get; set; } = -1;
        [JsonProperty(PropertyName = "RoleName")] public string RoleName { get; set; }
        [JsonProperty(PropertyName = "Email")] public string Email { get; set; }
        [JsonProperty(PropertyName = "Tel")] public string Tel { get; set; }
        [JsonProperty(PropertyName = "PasswordResetToken")] public string PasswordResetToken { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "PasswordResetTokenTime")]
        public DateTime? PasswordResetTokenTime { get; set; }
        [JsonProperty(PropertyName = "IsApproved")] public int IsApproved { get; set; } = -1;
        [JsonProperty(PropertyName = "IsBanned")] public int IsBanned { get; set; } = -1;
        [JsonProperty(PropertyName = "BannedReason")] public string BannedReason { get; set; }
        [JsonProperty(PropertyName = "Avatar")] public string Avatar { get; set; }
        [JsonProperty(PropertyName = "UserUpdated")] public string UserUpdated { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateUpdated")] public DateTime? DateUpdated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "DateCreated")] public DateTime? DateCreated { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "dd/MM/yyyy HH:mm:ss")]
        [JsonProperty(PropertyName = "LastLogin")] public DateTime? LastLogin { get; set; }

        [JsonProperty(PropertyName = "Address")] public string Address { get; set; }

        [JsonProperty(PropertyName = "Title")] public string Title { get; set; }




        public IList<Membership> GetUserData(string UserID = "", string UserName = "", string Password = "") {
            DataTable dt = new DataTable();
            string sql = "";
            var Member = new List<Membership>();
            string Criteria = "";
            try
            {

                Project.dal.AddCriteria(ref Criteria, "UserID", UserID, dbUtilities.FieldTypes.ftNumeric);

                if (UserName != "") {
                    if (Criteria != "") Criteria += " AND ";
                    UserName = UserName.ToUpper();
                    Criteria += " (UPPER(UserName) ='" + UserName + "' OR UPPER(Email)='" + UserName + "')";
                }

                //Project.dal.AddCriteria(ref Criteria, "Password", Password, dbUtilities.FieldTypes.ftText);

                sql = "SELECT * FROM SYS_USERS ";
                if (Criteria != "") sql += " WHERE " + Criteria;
                sql += "ORDER BY DateUpdated";

                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Member = dt.AsEnumerable().Select(dr =>
                        new Membership
                        {
                            UserID = dr["UserID"] + "",
                            UserName = dr["UserName"] + "",
                            Password = dr["Password"] + "",
                            DisplayName = dr["DisplayName"] + "",
                            Comment = dr["Comment"] + "",
                            RoleID = Utilities.ToInt(dr["RoleID"]),
                            Email = dr["Email"] + "",
                            Tel = dr["Tel"] + "",
                            PasswordResetToken = dr["PasswordResetToken"] + "",
                            PasswordResetTokenTime = Utilities.AppDateValue(dr["PasswordResetTokenTime"]),
                            IsApproved = Utilities.ToInt(dr["IsApproved"]),
                            IsBanned = Utilities.ToInt(dr["IsBanned"]),
                            BannedReason = (Utilities.ToInt(dr["IsBanned"]) == 1) ? dr["BannedReason"] + "" : "",
                            Avatar = dr["Avatar"] + "",
                            DateCreated = GetCreateDate(dr["UserName"] + ""),
                            LastLogin = GetLastLogin(dr["UserName"] + ""),
                            Address = dr["Address"] + "",
                            Title = dr["Title"] + "",
                            RoleName = getRoleName(Utilities.ToInt(dr["RoleID"]))
                        }
                    ).ToList<Membership>();
                }

                return Member;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string getRoleName(int roleId) {
            string ret = "";

            if (roleId == (int)Project.RoleType.Admin)
            {
                ret = "Administrator";
            } else if (roleId == (int)Project.RoleType.SuperUser) {
                ret = "Super User";
            }else {
                ret = "User";
            }

            return ret;
        }

        private DateTime? GetLastLogin(string userName) {
            string sql = "";
            DataTable dt = null;
            DateTime? d = default(Nullable<DateTime>);
            sql = "SELECT top 1 * FROM SYS_LOGS WHERE Categories='LOGIN' AND UPPER(UserUpdated)='" + userName.ToUpper() + "' ORDER BY DateUpdated DESC ";

            dt = Project.dal.QueryData(sql);
            if (dt != null && dt.Rows.Count > 0) {
                d = (DateTime)dt.Rows[0]["DateUpdated"];
            }

            return d;
        }

        private DateTime? GetCreateDate(string userName) {
            string sql = "";
            DataTable dt = null;
            DateTime? d = default(Nullable<DateTime>);
            sql = "SELECT top 1 * FROM SYS_LOGS WHERE Categories='REGISTER' AND UPPER(UserUpdated)='" + userName.ToUpper() + "' ORDER BY DateUpdated DESC ";

            dt = Project.dal.QueryData(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                d = (DateTime)dt.Rows[0]["DateUpdated"];
            }

            return d;
        }

        public bool MngUserData(int op, Membership user) {
            bool result = false;
            string SQL= "", SQL1 = "", SQL2="", Criteria = "";
            try {

                if (op != dbUtilities.opINSERT) {
                    Project.dal.AddCriteria(ref Criteria, "UserID", user.UserID, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddCriteria(ref Criteria, "UserName", user.UserName.ToUpper(), dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opDELETE)
                {
                    if (op == dbUtilities.opINSERT)
                    {
                        Project.dal.AddSQL(op, ref SQL1, ref SQL2, "UserName", user.UserName, dbUtilities.FieldTypes.ftText);
                        user.RoleID = (int)Project.RoleType.User;
                        Project.IsMe().UserName = user.UserName;
                    }

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "RoleID", user.RoleID, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Password", user.Password, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Comment", user.Comment, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "DisplayName", user.DisplayName, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Email", user.Email, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Tel", user.Tel, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "PasswordResetToken", user.PasswordResetToken, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "PasswordResetTokenTime", user.PasswordResetTokenTime, dbUtilities.FieldTypes.ftDateTime);
                    if (user.IsApproved != -1) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IsApproved", user.IsApproved, dbUtilities.FieldTypes.ftNumeric);
                    if (user.IsBanned != -1) Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "IsBanned", user.IsBanned, dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "BannedReason", user.BannedReason, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Avatar", user.Avatar, dbUtilities.FieldTypes.ftText);

                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Title", user.Title, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "Address", user.Address, dbUtilities.FieldTypes.ftText);
                }

                if (op != dbUtilities.opINSERT && Criteria == ""){
                    result = false;
                    Project.GetErrorMessage("Insufficient data!");
                } else {
                    SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "SYS_USERS", Criteria);
                    Project.dal.ExecuteSQL(SQL, null, null);
                }

                result = true;

            }
            catch (Exception ex) {
                result = false;
                Project.GetErrorMessage(ex);
            }
            return result;
        }

       
    }
}

    