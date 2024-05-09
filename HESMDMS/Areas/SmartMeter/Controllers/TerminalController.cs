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
using System.Web.UI.WebControls;
using System.Xml.Linq;


namespace HESMDMS.Areas.SmartMeter.Controllers
{
    [SessionRequired]
    public class TerminalController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();

        private readonly IHttpClientFactory _clientFactory;

        // GET: SmartMeter/Terminal
        public ActionResult Index()
        {
            

                //var data = clsMetersProd.tbl_SMeterMaster.ToList();
                var data = (from log in clsMetersProd.tbl_SMeterMaster
                            select new
                            {
                                ID = log.ID,
                                TempMeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                                AID = log.AID,
                                PLD = log.PLD,
                            }).ToList();
                ViewBag.data = data;
            
            return View();
        }




        public ActionResult Terminal()
        {
            //var data = clsMetersProd.tbl_SMeterMaster.ToList();
            var data = (from log in clsMetersProd.tbl_SMeterMaster
                        select new
                        {
                            ID = log.ID,
                            TempMeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                            AID = log.AID,
                            PLD = log.PLD,
                        }).ToList();
            ViewBag.data = data;
            return View();

        }
        public class JSONData
        {
            public sp_ResponseSplited_Result Resposne { get; set; }
            public List<tbl_CommandBackLog> Resposne1 { get; set; }
            public List<tbl_Response> CommandResponse { get; set; }

        }

