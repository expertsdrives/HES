using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http;
using HESMDMS.Models;
using Newtonsoft.Json.Converters;

using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNet.SignalR.Messaging;

namespace HESMDMS.Controllers
{
    public class MobileAppController : ApiController
    {
        SmartMeterEntities clsDB = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
        SmartMeter_ProdEntities clsMeters_Prod1 = new SmartMeter_ProdEntities();
        [HttpGet]
        [Route("LoginAPI")]
        public HttpResponseMessage Login(string username, string password)
        {
            var loginCred = clsDB.tbl_AdminCredentials.Where(c => c.Username == username && c.Password == password).Count();
            if (loginCred > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, loginCred);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, loginCred);
            }
            //return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerData")]
        public HttpResponseMessage GetCustomerData()
        {
            var loginCred = clsDB.FetchConsumption().OrderByDescending(x => x.Date).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetails")]
        public HttpResponseMessage GetCustomerDetails()
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdoo().OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetails1")]
        public HttpResponseMessage GetCustomerDetails1()
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdoo().Where(x => x.MonthOfSales == "July" || x.MonthOfSales == "August").OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetailsDateWise")]
        public HttpResponseMessage GetCustomerDetailsDateWise(DateTime startdate, DateTime enddate)
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdooDateWise(startdate, enddate).OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetailsDateWise1")]
        public HttpResponseMessage GetCustomerDetailsDateWise1(DateTime startdate, DateTime enddate)
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdooDateWise(startdate, enddate).OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("SendDataJio")]
        public async Task<HttpResponseMessage> SendDataJio(string aid, string pld, string eid, string eventname, string data)
        {
            var resp = clsDB.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            Random rnd = new Random();
            double rndNumber = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
            int size = rndNumber.ToString().Length;
            if (size != 12)
            {
                rndNumber = Convert.ToDouble(string.Format("{0}{1}", rndNumber, 0));
            }
            var bob = new
            {
                idType = "PLD",
                id = pld,
                transactionId = rndNumber.ToString(),
                retentionTime = DateTime.Now.ToString(),
                data = new[] {
                 new  { aid =aid, dataformat = "cp",dataType="JSON",
                 ext="{'data': '"+data+"'}"
                 },

            }
            };
            string stringjson = JsonConvert.SerializeObject(bob);
            Uri requestUri = new Uri("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login"); //replace your Url  
            var converter = new ExpandoObjectConverter();
            c2dProd users = new c2dProd();
            users.grant_type = "password";
            users.username = "2025000_2890001@iot.jio.com";
            users.password = "a737b902951ec15cff735357a850b09cd941818095527a1925760b5a4e471464";
            users.client_id = "db2f04a5e72547cbb68331f406946494";
            users.client_secret = "d95578aa9b1eb30e";
            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            var objClint = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage respon = await objClint.PostAsync(requestUri, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            string responJsonText = await respon.Content.ReadAsStringAsync();
            var bearerToken = JsonConvert.DeserializeObject<c2dTokenProd>(responJsonText);
            var logdate = DateTime.UtcNow;
            if (Convert.ToString(bearerToken.access_token) != null)
            {
                Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified"); //replace your Url  

                var content = JsonConvert.SerializeObject(bob);

                var objClint1 = new System.Net.Http.HttpClient();
                objClint1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objClint1.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.access_token);
                objClint1.DefaultRequestHeaders.Add("eid", eid);
                System.Net.Http.HttpResponseMessage respon1 = await objClint1.PostAsync(requestUri1, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                var responJsonText1 = respon1.Content.ReadAsStringAsync();
                Thread.Sleep(15000);
                int start = Convert.ToInt32(resp.StartingPostion);
                int end = Convert.ToInt32(resp.EndingPostion);
                var hexOutput = getData(pld, resp.Code, start, end, logdate, data, eventname);
                if (hexOutput != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, hexOutput, Configuration.Formatters.JsonFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, hexOutput, Configuration.Formatters.JsonFormatter);
                }
            }
            return Request.CreateResponse(HttpStatusCode.NoContent, "", Configuration.Formatters.JsonFormatter);

        }
        public string getData(string pld, string code, int startPostion, int endPostion, DateTime logdate, string data, string eventname)
        {
            var PreviousData = "";
            DateTime dt = DateTime.UtcNow;
            var res = clsMeters_Prod.sp_MeterResponse().Where(x => x.pld == pld && x.LogDate == logdate.Date).OrderByDescending(x => x.ID).ToList();
            FetchJioLogs fetchLogs = new FetchJioLogs();
            var LogsData = "";

            foreach (var fData in res)
            {
                try
                {
                    if (fData.Data.Contains(code))
                    {
                        //DateTime resDate = Convert.ToDateTime(fData.Data.Split(',')[1] + " " + fData.Data.Split(',')[2]);
                        //resDate = Convert.ToDateTime(fData.LogDate);
                        //dt = Convert.ToDateTime(Convert.ToDateTime(fData.LogDate).ToString("dd-MM-yyy HH:mm:ss"));
                        //if (logdate.Kind != DateTimeKind.Utc)
                        //    logdate = logdate.AddHours(-5).AddMinutes(-30);
                        //if (Convert.ToDateTime(resDate.ToString("dd-MM-yyy HH:mm:ss")) >= Convert.ToDateTime(logdate.ToString("dd-MM-yyy HH:mm:ss")))
                        //{
                        LogsData = fData.Data;
                        break;
                        //}
                    }
                }
                catch (Exception ex) { }

            }
            string hexOutput = "";
            if (LogsData != "")
            {
                string[] splitoutput = Convert.ToString(LogsData).Split(',');

                for (int i = startPostion + 3; i <= endPostion + 3; i++)
                {
                    hexOutput += splitoutput[i];
                }
                TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                // Convert UTC time to Indian Standard Time
                DateTime currentTimeIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);
                clsMeters_Prod1.Database.ExecuteSqlCommand("UPDATE tbl_CommandBackLog SET Status = 'Completed',CompletedLogDate='" + DateTime.Now + "' WHERE [pld] = '" + pld + "' and EventName='" + eventname + "'");
            }

            var MeterMaster = clsDB.tbl_SMeterMaster.Where(x => x.PLD == pld).FirstOrDefault();

            bool logs = c2d.SGMAuditLogs(pld, data, MeterMaster.MeterID, eventname);

            if (hexOutput != "")
                return hexOutput;
            else
                return LogsData + dt.ToString() + "-" + logdate.ToString();
        }
    }
}
