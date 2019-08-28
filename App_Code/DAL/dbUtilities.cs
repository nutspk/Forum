using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Odbc;

namespace Selectcon
{
    public class dbUtilities
    {
        private string _ConnectStr;
        private string _DB_Provider;
        private string _DB_Name;

        public const int opINSERT = 1;
        public const int opUPDATE = 2;
        public const int opDELETE = 3;

        public enum FieldTypes {
            ftNumeric = 1,
            ftText = 2,
            ftDate = 3,
            ftDateTime = 6,
            ftBinary = 7
        }

        public string ConnectStr {
            get { return _ConnectStr; }
            set { _ConnectStr = value; }
        }

        public string DB_Provider {
            get { return _DB_Provider; }
            set { _DB_Provider = value; }
        }

        public dynamic OpenConn(string ConnectStr)  {
            if (!string.IsNullOrEmpty(ConnectStr))
                _ConnectStr = ConnectStr;
            return OpenDBConn();
        }

        private dynamic OpenDBConn()
        {
            dynamic Conn = null;
            int I = 0;
            if (string.IsNullOrEmpty(_ConnectStr)) {
                _ConnectStr = ConnectStr;
            }

            try
            {
                Conn = CreateConnection(_ConnectStr);
                Conn.Open();
                return Conn;
            }
            catch (Exception ex)
            {
                CloseConn(ref Conn);
                throw ex;
            }
        }

        public dynamic CreateConnection(string _ConnectStr = "")
        {
            return new System.Data.SqlClient.SqlConnection(_ConnectStr);
        }

        public void CloseConn(ref dynamic Conn)
        {
            if ((Conn != null)){
                try {
                    Conn.Close();
                    Conn.Dispose();
                } catch (Exception ex){
                }
                Conn = null;
            }
        }

        //========================================
        // Begin Transaction
        public dynamic BeginTrans(ref dynamic Conn)
        {
            dynamic result = null;
            if ((Conn != null))
            {
                result = Conn.BeginTransaction();
            }
            else
            {
                throw new Exception("Connection has not been initialized!");
            }
            return result;
        }

        //========================================
        // Commit Transaction
        public void CommitTrans(ref dynamic Trans)
        {
            if ((Trans != null))
            {
                Trans.Commit();
                Trans = null;
            }
        }

        //========================================
        // Rollback Transaction
        public void RollbackTrans(ref dynamic Trans)
        {
            try
            {
                if ((Trans != null))
                {
                    Trans.Rollback();
                }
            }
            catch
            {
            }
            Trans = null;
        }

        public dynamic CreateCommand(string SQL = "", dynamic _Conn = null, dynamic _Trans = null)
        {
            dynamic cmd = null;
            try {
                if (_Conn != null) {
                    cmd = new System.Data.SqlClient.SqlCommand(SQL, (System.Data.SqlClient.SqlConnection)_Conn);
                    cmd.Transaction = _Trans;
                } else {
                    cmd = new System.Data.SqlClient.SqlCommand(SQL);
                    cmd.Connection = OpenConn(_ConnectStr);
                }

                return cmd;
            } catch (Exception) {
                return null;
            }
        }

        public void ClearCommand(ref dynamic cmd, dynamic _Conn = null) {
            if ((cmd != null)) {
                if (_Conn == null) {
                    dynamic Conn = cmd.Connection;
                    CloseConn(ref Conn);
                    cmd.Connection = Conn;
                }
                cmd.Dispose();
                cmd = null;
            }
        }

