using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Selectcon
{
    public partial class TestWS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = null;
            try
            {
                    dt = Project.dal.QueryData(txtQ.Text);
                    dt.Rows.Add(dt.NewRow());

                    Session["dt"] = dt;
                    Bind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        protected void btnExecu_Click(object sender, EventArgs e)
        {
            try
            {
                Project.dal.ExecuteSQL(txtExecu.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void Bind() {
            gvData.DataSource = Session["dt"];
            gvData.DataBind();
        }

    }
}