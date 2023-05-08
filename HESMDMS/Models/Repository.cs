using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class Repository
    {
        public string GetAllMessages(string pld)
        {
            var LogsData = "";
            SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlDependency.Start(connString);
            using (var cmd = new SqlCommand(@"SELECT * from tbl_JioLogs Order by ID DESC", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
              
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    FetchJioLogs deserializedProduct = JsonConvert.DeserializeObject<FetchJioLogs>(ds.Tables[0].Rows[i][3].ToString());
                    List<ModelParameter> model = new List<ModelParameter>();
                    if (deserializedProduct.pld == pld)
                    {

                        LogsData = deserializedProduct.Data;
                        break;
                    }
                }
            }
            return LogsData;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table. 
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendMessages();
            }
        }
    }
}