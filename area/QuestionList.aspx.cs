using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Selectcon
{
    public partial class QuestionList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                Init();
            }
        }

        private void Init() {
            try {
                NewAsk.Visible = (Project.IsMe().RoleID >= 1);
                cID.Value = Validations.GetParamStr("k");
            }
            catch (Exception ex) {

                throw ex;
            }
        }

        private void LoadData() {
            DataTable dt = null;
            try {
                //dt = Project.dal.QueryData("SELECT * FROM Topic");

            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}