using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

namespace Selectcon
{
    public class dbContext {
        private String _dbProvider, _dbDataSource, _dbName, _dbUserName, _dbPassword;
        public String _ConnectionString;
        public dbUtilities DB = new dbUtilities();

        public class SysConfig {
            public static Dictionary<string, string> key { get; set; }
        }

        public dbContext()
        {
            ReadDALConfigurations();
            DB.ConnectStr = _ConnectionString;
        }

        private void ReadDALConfigurations()
        {
            try
            {
                _dbProvider = "";
                _dbDataSource = ConfigurationManager.AppSettings["DB_DataSource"].ToString();
                _dbName = ConfigurationManager.AppSettings["DB_Name"].ToString();
                _dbUserName = ConfigurationManager.AppSettings["DB_UserName"].ToString();
                _dbPassword = ConfigurationManager.AppSettings["DB_Password"].ToString();

                _ConnectionString = "Data Source=" + _dbDataSource + ";Initial Catalog=" + _dbName + ";User ID=" + _dbUserName + ";Password=" + _dbPassword;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetConnectStr() {
            if (_ConnectionString == "") ReadDALConfigurations();
            return (_ConnectionString);
        }

        public dynamic OpenConn(string ConnStr = "") {
            if (ConnStr == "") {
                if (_ConnectionString == "") ReadDALConfigurations();
                ConnStr = _ConnectionString;
            }
            return DB.OpenConn(ConnStr);
        }

        public void CloseConn(ref dynamic Conn) {
            DB.CloseConn(ref Conn);
        }

        public dynamic BeginTrans(dynamic Conn)
        {
            return DB.BeginTrans(ref Conn);
        }

        public void CommitTrans(ref dynamic Trans)
        {
            DB.CommitTrans(ref Trans);
        }

        public void RollbackTrans(ref dynamic Trans)
        {
            DB.RollbackTrans(ref Trans);
        }

        public DataTable QueryData(string SQL, dynamic Conn = null, dynamic Trans = null)
        {
            return DB.QueryData(SQL, Conn, Trans); ;
        }

        public int ExecuteSQL(string SQL, dynamic Conn = null, dynamic Trans = null)
        {
            return DB.ExecuteSQL(SQL, Conn, Trans);
        }

        public Object LookupSQL(string sql, dynamic conn = null, dynamic trans = null)
        {
            return (DB.LookupSQL(sql, conn, trans));
        }

        public void AddCriteria(ref String criteriaSQL, String fieldName, Object fieldValue, dbUtilities.FieldTypes fieldType, bool AllowIN = false)
        {
            DB.AddCriteria(ref criteriaSQL, fieldName, fieldValue, fieldType, AllowIN);
        }

        public void AddCriteriaRange(ref String criteriaSQL, String fieldName, Object fromValue, Object toValue, dbUtilities.FieldTypes fieldType)
        {
            DB.AddCriteriaRange(ref criteriaSQL, fieldName, fromValue, toValue, fieldType);
        }

        public void AddSQL(Int32 operation, ref String sql1, ref String sql2, String fieldName, Object fieldValue, dbUtilities.FieldTypes fieldType)
        {
            DB.AddSQL(operation, ref sql1, ref sql2, fieldName, fieldValue, fieldType);
        }

        public void AddSQL2(Int32 operation, ref String sql1, ref String sql2, String fieldName, Object fieldValue, dbUtilities.FieldTypes fieldType)
        {
                DB.AddSQL(operation, ref sql1, ref sql2, fieldName, fieldValue, fieldType);
        }

        public String CombineSQL(Int32 operation, ref String sql1, ref String sql2, String tableName, String criteriaSQL) {
            return CombineSQL(operation, ref sql1, ref sql2, tableName, criteriaSQL, timeStamp: true);
        }

        public String CombineSQL(Int32 operation, ref String sql1, ref String sql2, String tableName, String criteriaSQL, Boolean timeStamp = true) {
            return DB.CombineSQL(operation, ref sql1, ref sql2, tableName, criteriaSQL, timeStamp);
        }

        public Object GenerateID2(string tableName, string idField, string criteria="", string prefix="", Int32 idLength=0, dynamic conn = null, dynamic trans=null)
        {
            string sql = "";
            DataTable DT;
            Object id = null; string tmp = null;
            int firstValue = 0;
            int currentValue = 0;
            int newValue = 0;
            int nextValue = 0;
            bool IsEmpty = false;
            try
            {

                try
                {
                    sql = "SELECT " + idField + " FROM " + tableName;
                    if (prefix != "")
                    {
                        if (criteria != "") { criteria += " AND "; }
                        criteria += idField + " LIKE '" + prefix + "%'";
                    }
                    if (criteria != "") { sql += " WHERE " + criteria; }
                    sql += " ORDER BY " + idField;
                    DT = QueryData(sql, conn, trans);

                    for (int running = 0; running < DT.Rows.Count; running++)
                    {
                        firstValue = Utilities.ToInt(DT.Rows[0][idField]);
                        currentValue = Utilities.ToInt(DT.Rows[running][idField]);
                        newValue = Utilities.ToInt(DT.Rows[running][idField]) + 1;

                        nextValue = Utilities.ToInt(DT.Rows[running + 1][idField]);
                        if (newValue != nextValue)
                        {
                            IsEmpty = true;
                            break;
                        }
                    }
                }
                catch
                {
                    IsEmpty = true;
                }

                if (firstValue != 1)
                {
                    id = 1;
                }
                else if (IsEmpty)
                {
                    id = newValue;  // มีช่องวาง
                }
                else
                {
                    sql = "SELECT MAX(" + idField + ") FROM " + tableName;
                    if (prefix != "")
                    {
                        if (criteria != "") { criteria += " AND "; }
                        criteria += idField + " LIKE '" + prefix + "%'";
                    }
                    if (criteria != "") { sql += " WHERE " + criteria; }
                    id = LookupSQL(sql, conn, trans);//Lookup
                    id = Project.dal.QueryData(sql);
                    if (id != null)
                    {
                        if (id.ToString().IndexOf("-") > 0)
                        {
                            tmp = id.ToString().Substring(id.ToString().IndexOf("-") + 1);
                            tmp = (Utilities.ToNum(tmp) + 1).ToString();
                        }
                        else
                        {
                            if (prefix != "") { id = id.ToString().Substring(prefix.Length + 1); }
                            id = Utilities.ToNum(id) + 1;
                        }
                    }
                    else
                    {
                        id = "1";
                    }
                }
                if (prefix != "" || idLength > 0) { id = prefix + Utilities.ToString(id).PadLeft(idLength, '0'); }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}