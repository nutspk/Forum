using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Selectcon
{
    public partial class Posted : System.Web.UI.Page
    {
        public string SiteKey = Project.SiteKey;
        public string QuestionID;
        protected void Page_Load(object sender, EventArgs e)
        {
            QuestionID = Validations.GetParamStr("k");

            if (!IsPostBack) {
                LoadData();
            }
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
    }
}