using Elmah;
using HESMDMS.Models;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Syncfusion.Lic.util.encoders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class TerminalController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        private readonly IHttpClientFactory _clientFactory;
        // GET: SmartMeter/Terminal
        public ActionResult Index()
        {

            var data = clsMeters.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();
        }


        public ActionResult Terminal()
        {
            var data = clsMeters.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();

        }
        [HttpPost]
        public async Task<JsonResult> SendData(string aid, string pld, string eid, string eventname, string modetype, string balanceInput)
        {
            if (modetype == "auto")
            {
                var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
                var data = command.Command;
                var logdate = DateTime.Now;
                DateTime time = DateTime.UtcNow;
                var res = clsMetersProd.tbl_Response.Where(x => x.pld == pld).OrderByDescending(x => x.ID).FirstOrDefault();
                DateTime lDate = Convert.ToDateTime(res.LogDate);
                TimeSpan difference = time - lDate;
                if (difference.TotalSeconds < 18)
                {
                    var response = await c2d.SendData(aid, pld, eid, eventname, balanceInput, modetype);
                    if (response.ToString() != "error")
                        return Json(response, JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else if (eventname.Contains("Get"))
                {
                    int start = Convert.ToInt32(command.StartingPostion);
                    int end = Convert.ToInt32(command.EndingPostion);
                    var hexOutput = getData(pld, command.Code, start, end, logdate, data, eventname, balanceInput);
                    //var balanceString = 0.0;
                    if (hexOutput != null)
                        return Json(hexOutput, JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int start = Convert.ToInt32(command.StartingPostion);
                    int end = Convert.ToInt32(command.EndingPostion);
                    var hexOutput = getData(pld, command.Code, start, end, logdate, data, eventname, balanceInput);
                    //var balanceString = 0.0;
                    if (hexOutput != null)
                        return Json(hexOutput, JsonRequestBehavior.AllowGet);
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //bool chckMalua = await c2d.checkManualMode("00,00", aid, pld, eid);
                //if (chckMalua)
                //{
                    var response = await c2d.SendData(aid, pld, eid, eventname, balanceInput, modetype);
                    if (response.ToString() != "error")
                        return Json(response, JsonRequestBehavior.AllowGet);
                    return Json(response, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json("Manual Mode Not Activated", JsonRequestBehavior.AllowGet);
                //}
            }
        }
        [HttpPost]
        public async Task<JsonResult> SendDataAddBalance(string aid, string pld, string eid, string eventname, string balanceInput)
        {
            var logdate = DateTime.Now;
            DateTime time = DateTime.UtcNow;
            var res = clsMetersProd.tbl_Response.Where(x => x.pld == pld).OrderByDescending(x => x.ID).FirstOrDefault();
            DateTime lDate = Convert.ToDateTime(res.LogDate);
            TimeSpan difference = time - lDate;
            var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            var data = command.Command;
            float intValue = 0;
            if (eventname.Contains("Set Vat"))
                intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
            else
                intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
            string balanceString = ToHexString(intValue);
            var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
            string modifiedString = data.Replace("-", balanceutput);
            data = modifiedString;
            Random rnd = new Random();
            double rndNumber = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
            int size = rndNumber.ToString().Length;
            if (size != 12)
            {
                rndNumber = Convert.ToDouble(string.Format("{0}{1}", rndNumber, 0));
            }
            if (difference.TotalSeconds < 18)
            {
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
                //APIData apiData = new APIData()
                //{
                //    idType = "PLD",
                //    id = pld,
                //    transactionId = rndNumber.ToString(),
                //    retentionTime = DateTime.Now.ToString(),
                //    data = ""
                //};
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

                Console.WriteLine(bearerToken);
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
                    MyHub.SendMessages();
                    logdate = DateTime.Now;
                    Thread.Sleep(8000);
                }



            }
            int start = Convert.ToInt32(command.StartingPostion);
            int end = Convert.ToInt32(command.EndingPostion);
            var hexOutput = getData(pld, command.Code, start, end, logdate, data, eventname, balanceInput);
            //var balanceString = 0.0;
            if (hexOutput != null)
            {
                return Json(hexOutput, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> SendDataAddTariff(string aid, string pld, string eid, string eventname, string date, string time, string Tariff)
        {
            DateTime datetime = Convert.ToDateTime(date + " " + time);
            string inputDate = Convert.ToDateTime(datetime).ToString("ddMMyyHHmmss");
            string hex = "";
            for (int i = 0; i < inputDate.Length; i += 2)
            {
                string pair = inputDate.Substring(i, 2);
                int value = int.Parse(pair);

                // convert the decimal value to a two-digit hexadecimal string
                string hexPair = value.ToString("X2");
                hex += hexPair;
            }
            var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            var data = command.Command;
            float intValue = 0;
            intValue = float.Parse(Tariff, CultureInfo.InvariantCulture.NumberFormat);
            string balanceString = ToHexString(intValue);
            var dateput = string.Join(",", SplitIntoChunks(hex, 2));
            var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
            string modifiedString = data.Replace("-", dateput + "," + balanceutput);
            data = modifiedString;
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
            //APIData apiData = new APIData()
            //{
            //    idType = "PLD",
            //    id = pld,
            //    transactionId = rndNumber.ToString(),
            //    retentionTime = DateTime.Now.ToString(),
            //    data = ""
            //};
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

            Console.WriteLine(bearerToken);
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
                MyHub.SendMessages();
                var logdate = DateTime.Now;
                Thread.Sleep(8000);
                int start = Convert.ToInt32(command.StartingPostion);
                int end = Convert.ToInt32(command.EndingPostion);
                var hexOutput = getData(pld, command.Code, start, end, logdate, data, eventname, "");
                //var balanceString = 0.0;
                if (hexOutput != null)
                {
                    return Json(hexOutput, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }


            }
            return Json("error", JsonRequestBehavior.AllowGet);

        }
        public JsonResult StringToHexNVE(string data)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }
            // Convert byte array to float
            float balanceString = BitConverter.ToSingle(bytes, 0);
            return Json(balanceString.ToString("F3"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult StringToHex(string data)
        {
            var balanceString = FromHexString(data.ToString());
            return Json(balanceString.ToString("F3"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TamperStatus(string data)
        {
            string output = string.Join(",", SplitIntoChunks(data, 2));
            string[] splitoutput = Convert.ToString(output).Split(',');
            var titlt = Convert.ToInt32("0x" + splitoutput[0], 16);
            var caseT = Convert.ToInt32("0x" + splitoutput[1], 16);
            var managetT = Convert.ToInt32("0x" + splitoutput[2], 16);
            var ExcessPush = Convert.ToInt32("0x" + splitoutput[3], 16);
            var Excessgas = Convert.ToInt32("0x" + splitoutput[4], 16);
            var SovStuck = Convert.ToInt32("0x" + splitoutput[5], 16);
            var Invalid = Convert.ToInt32("0x" + splitoutput[6], 16);
            var main = Convert.ToInt32("0x" + splitoutput[7], 16);
            var Rf = Convert.ToInt32("0x" + splitoutput[8], 16);
            return Json(titlt + "," + caseT + "," + managetT + "," + ExcessPush + "," + Excessgas + "," + SovStuck + "," + Invalid + "," + main + "," + Rf, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBalance(string data)
        {
            long longValue = long.Parse(data, System.Globalization.NumberStyles.HexNumber);
            double doubleValue = BitConverter.Int64BitsToDouble(longValue);
            var bal = doubleValue.ToString("F2");

            return Json(bal, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTariff(string data)
        {
            float doubleValue = FromHexString("0x" + data);
            var bal = doubleValue.ToString("F2");
            return Json(bal, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetVat(string data)
        {
            float bal = FromHexString("0x" + data) * 100;
            bal.ToString("F2");
            return Json(bal, JsonRequestBehavior.AllowGet);
        }
        public string ConvertHex(string hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    string hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    ulong decval = Convert.ToUInt64(hs, 16);
                    long deccc = Convert.ToInt64(hs, 16);
                    char character = Convert.ToChar(deccc);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }
        public JsonResult GetRTC(string data)
        {
            string op = "";
            string output = string.Join(",", SplitIntoChunks(data, 2));
            int cout = 0;
            var bytes = output.Split(',');
            foreach (var sdci in bytes)
            {
                if (sdci != "")
                {
                    op += ConvertHex(sdci);
                }
            }
            StringBuilder myStringBuilder = new StringBuilder(op);
            myStringBuilder = myStringBuilder.Insert(2, "-");
            myStringBuilder = myStringBuilder.Insert(5, "-");
            myStringBuilder = myStringBuilder.Insert(8, " ");
            myStringBuilder = myStringBuilder.Insert(11, ":");
            myStringBuilder = myStringBuilder.Insert(14, ":");
            return Json(myStringBuilder.ToString(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult VolategCalc(string data)
        {
            var hexString = (decimal)Int64.Parse(data, System.Globalization.NumberStyles.HexNumber);
            decimal voltage = hexString / 1000;
            return Json(voltage.ToString("F3"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult BatteryLife(string data)
        {
            var hexString = Convert.ToInt32(data, 16);
            int ndays = 0, year = 0, week = 0, days = 0, DAYSINWEEK = 7;
            ndays = hexString;
            year = ndays / 365;
            week = (ndays % 365) / DAYSINWEEK;
            days = (ndays % 365);
            return Json(year.ToString() + " Years " + days.ToString() + " days", JsonRequestBehavior.AllowGet);
        }
        public static IEnumerable<string> SplitIntoChunks(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }
        public string getData(string pld, string code, int startPostion, int endPostion, DateTime logdate, string data, string eventname, string balanceInput)
        {
            //DateTime currentTime = DateTime.Now;
            //DateTime time =DateTime.Now;
            //if (currentTime.Kind == DateTimeKind.Utc)
            //{
            //    time = DateTime.UtcNow;
            //}
            //else
            //{
            //    TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            //    // Convert UTC time to Indian Standard Time
            //    time =Convert.ToDateTime( time.AddHours(-5).AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"));
            //}
            var PreviousData = "";
            var res = clsMetersProd.tbl_Response.Where(x => x.pld == pld).OrderByDescending(x => x.ID).ToList();
            float intValue = 0;
            FetchJioLogs fetchLogs = new FetchJioLogs();
            var LogsData = "";
            if (eventname == "Add Balance" || eventname == "Set Vat")
            {
                if (eventname.Contains("Set Vat"))
                    intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
                else
                    intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
                string balanceString = ToHexString(intValue);
                var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
                string modifiedString = data.Replace("-", balanceutput);
                data = modifiedString;

            }
            string hexOutput = null;
            if (!eventname.Contains("Get"))
            {
                
                var cData = clsMetersProd.tbl_CommandBackLog;
                tbl_CommandBackLog clsCmd = new tbl_CommandBackLog();
                clsCmd.Data = data;
                clsCmd.EventName = eventname;
                clsCmd.LogDate = DateTime.UtcNow;
                clsCmd.Status = "Pending";
                clsCmd.pld = pld;
                clsMetersProd.tbl_CommandBackLog.Add(clsCmd);
                clsMetersProd.SaveChanges();
            }
            else
            {
                foreach (var fData in res)
                {
                    try
                    {
                        if (fData.Data.Contains(code))
                        {
                            DateTime resDate = Convert.ToDateTime(fData.Data.Split(',')[1] + " " + fData.Data.Split(',')[2]);
                            resDate = Convert.ToDateTime(fData.LogDate);
                            if (logdate.Kind != DateTimeKind.Utc)
                                logdate = logdate.AddHours(-5).AddMinutes(-30);

                            if (resDate >= logdate)
                            {
                                LogsData = fData.Data;
                                break;
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
                if (LogsData != "")
                {
                    string[] splitoutput = Convert.ToString(LogsData).Split(',');
                    for (int i = startPostion + 3; i <= endPostion + 3; i++)
                    {
                        hexOutput += splitoutput[i];
                    }
                }
            }

            return hexOutput;
        }
        public async Task<ActionResult> SendToAPIAsync(string aid, string pld, string data, string eid)
        {
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
            //APIData apiData = new APIData()
            //{
            //    idType = "PLD",
            //    id = pld,
            //    transactionId = rndNumber.ToString(),
            //    retentionTime = DateTime.Now.ToString(),
            //    data = ""
            //};
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

            Console.WriteLine(bearerToken);
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
                MyHub.SendMessages();
                return Json(responJsonText1.Result.ToString(), JsonRequestBehavior.AllowGet);

            }
            return Json(responJsonText, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMessages(string pld)
        {
            var PreviousData = "";
            var res = clsMetersProd.tbl_JioLogs.OrderByDescending(x => x.ID).Select(x => x.SMTPLResponse).ToList();

            FetchJioLogs fetchLogs = new FetchJioLogs();
            var LogsData = "";

            foreach (var fData in res)
            {
                if (IsValidJson(fData.ToString()))
                {
                    FetchJioLogs deserializedProduct = JsonConvert.DeserializeObject<FetchJioLogs>(fData);

                    List<ModelParameter> model = new List<ModelParameter>();
                    if (deserializedProduct.pld == pld)
                    {
                        if (PreviousData != deserializedProduct.Data)
                        {
                            PreviousData = deserializedProduct.Data;
                        }

                        LogsData = deserializedProduct.Data;
                        break;
                    }
                }
            }

            return Json(LogsData, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Demo()
        {
            return View();
        }
        public ActionResult BillingParameters()
        {
            var data = clsMeters.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();
        }
        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var js = new Newtonsoft.Json.JsonSerializer();
                    js.Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(strInput)));
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        unsafe string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }
        unsafe float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
        }

    }

    public class Response
    {
        public string Result { get; set; }
    }

}