        public JsonResult LoadAllBilling(string pld, string mid)
        {
            try
            {
                Session.Add("mid", mid);
                DateTime CurrentDate = DateTime.UtcNow.Date;
                var query = clsMetersProd.sp_ResponseSplited(pld, CurrentDate, CurrentDate).OrderByDescending(x => x.ID).FirstOrDefault();
                var data = clsMetersProd.tbl_CommandBackLog.Where(x => x.pld == pld).OrderByDescending(x => x.ID).ToList();
                var response = clsMetersProd.tbl_Response.Where(x => x.Data.Contains("&") && x.pld == pld).OrderByDescending(c => c.ID).ToList();
                var balance = query.AccountBalance;
                var tariff = query.StandardCharge;
                var commonQuery = new JSONData { Resposne = query };
                var commonData = new JSONData { Resposne1 = data };
                var commonResponse = new JSONData { CommandResponse = response };
                var combinedList = new List<JSONData> { commonQuery, commonData, commonResponse };

                return Json(combinedList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }
        [HttpPost]
        public async Task<JsonResult> SendData(string aid, string pld, string eid, string eventname, string modetype, string balanceInput)
        {
            try
            {
                bool dataFound = false;
                float intValue = 0;
                if (modetype == "auto")
                {
                    string[] eventsplit = eventname.Split(',');
                    if (eventsplit.Length > 0)
                    {
                        foreach (string s in eventsplit)
                        {

                            var command1 = clsMeters.tbl_OTACommands.Where(x => x.Name == s).FirstOrDefault();
                            var data1 = command1.Command;
                            DateTime time1 = DateTime.UtcNow;
                            if (eventname == "Add Balance" || eventname == "Set Vat" || eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                            {
                                if (eventname.Contains("Set Vat"))
                                    intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
                                else
                                    intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
                                string balanceString = ToHexString(intValue);
                                //if (eventname == "Set Average Gas Calorific Value")
                                //{

                                //    balanceString = ConvertMsbToLsb(balanceString);
                                //}
                                var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
                                if (eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                                {
                                    // Reverse the array
                                    string[] splitArray = balanceutput.Split(',');

                                    // Reverse the array
                                    Array.Reverse(splitArray);

                                    // Join the array elements back into a string using a comma as the separator
                                    balanceutput = string.Join(",", splitArray);

                                }

                                string modifiedString = data1.Replace("-", balanceutput);
                                var length = balanceutput.Split(',').Length.ToString("D2");
                                data1 = modifiedString;
                                string[] values = data1.Split(',');
                                if (values.Length >= 3)
                                {
                                    // Replace the third value with the new variable
                                    values[3] = length;

                                    // Join the array back into a string with commas
                                    string resultString = string.Join(",", values);

                                    // Now, resultString will be "value1,value2,newvalue,value4,value5"

                                    // You can use the resultString in your view or further processing
                                    data1 = resultString;
                                }

                            }
                            var cData1 = clsMetersProd.tbl_CommandBackLog;
                            tbl_CommandBackLog clsCmd1 = new tbl_CommandBackLog();
                            clsCmd1.Data = data1;
                            clsCmd1.EventName = s;
                            clsCmd1.LogDate = DateTime.UtcNow;
                            clsCmd1.Status = "Pending";
                            clsCmd1.pld = pld;
                            clsMetersProd.tbl_CommandBackLog.Add(clsCmd1);
                            clsMetersProd.SaveChanges();
                        }

                    }
                    else
                    {
                        var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
                        var data = command.Command;
                        DateTime time = DateTime.UtcNow;
                        if (eventname == "Add Balance" || eventname == "Set Vat" || eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                        {
                            if (eventname.Contains("Set Vat"))
                                intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
                            else
                                intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
                            string balanceString = ToHexString(intValue);
                            //if (eventname == "Set Average Gas Calorific Value")
                            //{

                            //    balanceString = ConvertMsbToLsb(balanceString);
                            //}
                            var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
                            if (eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                            {
                                // Reverse the array
                                string[] splitArray = balanceutput.Split(',');

                                // Reverse the array
                                Array.Reverse(splitArray);

                                // Join the array elements back into a string using a comma as the separator
                                balanceutput = string.Join(",", splitArray);

                            }

                            string modifiedString = data.Replace("-", balanceutput);

                            data = modifiedString;

                        }
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
                    return Json("Command Added In Queue", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DateTime logdate = DateTime.UtcNow;
                    var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
                    var data = command.Command;
                    DateTime time = DateTime.UtcNow;
                    if (eventname == "Add Balance" || eventname == "Set Vat" || eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                    {
                        if (eventname.Contains("Set Vat"))
                            intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
                        else
                            intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
                        string balanceString = ToHexString(intValue);
                        //if (eventname == "Set Average Gas Calorific Value")
                        //{

                        //    balanceString = ConvertMsbToLsb(balanceString);
                        //}
                        var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
                        if (eventname == "Set Average Gas Calorific Value" || eventname == "Set E-Credit Threshold")
                        {
                            // Reverse the array
                            string[] splitArray = balanceutput.Split(',');

                            // Reverse the array
                            Array.Reverse(splitArray);

                            // Join the array elements back into a string using a comma as the separator
                            balanceutput = string.Join(",", splitArray);

                        }

                        string modifiedString = data.Replace("-", balanceutput);

                        data = modifiedString;

                    }
                    Random rnd1 = new Random();
                    double rndNumber1 = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
                    int size1 = rndNumber1.ToString().Length;
                    if (size1 != 12)
                    {
                        rndNumber1 = Convert.ToDouble(string.Format("{0}{1}", rndNumber1, 0));
                    }
                    List<string> substrings = new List<string>();

                    for (int i = 0; i < rndNumber1.ToString().Length; i += 2)
                    {
                        int length = Math.Min(2, rndNumber1.ToString().Length - i);
                        substrings.Add(rndNumber1.ToString().Substring(i, length));
                    }
                    var tid = string.Join(",", substrings);
                    data = "?" + data + ",##," + tid.ToString() + "!";
                    var bob1 = new
                    {
                        idType = "PLD",
                        id = pld,
                        transactionId = rndNumber1.ToString(),
                        retentionTime = DateTime.Now.ToString(),
                        data = new[] {
                     new  { aid =aid, dataformat = "cp",dataType="JSON",
                     ext="{'data': '"+data+"'}"
                     },

                }
                    };
                    int start = Convert.ToInt32(command.StartingPostion);
                    int end = Convert.ToInt32(command.EndingPostion);
                    var content1 = JsonConvert.SerializeObject(bob1);
                    var objClint2 = new System.Net.Http.HttpClient();
                    Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login"); //replace your Url  
                    var converter1 = new ExpandoObjectConverter();
                    c2dProd users1 = new c2dProd();
                    users1.grant_type = "password";
                    users1.username = "2025000_2890001@iot.jio.com";
                    users1.password = "a737b902951ec15cff735357a850b09cd941818095527a1925760b5a4e471464";
                    users1.client_id = "db2f04a5e72547cbb68331f406946494";
                    users1.client_secret = "d95578aa9b1eb30e";
                    string json1 = "";
                    json1 = Newtonsoft.Json.JsonConvert.SerializeObject(users1);
                    var objClint3 = new System.Net.Http.HttpClient();
                    System.Net.Http.HttpResponseMessage respon3 = await objClint3.PostAsync(requestUri1, new StringContent(json1, System.Text.Encoding.UTF8, "application/json"));
                    string responJsonText1 = await respon3.Content.ReadAsStringAsync();
                    var bearerToken1 = JsonConvert.DeserializeObject<c2dTokenProd>(responJsonText1);

                    Console.WriteLine(bearerToken1);
                    if (Convert.ToString(bearerToken1.access_token) != null)
                    {
                        Uri requestUri2 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified"); //replace your Url  
                        objClint2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        objClint2.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken1.access_token);
                        objClint2.DefaultRequestHeaders.Add("eid", eid);
                        System.Net.Http.HttpResponseMessage respon1 = await objClint2.PostAsync(requestUri2, new StringContent(content1, System.Text.Encoding.UTF8, "application/json"));
                        Thread.Sleep(18000);
                        var responJsonText2 = respon1.Content.ReadAsStringAsync();
                        var st = respon1.StatusCode;

                    }
                    //Thread.Sleep(22000);
                    var LogsData = "";
                    var fData = clsMetersProd.tbl_Response.Where(x => x.pld == pld && !x.Data.Contains("A1") && x.Data.Contains("&")).OrderByDescending(x => x.ID).FirstOrDefault();
                    if (fData.Data.Contains("&") && fData.Data.ToString().Split(',')[8].Contains(command.Code))
                    {
                        DateTime resDate = Convert.ToDateTime(fData.LogDate);
                        DateTime ckDate = logdate;
                        if (resDate >= ckDate)
                        {
                            LogsData = fData.Data;
                        }
                        string hexOutput = "";
                        if (LogsData != "")
                        {
                            string[] splitoutput = Convert.ToString(LogsData).Split(',');
                            for (int i = start + 4; i <= end + 4; i++)
                            {
                                hexOutput += splitoutput[i];
                            }
                        }
                        if (hexOutput != "")
                        {
                            return Json(hexOutput, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("error", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("error", JsonRequestBehavior.AllowGet);
                    }
                    //var response = await c2d.SendData(aid, pld, eid, eventname, balanceInput, modetype);
                    //if (response.ToString() != "error")
                    //    return Json(response, JsonRequestBehavior.AllowGet);
                    //return Json("error", JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        public string GetRes(string pld, string eventname, string balanceInput)
        {
            var resutlt = "";
            var data = "";
            float intValue = 0;
            var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            data = command.Command;
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
            int start = Convert.ToInt32(command.StartingPostion);
            int end = Convert.ToInt32(command.EndingPostion);
            var cData = clsMetersProd.tbl_CommandBackLog;
            tbl_CommandBackLog clsCmd = new tbl_CommandBackLog();
            clsCmd.Data = data;
            clsCmd.EventName = eventname;
            clsCmd.LogDate = DateTime.UtcNow;
            clsCmd.Status = "Pending";
            clsCmd.pld = pld;
            clsMetersProd.tbl_CommandBackLog.Add(clsCmd);
            clsMetersProd.SaveChanges();
            var LogsData = "";
            if (eventname != "Add Balance" || eventname != "Set Vat")
            {
                var res = clsMetersProd.sp_MeterResponse().Where(x => x.pld == pld && x.LogDate == DateTime.UtcNow.Date).OrderByDescending(x => x.ID).ToList();
                foreach (var fData in res)
                {
                    if (fData.Data.Contains(command.Code))
                    {
                        LogsData = fData.Data;
                        break;
                    }
                }
            }
            string hexOutput = "";
            if (LogsData != "")
            {
                string[] splitoutput = Convert.ToString(LogsData).Split(',');
                for (int i = start + 3; i <= end + 3; i++)
                {
                    hexOutput += splitoutput[i];
                }
                var Smeter = clsMetersProd.tbl_SMeterMaster.Where(x => x.PLD == pld).FirstOrDefault();
            }
            else
            {
                hexOutput = "Command Added In Queue";
            }
            return hexOutput;
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
        public string ConvertMsbToLsb(string msbValue)
        {
            char[] charArray = msbValue.ToCharArray();
            Array.Reverse(charArray);
            string lsbValue = new string(charArray);
            return lsbValue;
        }
        public async Task<JsonResult> SendDataAddTariff(string aid, string pld, string eid, string eventname, string date, string time, string Tariff)
        {
            bool dataFound = false;
            float intValue = 0;
            var lstdata = "?02,03,00,00,A1,03,##,23,07,27,10,38,00!";
            var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            var data = command.Command;
            DateTime time2 = DateTime.UtcNow;
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
            intValue = float.Parse(Tariff, CultureInfo.InvariantCulture.NumberFormat);
            string balanceString = ToHexString(intValue);
            var dateput = string.Join(",", SplitIntoChunks(hex, 2));
            var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
            string modifiedString = data.Replace("-", dateput + "," + balanceutput);
            data = modifiedString;
            var length = ((dateput.Split(',').Length + Convert.ToInt16(balanceutput.Split(',').Length.ToString("D2"))) + 1).ToString("X");
            string[] values = data.Split(',');
            if (values.Length >= 3)
            {
                // Replace the third value with the new variable
                values[3] = "0" + length;

                // Join the array back into a string with commas
                string resultString = string.Join(",", values);

                // Now, resultString will be "value1,value2,newvalue,value4,value5"

                // You can use the resultString in your view or further processing
                data = resultString;
            }
            var cData = clsMetersProd.tbl_CommandBackLog;
            tbl_CommandBackLog clsCmd = new tbl_CommandBackLog();
            clsCmd.Data = data;
            clsCmd.EventName = eventname;
            clsCmd.LogDate = DateTime.UtcNow;
            clsCmd.Status = "Pending";
            clsCmd.pld = pld;
            clsMetersProd.tbl_CommandBackLog.Add(clsCmd);
            clsMetersProd.SaveChanges();
            return Json("Command Added In Queue", JsonRequestBehavior.AllowGet);


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
        public JsonResult GetCal(string data)
        {
            var balanceutput = string.Join(",", SplitIntoChunks(data, 2));

            // Reverse the array
            string[] splitArray = balanceutput.Split(',');

            // Reverse the array
            Array.Reverse(splitArray);

            // Join the array elements back into a string using a comma as the separator
            balanceutput = string.Join(",", splitArray);

            data = balanceutput.Replace(",", "");
            float creditE = FromHexString(data);


            return Json(creditE.ToString("F1"), JsonRequestBehavior.AllowGet);

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
        public string HexToString(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hexadecimal string length.");
            }

            // Create a byte array to hold the hexadecimal values
            byte[] bytes = new byte[hex.Length / 2];

            // Iterate through the hexadecimal string and convert each pair of characters to a byte
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            // Convert the byte array to a string using ASCII encoding
            string result = System.Text.Encoding.ASCII.GetString(bytes);
            return result;
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
                    op += HexToString(sdci);
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

            var res = clsMetersProd.sp_MeterResponse().Where(x => x.pld == pld && !x.Data.Contains("A1") && x.Data.Contains("&")).OrderByDescending(x => x.ID).ToList();
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

            foreach (var fData in res)
            {
                try
                {
                    if (fData.Data.Contains(code))
                    {

                        LogsData = fData.Data;
                        break;

                    }
                }
                catch (Exception ex) { }
            }
            if (LogsData != "")
            {
                string[] splitoutput = Convert.ToString(LogsData).Split(',');
                for (int i = startPostion + 4; i <= endPostion + 4; i++)
                {
                    hexOutput += splitoutput[i];
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
                var responJsonText2 = respon1.StatusCode;
                MyHub.SendMessages();
                return Json(responJsonText1.Result.ToString(), JsonRequestBehavior.AllowGet);

            }
            return Json(responJsonText, JsonRequestBehavior.AllowGet);
        }
        public string ReverseString(string input)
        {
            // Convert the string to a character array
            char[] charArray = input.ToCharArray();

            // Reverse the character array
            Array.Reverse(charArray);

            // Convert the reversed character array back to a string
            string reversedString = new string(charArray);

            return reversedString;
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
        public ActionResult BillingParameters(string mid)
        {
            if (mid != null)
            {
                ViewBag.mid = mid;
            }
            var data = (from log in clsMetersProd.tbl_SMeterMaster
                        select new
                        {
                            ID = log.ID,
                            TempMeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                            AID = log.AID,
                            PLD = log.PLD,
                        }).ToList();
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

        public ActionResult CommandMonitoring()
        {
            return View();
        }
        public ActionResult CommandMonitoringData()
        {
            if (Convert.ToString(Session["RoleID"]) == "9")
            {
                var data = (from backLog in clsMetersProd.tbl_CommandBackLog
                            join log in clsMetersProd.tbl_SMeterMaster on backLog.pld equals log.PLD
                            orderby backLog.ID descending
                            select new
                            {
                                ID = log.ID,
                                MeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                                Data = backLog.Data,
                                EventName = backLog.EventName,
                                Status = backLog.Status,
                                LogDate = backLog.LogDate,
                                CompletedLogDate = backLog.CompletedLogDate,
                            });
                return Json(data.Where(x=>x.MeterID== "100111").ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = (from backLog in clsMetersProd.tbl_CommandBackLog
                            join log in clsMetersProd.tbl_SMeterMaster on backLog.pld equals log.PLD
                            orderby backLog.ID descending
                            select new
                            {
                                ID = log.ID,
                                MeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                                Data = backLog.Data,
                                EventName = backLog.EventName,
                                Status = backLog.Status,
                                LogDate = backLog.LogDate,
                                CompletedLogDate = backLog.CompletedLogDate,
                            }).ToList();
                return Json(data.ToList(), JsonRequestBehavior.AllowGet);
            }

            
        }


    }

    public class Response
    {
        public string Result { get; set; }
    }

}