using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Selectcon
{
    public partial class Category : System.Web.UI.Page
    {
        public bool IsAdmin = Project.IsAdmin;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                LoadData();
            }
            
            NewCate.Visible = Project.IsAdmin;
        }

        private void LoadData() {
            DataTable dt = null;
            try {
               

            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        protected void btnAddCate_Click(object sender, EventArgs e)
        {
            string SQL = "", SQL1 = "", SQL2 = "", Criteria = "";
            try {
                if (txtTitle.Text != "" && Project.IsAdmin) {

                    Project.dal.AddSQL(dbUtilities.opINSERT, ref SQL1, ref SQL2, "CategoryName", txtTitle.Text, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(dbUtilities.opINSERT, ref SQL1, ref SQL2, "CategoryDesc", txtDesc.Text, dbUtilities.FieldTypes.ftText);
                    Project.dal.AddSQL2(dbUtilities.opINSERT, ref SQL1, ref SQL2, "IsPublic", "1", dbUtilities.FieldTypes.ftNumeric);
                    Project.dal.AddSQL2(dbUtilities.opINSERT, ref SQL1, ref SQL2, "ActiveFlag", "1", dbUtilities.FieldTypes.ftNumeric);

                    SQL = Project.dal.CombineSQL(dbUtilities.opINSERT, ref SQL1, ref SQL2, "CATEGORIES", Criteria);
                    Project.dal.ExecuteSQL(SQL, null, null);
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}