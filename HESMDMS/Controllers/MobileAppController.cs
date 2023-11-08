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
using Syncfusion.EJ2.ProgressBar;
using System.Text;
using System.Web.UI.WebControls;
using HESMDMS.Areas.SmartMeter.Controllers;

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
        [Route("EXECommand")]
        public HttpResponseMessage EXECommand(string aid, string pld, string eid, string eventname, string data)
        {
            try
            {
                tbl_BackLogAPILogs clsAPI1 = new tbl_BackLogAPILogs();
                clsAPI1.Status = "Started";
                clsAPI1.LogDate = DateTime.UtcNow;
                clsAPI1.code = "0";
                clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI1);
                clsMeters_Prod.SaveChanges();
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
                HttpResponseMessage response1 = objClint.PostAsync("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login",new StringContent(json, System.Text.Encoding.UTF8, "application/json")).Result;

                //System.Net.Http.HttpResponseMessage respon =  objClint.PostAsync(requestUri, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                string responJsonText = response1.Content.ReadAsStringAsync().Result;
                var bearerToken = JsonConvert.DeserializeObject<c2dTokenProd>(responJsonText);

                Console.WriteLine(bearerToken);
                if (Convert.ToString(bearerToken.access_token) != null)
                {
                    tbl_BackLogAPILogs clsAPI2 = new tbl_BackLogAPILogs();
                    clsAPI2.Status = "bearerToken";
                    clsAPI2.LogDate = DateTime.UtcNow;
                    clsAPI2.code = stringjson.ToString();
                    clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI2);
                    clsMeters_Prod.SaveChanges();
                    var content1 = JsonConvert.SerializeObject(bob);
                    using (HttpClient client = new HttpClient())
                    {
                        tbl_BackLogAPILogs clsAPI3 = new tbl_BackLogAPILogs();
                        clsAPI3.Status = "client";
                        clsAPI3.LogDate = DateTime.UtcNow;
                        clsAPI3.code = content1;
                        clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI3);
                        clsMeters_Prod.SaveChanges();
                        // Create the request content with JSON data
                        StringContent content = new StringContent(content1, Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.access_token);
                        client.DefaultRequestHeaders.Add("eid", eid);
                        // Send the POST request and block until the response is received
                        HttpResponseMessage response = client.PostAsync("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified", new StringContent(content1, System.Text.Encoding.UTF8, "application/json")).Result;
                        tbl_BackLogAPILogs clsAPI4 = new tbl_BackLogAPILogs();
                        clsAPI4.Status = "client1";
                        clsAPI4.LogDate = DateTime.UtcNow;
                        clsAPI4.code = response.Content.ReadAsStringAsync().Result;
                        clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI4);
                        clsMeters_Prod.SaveChanges();
                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response content as a string
                            string responseJson = response.Content.ReadAsStringAsync().ToString();
                            var responJsonText1 = response.Content.ReadAsStringAsync();
                            // Process the response data
                            Console.WriteLine("Response: " + responseJson);
                            tbl_BackLogAPILogs clsAPI7 = new tbl_BackLogAPILogs();
                            clsAPI7.Status = "Ended";
                            clsAPI7.LogDate = DateTime.UtcNow;
                            clsAPI7.pld = pld;
                            clsAPI7.code = responseJson;
                            clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI7);
                            clsMeters_Prod.SaveChanges();
                        }
                        else
                        {
                            tbl_BackLogAPILogs clsAPI8 = new tbl_BackLogAPILogs();
                            clsAPI8.Status = "Ended";
                            clsAPI8.LogDate = DateTime.UtcNow;
                            clsAPI8.pld = pld;
                            clsAPI8.code = response.StatusCode.ToString();
                            clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI8);
                            clsMeters_Prod.SaveChanges();
                            // Request failed, display the status code
                            Console.WriteLine("Error: " + response.StatusCode);
                        }
                    }
                    //Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified"); //replace your Url  

                    //var content = JsonConvert.SerializeObject(bob);
                    //var logdate = DateTime.Now;
                    //var objClint1 = new System.Net.Http.HttpClient();
                    //objClint1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //objClint1.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.access_token);
                    //objClint1.DefaultRequestHeaders.Add("eid", eid);
                    //System.Net.Http.HttpResponseMessage respon1 = await objClint1.PostAsync(requestUri1, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                    //var responJsonText1 = respon1.Content.ReadAsStringAsync();
                    
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Command Executed", Configuration.Formatters.JsonFormatter);
                //tbl_BackLogAPILogs clsAPI = new tbl_BackLogAPILogs();
                //clsAPI.Status = "Called";
                //clsAPI.LogDate = DateTime.UtcNow;
                //clsAPI.pld = pld;
                //clsAPI.code = "";
                //clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI);
                //clsMeters_Prod.SaveChanges();
                //var response = await c2d.SendData(aid, pld, eid, eventname, data, "api");
                //tbl_BackLogAPILogs clsAPI1= new tbl_BackLogAPILogs();
                //clsAPI1.Status = response;
                //clsAPI1.LogDate = DateTime.UtcNow;
                //clsAPI1.code = "";
                //clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI1);
                //clsMeters_Prod.SaveChanges();
                //if (response != "")
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK, "Command Executed", Configuration.Formatters.JsonFormatter);
                //}
                //else
                //{
                //    return Request.CreateResponse(HttpStatusCode.NoContent, "", Configuration.Formatters.JsonFormatter);
                //}
            }
            catch (Exception ex)
            {
                tbl_BackLogAPILogs clsAPI3 = new tbl_BackLogAPILogs();
                clsAPI3.Status = "catch";
                clsAPI3.LogDate = DateTime.UtcNow;
                clsAPI3.code = ex.ToString();
                clsMeters_Prod.tbl_BackLogAPILogs.Add(clsAPI3);
                clsMeters_Prod.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.NoContent, "", Configuration.Formatters.JsonFormatter);
            }

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

            }

            var MeterMaster = clsMeters_Prod.tbl_SMeterMaster.Where(x => x.PLD == pld).FirstOrDefault();

            bool logs = c2d.SGMAuditLogs(pld, data, MeterMaster.TempMeterID, eventname);

            if (hexOutput != "")
                return hexOutput;
            else
                return LogsData + dt.ToString() + "-" + logdate.ToString();
        }
    }
}