        public dynamic CreateDataAdapter(string SQL = "", dynamic _Conn = null, dynamic _Trans = null)
        {
            dynamic DA = null;
            try
            {
                if ((_Conn != null))
                {
                    DA = new System.Data.SqlClient.SqlDataAdapter(SQL, (System.Data.SqlClient.SqlConnection)_Conn);
                    if ((_Trans != null))  {
                        DA.SelectCommand.Transaction = (System.Data.SqlClient.SqlTransaction)_Trans;
                    }
                } else{
                    DA = new System.Data.SqlClient.SqlDataAdapter(SQL, ConnectStr);
                }

                return DA;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public void ClearDataAdapter(ref dynamic DA, dynamic _Conn = null)
        {
            if ((DA != null)) {
                if ((_Conn == null) && (DA.SelectCommand != null)) {
                    dynamic Conn = DA.SelectCommand.Connection;
                    CloseConn(ref Conn);
                    DA.SelectCommand.Connection = Conn;
                }
                DA.Dispose();
                DA = null;
            }
        }

        public DataTable QueryData(string SQL, dynamic _Conn = null, dynamic _Trans = null)
        {
            DataTable DT = new DataTable();
            try
            {
                if ((_Conn == null)) {
                    OpenDT(ref DT, SQL, _Conn, _Trans);
                } else {
                    OpenDT(ref DT, SQL);
                }

                return DT;
            } catch (Exception ex)  {
                throw (ex);
            } finally {
                DT = null;
            }
        }

        public int ExecuteSQL(string SQL, dynamic _Conn = null, dynamic _Trans = null)
        {
            dynamic cmd = null;
            int rows = 0;

            try {
                cmd = CreateCommand(SQL, _Conn, _Trans);
                rows = cmd.ExecuteNonQuery();

                return rows;
            } catch (Exception ex) {
                throw ex;
            } finally {
                ClearCommand(ref cmd, _Conn);
            }
        }

        public object LookupSQL(string SQL, dynamic Conn = null, dynamic Trans = null)
        {
            dynamic tmpConn = Conn;
            dynamic cmd = null;
            object Value = "";
            try
            {
                if ((Conn == null))
                    tmpConn = OpenConn(_ConnectStr);
                cmd = CreateCommand(SQL, tmpConn);
                if ((Trans != null))
                {
                    cmd.Transaction = Trans;
                }
                Value = cmd.ExecuteScalar();
                cmd.Dispose();
                cmd = null;
                if ((Conn == null))
                    CloseConn(ref tmpConn);

                return Value;
            }
            catch (Exception ex)
            {
                if ((Conn == null))
                    CloseConn(ref tmpConn);
                cmd.Dispose();
                cmd = null;
                throw ex;
            }
        }

        public DataRow OpenDT(ref DataTable DT, string SQL, dynamic _Conn = null, dynamic _Trans = null)
        {
            dynamic DA = null;
            try {
                if (DT == null) {
                    DT = new DataTable();
                } else {
                    DT.Clear();
                }

                DA = CreateDataAdapter(SQL, _Conn, _Trans);
                DA.Fill(DT);

                if ((DT.Rows.Count > 0)) {
                    return DT.Rows[0];
                } else {
                    return null;
                }
            } catch (Exception ex) {
                throw (ex);
            } finally {
                ClearDataAdapter(ref DA, _Conn);
            }
        }

        public string SQLValue(object Value, FieldTypes DataType)
        {
            string result = null;
            string S = Utilities.ToString(Value);
            string T = "";
            if (S == "" || S == "NULL")
            {
                result = "NULL";
            }
            else
            {
                T = Value.GetType().ToString().ToUpper();
                switch (DataType)
                {
                    case FieldTypes.ftDate:
                        if (T.StartsWith("SYSTEM.DATE")) {
                            result = SQLDate((DateTime)Value);
                        } else {
                            result = Value.ToString().ToUpper();
                        }
                        break;
                    case FieldTypes.ftDateTime:
                        if (T.StartsWith("SYSTEM.DATE")) {
                            result = SQLDateTime((DateTime)Value);
                        } else {
                            result = Value.ToString().ToUpper();
                        }
                        break;
                    case FieldTypes.ftNumeric:
                        if (Utilities.IsNumeric(Value)) {
                            result = Utilities.ToNum(Value).ToString();
                        } else if (Value.ToString().ToUpper().EndsWith("NEXTVAL")) {
                            result = Value.ToString();
                        } else {
                            result = "NULL";
                        }
                        break;
                    case FieldTypes.ftText:
                        result = "'" + Value.ToString().Replace("'", "''") + "'";
                        break;
                    default:
                        result = Value.ToString();
                        break;
                }
            }
            return result;

        }


        public void AddSQL(int operation, ref string SQL1, ref string SQL2, string FieldName, object FieldValue, FieldTypes ColType)
        {
            string Data = null;

            if (!string.IsNullOrEmpty(FieldName))
            {
                Data = Convert.ToString(SQLValue(FieldValue, ColType));
                if (operation == opINSERT)
                {
                    if (!string.IsNullOrEmpty(SQL1))
                    {
                        SQL1 = SQL1 + ", ";
                        SQL2 = SQL2 + ", ";
                    }
                    SQL1 = SQL1 + FieldName;
                    SQL2 = SQL2 + Data;
                }
                else
                {
                    if (!string.IsNullOrEmpty(SQL1))
                        SQL1 = SQL1 + ", ";
                    SQL1 = SQL1 + FieldName + "=" + Data;
                }
            }
        }

        public void AddCriteria(ref string CriteriaSQL, string FieldName, object FieldValue, FieldTypes FieldType, bool AllowIN = false)
        {
            string Oper = "=";
            string FVal = null;
            string[] ValList = null;
            string V = null;

            FVal = (FieldValue + "").Replace("*", "%");

            FVal = Convert.ToString(FieldValue) + "";
            if (FVal.ToUpper().StartsWith("IN ("))
            {
                Oper = " IN ";
                ValList = FVal.Trim().Substring(4, FVal.Length - 5).Split(',');
                FVal = "";
                foreach (string VI in ValList)
                {
                    V = VI;
                    switch (FieldType)
                    {
                        case FieldTypes.ftNumeric:
                            if (Utilities.IsNumeric(V))  {
                                V = Convert.ToString(V.Replace(",", ""));
                            }
                            break;
                        case FieldTypes.ftText:
                            if (V != "NULL") {
                                V = "'" + V.Replace("'", "''") + "'";
                            }
                            break;
                        case FieldTypes.ftDate:
                            if (Utilities.IsDate(V)) {
                                V = SQLDate(Convert.ToDateTime(V));
                            }
                            break;
                        case FieldTypes.ftDateTime:
                            if (Utilities.IsDate(V)) {
                                V = SQLDateTime(Convert.ToDateTime(V));
                            }
                            break;
                    }
                    if (!string.IsNullOrEmpty(V))
                        FVal += "," + V;
                }
                if (!string.IsNullOrEmpty(FVal))
                {
                    FVal = "(" + FVal.Substring(1) + ")";
                }
            }
            else if (!string.IsNullOrEmpty(FVal))
            {
                if ((FVal.IndexOf("%") >= 0))
                {
                    Oper = " LIKE ";
                    FieldType = FieldTypes.ftText;
                }
                if (FVal.StartsWith("<"))
                {
                    if (FVal.Substring(1, 1) == ">")
                    {
                        Oper = "<>";
                        FieldValue = FVal.Substring(2);
                    }
                    else if (FVal.Substring(1, 1) == "=")
                    {
                        Oper = "<=";
                        FieldValue = FVal.Substring(2);
                    }
                    else
                    {
                        Oper = "<";
                        FieldValue = FVal.Substring(1);
                    }
                }
                else if (FVal.StartsWith(">"))
                {
                    if (FVal.Substring(1, 1) == "=")
                    {
                        Oper = ">=";
                        FieldValue = FVal.Substring(2);
                    }
                    else
                    {
                        Oper = ">";
                        FieldValue = FVal.Substring(1);
                    }
                }
                else if (FVal.StartsWith("="))
                {
                    Oper = "=";
                    FieldValue = FVal.Substring(1);
                }

                switch (FieldType)
                {
                    case FieldTypes.ftNumeric:
                        if (Utilities.IsNumeric(FieldValue))
                        {
                            FVal = Convert.ToString(FieldValue).Replace(",", "");
                        }
                        break;
                    case FieldTypes.ftText:
                        if (AllowIN && FVal.IndexOf(",") >= 0)
                        {
                            Oper = " IN ";
                            FVal = "('" + FieldValue.ToString().Replace(",", "', '") + "')";
                        }
                        else
                        {
                            FVal = "'" + FieldValue.ToString().Replace("'", "''") + "'";
                        }
                        break;
                    case FieldTypes.ftDate:
                        if (Utilities.IsDate(FieldValue) && (Convert.ToDouble(Convert.ToDateTime(FieldValue).ToOADate()) > 0))
                        {
                            FVal = SQLDate(Convert.ToDateTime(FieldValue));
                        }
                        break;
                    case FieldTypes.ftDateTime:
                        if (Utilities.IsDate(FieldValue) && (Convert.ToDouble(Convert.ToDateTime(FieldValue).ToOADate()) > 0))
                        {
                            FVal = SQLDateTime(Convert.ToDateTime(FieldValue));
                        }
                        break;
                }

            }

            if (!string.IsNullOrEmpty(FVal))
            {
                if (!string.IsNullOrEmpty(CriteriaSQL))
                    CriteriaSQL += " AND ";
                CriteriaSQL += FieldName + Oper + FVal;
            }
        }

        public void AddCriteriaRange(ref string CriteriaSQL, string FieldName, object FromValue, object ToValue, FieldTypes FieldType)
        {
            string FromVal = "";
            string ToVal = "";

            if (!string.IsNullOrEmpty(FromValue + ""))
            {
                switch (FieldType) {
                    case FieldTypes.ftNumeric:
                        if (Utilities.IsNumeric(FromValue))
                            FromVal = Convert.ToString(FromValue);
                        if (Utilities.IsNumeric(ToValue))
                            ToVal = Convert.ToString(ToValue);
                        break;
                    case FieldTypes.ftText:
                        FromVal = "'" + FromValue.ToString().Replace("'", "''") + "'";
                        if (!string.IsNullOrEmpty(ToValue + ""))
                            ToVal = "'" + ToValue.ToString().Replace("'", "''") + "'";
                        break;
                    case FieldTypes.ftDate:
                        if (Utilities.IsDate(FromValue) && (Convert.ToDouble(Convert.ToDateTime(FromValue).ToOADate()) > 0))
                            FromVal = SQLDate(Convert.ToDateTime(FromValue));
                        if (Utilities.IsDate(ToValue) && (Convert.ToDouble(Convert.ToDateTime(ToValue).ToOADate()) > 0))
                            ToVal = SQLDate(Convert.ToDateTime(ToValue).AddDays(1));
                        break;
                    case FieldTypes.ftDateTime:
                        if (Utilities.IsDate(FromValue) && (Convert.ToDouble(Convert.ToDateTime(FromValue).ToOADate()) > 0))
                            FromVal = SQLDateTime(Convert.ToDateTime(FromValue));
                        if (Utilities.IsDate(ToValue) && (Convert.ToDouble(Convert.ToDateTime(ToValue).ToOADate()) > 0))
                            ToVal = SQLDateTime(Convert.ToDateTime(ToValue).AddDays(1));
                        break;
                }
            }

            if (!string.IsNullOrEmpty(FromVal + "")) {
                if (string.IsNullOrEmpty(ToVal + "")) {
                    AddCriteria(ref CriteriaSQL, FieldName, FromValue, FieldType);
                } else {
                    if (!string.IsNullOrEmpty(CriteriaSQL))
                        CriteriaSQL += " AND ";
                    if (FieldType == FieldTypes.ftDate)
                    {
                        CriteriaSQL += "(" + FieldName + ">=" + FromVal + " AND " + FieldName + "<" + ToVal + ")";
                    }
                    else
                    {
                        CriteriaSQL += "(" + FieldName + " BETWEEN " + FromVal + " AND " + ToVal + ")";
                    }
                }
            }
        }

        public string CombineSQL(int operation, ref string SQL1, ref string SQL2, string TableName, string CriteriaSQL, bool TimeStamp = true)
        {
            string SQL = "";

            switch (operation) {
                case opINSERT:
                    if (TimeStamp) {
                        AddSQL(operation, ref SQL1, ref SQL2, "DateUpdated", System.DateTime.Now, FieldTypes.ftDateTime);
                        AddSQL(operation, ref SQL1, ref SQL2, "UserUpdated", Project.IsMe().UserName.ToUpper(), FieldTypes.ftText);
                    }

                    SQL = "INSERT INTO " + TableName + " (" + SQL1 + ") VALUES (" + SQL2 + ")";
                    break;
                case opUPDATE:
                    if (TimeStamp)
                    {
                        AddSQL(operation, ref SQL1, ref SQL2, "DateUpdated", System.DateTime.Now, FieldTypes.ftDateTime);
                        AddSQL(operation, ref SQL1, ref SQL2, "UserUpdated", Project.IsMe().UserName.ToUpper(), FieldTypes.ftText);
                    }

                    SQL = "UPDATE " + TableName + " SET " + SQL1;
                    if (CriteriaSQL.IndexOf("WHERE") + 1 > 0)
                    {
                        if (!CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                        {
                            SQL = SQL + " AND " + CriteriaSQL;
                        }
                        else
                        {
                            SQL = SQL + " " + CriteriaSQL;
                        }
                    }
                    else if (!string.IsNullOrEmpty(CriteriaSQL))
                    {
                        if (CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                        {
                            SQL = SQL + " WHERE " + CriteriaSQL.Trim().Substring(4);
                        }
                        else
                        {
                            SQL = SQL + " WHERE " + CriteriaSQL;
                        }
                    }
                    break;
                case opDELETE:
                    SQL = "DELETE FROM " + TableName;
                    if (!string.IsNullOrEmpty(CriteriaSQL))
                    {
                        if (CriteriaSQL.Trim().ToUpper().StartsWith("AND")) {
                            SQL = SQL + " WHERE " + CriteriaSQL.Trim().Substring(4);
                        }else {
                            SQL = SQL + " WHERE " + CriteriaSQL;
                        }
                    }

                    break;
            }
            return SQL;
        }

        public string SQLDateTime(System.DateTime DT) {
            string result = null;
            int Y = 0;

            if ((DT != null) && (Convert.ToDouble(DT.ToOADate()) > 0)) {
                Y = DT.Year;
                if (Y > 2500)
                    Y -= 543;
                result = "'" + Y + DT.ToString("-MM-dd HH:mm:ss") + "'";
            } else {
                result = "NULL";
            }
            return result;
        }

        public string SQLDate(System.DateTime D) {
            string result = null;
            int Y = 0;
            if ((D != null) && (Convert.ToDouble(D.ToOADate()) > 0))
            {
                result = "convert(datetime," + (Convert.ToDouble(D.ToOADate()) - 2) + ")";
            } else {
                result = "NULL";
            }
            return result;
        }


    }
}