using HESMDMS.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Owin;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        // GET: SmartMeter/User
        public ActionResult Index()
        {
            try
            {
                int userID = Convert.ToInt32(Session["UserID"]);
                if (userID == 0)
                {
                    return Redirect("../../Login");
                }
                else
                {
                    //TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    //DateTimeOffset Currdate = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, ist);
                    DateTime Currdate = DateTime.UtcNow.Date;
                    DateTime Prevdate = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0)).Date;
                    DateTime MonthDate = DateTime.UtcNow.Date.AddDays(-30);
                    ViewBag.cur = Prevdate;
                    var UserDetails = clsMetersProd.tbl_SmartMeterUser.Where(x => x.UserID == userID).FirstOrDefault();
                    var MeterID = UserDetails.MeterID;
                    var pldQuery = clsMetersProd.tbl_SMeterMaster.Where(x => x.TempMeterID == MeterID).FirstOrDefault();
                    var pld = pldQuery.PLD;
                    var AuditLog = clsMetersProd.tbl_CommandBackLog.Where(x => x.pld == pld && x.EventName == "Add Balance").OrderByDescending(x => x.ID).Take(5).ToList();
                    ViewBag.AuditLog = AuditLog;
                    Session.Add("pld", pld);
                    ViewBag.pld = pld;
                    ViewBag.aid = pldQuery.AID;
                    ViewBag.eid = pldQuery.EID;
                    var DataQueryToday = clsMetersProd.sp_ResponseSplited(pld, Currdate, Currdate).FirstOrDefault();
                    var DataQueryYesterday = clsMetersProd.sp_ResponseSplited(pld, Prevdate, Prevdate).FirstOrDefault();
                    Session.Add("Meter ID", MeterID);
                    var AccountBalance = DataQueryToday != null ? DataQueryToday.AccountBalance.ToString().TrimStart('+').TrimStart('0') : "000";
                    var YAccountBalance = DataQueryYesterday != null ? DataQueryYesterday.AccountBalance.ToString().TrimStart('+').TrimStart('0') : "00";
                    var Volume = DataQueryToday != null ? DataQueryToday.ActualConsumption : 000;
                    var YVolume = DataQueryYesterday != null ? DataQueryYesterday.ActualConsumption : 000;
                    var Time = DataQueryToday != null ? DataQueryToday.Time : "00";
                    double vol = Convert.ToDouble(Volume) * 0.035315;
                    double Yvol = Convert.ToDouble(YVolume) * 0.035315;
                    ViewBag.Balance = AccountBalance;
                    ViewBag.YBalance = YAccountBalance;
                    ViewBag.BalanceDate = DataQueryToday != null ? DataQueryToday.Date.ToString().Replace("12:00:00 AM", "") + " " + Time : "00";
                    ViewBag.YBalanceDate = DataQueryYesterday != null ? DataQueryYesterday.Date.ToString().Replace("12:00:00 AM", "") + " " + DataQueryYesterday.Time : "00";
                    ViewBag.VolumeMMBTU = vol.ToString("F3");
                    ViewBag.YVolumeMMBTU = Yvol.ToString("F3");
                    return View();

                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                // You might want to return a specific "Error" view here
                return View("Error");
            }

        }
        public ActionResult PaymentSuccess()
        {
            return View();
        }
        public JsonResult GetOderID()
        {
            return Json(Session["orderId"].ToString(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);
            try
            {
                Utils.verifyPaymentSignature(attributes);
                ViewBag.pld = Session["pld"].ToString();
                ViewBag.aid = Session["aid"].ToString();
                ViewBag.eid = Session["eeid"].ToString();
                ViewBag.amount = Session["amount"].ToString();
                return View("PaymentSuccess");
            }
            catch (Exception ex)
            {
                return View("PaymentFailure");
            }
        }
        public ActionResult InitiatePayment(string amount, string pld, string aid, string eeid)
        {
            var key = ConfigurationManager.AppSettings["RazorPaykey"].ToString();
            var secret = ConfigurationManager.AppSettings["RazorPaySecret"].ToString();
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(amount)*100);
            options.Add("currency", "INR");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Order order = client.Order.Create(options);
            ViewBag.orderId = order["id"].ToString();
            Session.Add("orderId", order["id"].ToString());
            Session.Add("pld", pld);
            Session.Add("aid", aid);
            Session.Add("amount", amount);
            Session.Add("eeid", eeid);
            return View("Payment");
        }


        public ActionResult GetChartData()
        {
            DateTime Currdate = DateTime.UtcNow.Date;
            DateTime MonthDate = DateTime.UtcNow.Date.AddDays(-30);
            string pld = Convert.ToString(Session["pld"]);
            var DataQuery30Days = clsMetersProd.sp_ResponseSplited(pld, MonthDate.AddHours(+5).AddMinutes(+30), Currdate).AsEnumerable()
    .ToList();

            var latestTransactionsPerDay = DataQuery30Days
      .GroupBy(t => t.Date)
      .Select(g => g.OrderByDescending(t => t.ID).First())
      .ToList();

            decimal[] amounts1 = latestTransactionsPerDay.OrderByDescending(r => r.ID)
                .Select(t => t.ActualConsumption)
                .ToArray();

            decimal[] amounts = amounts1.Select(val => val * (decimal)0.035315).ToArray();

            string[] categories = latestTransactionsPerDay.OrderByDescending(r => r.ID)
                .Select(t => t.Date.ToString().Replace("12:00:00 AM", "")) // Assume 'Category' is a property of 'Transaction'
                .ToArray();

            return Json(new { amounts, categories }, JsonRequestBehavior.AllowGet);
        }
        unsafe float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
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
            var cData = clsMetersProd.tbl_CommandBackLog;
            tbl_CommandBackLog clsCmd = new tbl_CommandBackLog();
            clsCmd.Data = data;
            clsCmd.EventName = eventname;
            clsCmd.LogDate = DateTime.UtcNow;
            clsCmd.Status = "Pending";
            clsCmd.pld = pld;
            clsMetersProd.tbl_CommandBackLog.Add(clsCmd);
            clsMetersProd.SaveChanges();


            return hexOutput;
        }
        public static IEnumerable<string> SplitIntoChunks(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }
        unsafe string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }
        public ActionResult Error()
        {
            return View();
        }

        //[SessionRequired]
        //public ActionResult AddUser()
        //{
        //    return View();
        //}
    }
}