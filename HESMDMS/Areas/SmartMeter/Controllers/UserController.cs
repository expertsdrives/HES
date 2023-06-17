using HESMDMS.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
                    var MeterID = Convert.ToDouble(UserDetails.MeterID);
                    var pldQuery = clsMeters.tbl_SMeterMaster.Where(x => x.MeterID == MeterID).FirstOrDefault();
                    var pld = pldQuery.PLD;
                    var AuditLog = clsMetersProd.tbl_SGMAuditLogs.Where(x => x.pld == pld && x.EventName == "Add Balance").OrderByDescending(x => x.ID).Take(5).ToList();
                    ViewBag.AuditLog = AuditLog;
                    Session.Add("pld", pld);
                    ViewBag.pld = pld;
                    ViewBag.aid = pldQuery.AID;
                    ViewBag.eid = pldQuery.EID;
                    var DataQueryToday = clsMetersProd.sp_ResponseSplited(pld, Currdate, Currdate).FirstOrDefault();
                    var DataQueryYesterday = clsMetersProd.sp_ResponseSplited(pld, Prevdate, Prevdate).FirstOrDefault();
                    Session.Add("Meter ID", MeterID);
                    var AccountBalance = DataQueryToday.AccountBalance.ToString().TrimStart('+').TrimStart('0');
                    var YAccountBalance = DataQueryYesterday.AccountBalance.ToString().TrimStart('+').TrimStart('0');
                    var Volume = DataQueryToday.ActualConsumption;
                    var YVolume = DataQueryYesterday.ActualConsumption;
                    var Time = DataQueryToday.Time;
                    double vol = Convert.ToDouble(Volume) * 0.035315;
                    double Yvol = Convert.ToDouble(YVolume) * 0.035315;
                    ViewBag.Balance = AccountBalance;
                    ViewBag.YBalance = YAccountBalance;
                    ViewBag.BalanceDate = DataQueryToday.Date.ToString().Replace("12:00:00 AM", "") + " " + Time;
                    ViewBag.YBalanceDate = DataQueryYesterday.Date.ToString().Replace("12:00:00 AM", "") + " " + DataQueryYesterday.Time;
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
                return Json("error", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json("Manual Mode Not Activated", JsonRequestBehavior.AllowGet);
                //}
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
        public ActionResult AddUser()
        {
            return View();
        }
    }
}