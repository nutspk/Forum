using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace Selectcon.Models
{
    public class Tags
    {
        [JsonProperty(PropertyName = "TagID")]
        public int TagID { get; set; }

        [JsonProperty(PropertyName = "TagDesc")]
        public string TagDesc { get; set; }

        [JsonProperty(PropertyName = "ShowFlag")]
        public string ShowFlag { get; set; }


        public IList<Tags> GetTagData()
        {
            DataTable dt = new DataTable();
            string sql = "";
            var Tags = new List<Tags>();
            string Criteria = "";
            try
            {
                sql = "SELECT * FROM TAGS WHERE ShowFlag=1 ";

                dt = Project.dal.QueryData(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Tags = dt.AsEnumerable().Select(dr =>
                        new Tags
                        {
                            TagID = (int)dr["TagID"],
                            TagDesc = dr["TagDesc"] + "",
                            ShowFlag = dr["ShowFlag"] + ""
                        }
                    ).ToList<Tags>();
                }

                return Tags;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}