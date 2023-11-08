using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Controllers
{
    public class SignalController : Controller
    {
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();

        SqlConnection con = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ProdEnt"].ConnectionString);
        public List<tbl_Response> GetAllMessages()
        {
            var messages = new List<tbl_Response>();
            using (var cmd = new SqlCommand(@"select * FROM [dbo].tbl_JioLogs", con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    messages.Add(item: new tbl_Response
                    {
                        ID = int.Parse(ds.Tables[0].Rows[i][0].ToString()),
                        Data = ds.Tables[0].Rows[i][1].ToString(),
                    });
                }
            }
            return messages;
        }
        private void dependency_OnChange(object sender, SqlNotificationEventArgs e) //this will be called when any changes occur in db table.    
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.SendMessages();
            }
        }
        // GET: Signal
        public ActionResult Index()
        {
            return View();
        }
   
        public ActionResult GetData()
        {
            var data = clsMetersProd.tbl_BackLogAPILogs.ToList(); // Replace with your entity name

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}