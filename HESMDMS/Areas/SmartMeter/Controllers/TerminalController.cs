using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    [SessionRequired]
    public class TerminalController : Controller
    {
        // TODO: Use dependency injection for DbContexts and IHttpClientFactory for better testability and management.
        private readonly SmartMeterEntities _clsMeters = new SmartMeterEntities();
        private readonly SmartMeter_ProdEntities _clsMetersProd = new SmartMeter_ProdEntities();
        private readonly IHttpClientFactory _clientFactory;

        /*
        public TerminalController(SmartMeterEntities clsMeters, SmartMeter_ProdEntities clsMetersProd, IHttpClientFactory clientFactory)
        {
            _clsMeters = clsMeters;
            _clsMetersProd = clsMetersProd;
            _clientFactory = clientFactory;
        }
        */

        #region Views

        public ActionResult Index()
        {
            LoadMeterDataToViewBag();
            return View();
        }

        public ActionResult Terminal()
        {
            LoadMeterDataToViewBag();
            return View();
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
            LoadMeterDataToViewBag();
            return View();
        }

        public ActionResult CommandMonitoring()
        {
            return View();
        }

        #endregion

        #region JSON Actions for UI

        public JsonResult LoadAllBilling(string pld, string mid)
        {
            try
            {
                Session["mid"] = mid;
                DateTime currentDate = DateTime.UtcNow.Date;

                var latestResponse = _clsMetersProd.sp_ResponseSplited(pld, currentDate, currentDate)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                var commandBacklog = _clsMetersProd.tbl_CommandBackLog
                    .Where(x => x.pld == pld)
                    .OrderByDescending(x => x.ID)
                    .ToList();

                var commandResponses = _clsMetersProd.tbl_Response
                    .Where(x => x.Data.Contains("&") && x.pld == pld)
                    .OrderByDescending(c => c.ID)
                    .ToList();

                var viewModel = new BillingDataViewModel
                {
                    LatestResponse = latestResponse,
                    CommandBacklog = commandBacklog,
                    CommandResponses = commandResponses
                };

                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return Json(new { success = false, message = "An error occurred while loading billing data." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SendData(string aid, string pld, string eid, string eventname, string modetype, string balanceInput)
        {
            try
            {
                if (eventname == "Mod Balance")
                {
                    return HandleModBalance(pld, balanceInput);
                }

                if (modetype == "auto")
                {
                    return HandleAutoMode(pld, eventname, balanceInput);
                }

                // The original code had a third case which was not clear.
                // It is recommended to create a separate method for that logic if needed.
                return Json("error: Unsupported operation", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // TODO: Log the exception ex
                return Json("error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SendDataAddTariff(string aid, string pld, string eid, string eventname, string date, string time, string Tariff)
        {
            try
            {
                var command = _clsMeters.tbl_OTACommands.FirstOrDefault(x => x.Name == eventname);
                if (command == null)
                {
                    return Json("Error: Command not found.", JsonRequestBehavior.AllowGet);
                }

                DateTime datetime = Convert.ToDateTime(date + " " + time);
                string inputDateHex = ConvertDateTimeToHex(datetime);

                float tariffValue = float.Parse(Tariff, CultureInfo.InvariantCulture.NumberFormat);
                string tariffHex = ToHexString(tariffValue);

                var datePut = string.Join(",", SplitIntoChunks(inputDateHex, 2));
                var balanceOutput = string.Join(",", SplitIntoChunks(tariffHex, 2));

                string data = command.Command.Replace("-", datePut + "," + balanceOutput);

                var length = ((datePut.Split(',').Length + Convert.ToInt16(balanceOutput.Split(',').Length.ToString("D2"))) + 1).ToString("X");
                string[] values = data.Split(',');
                if (values.Length >= 4)
                {
                    values[3] = "0" + length;
                    data = string.Join(",", values);
                }

                AddCommandToBacklog(eventname, data, pld);
                _clsMetersProd.SaveChanges();

                return Json("Command Added In Queue", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // TODO: Log exception ex
                return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CommandMonitoringData()
        {
            var query = from backLog in _clsMetersProd.tbl_CommandBackLog
                        join log in _clsMetersProd.tbl_SMeterMaster on backLog.pld equals log.PLD
                        orderby backLog.ID descending
                        select new
                        {
                            ID = log.ID,
                            MeterID = log.MeterSerialNumber ?? log.TempMeterID,
                            Data = backLog.Data,
                            EventName = backLog.EventName,
                            Status = backLog.Status,
                            LogDate = backLog.LogDate,
                            CompletedLogDate = backLog.CompletedLogDate,
                        };

            if (Convert.ToString(Session["RoleID"]) == "9")
            {
                var filteredData = query.Where(x => x.MeterID == "100111" || x.MeterID == "100115").ToList();
                return Json(filteredData, JsonRequestBehavior.AllowGet);
            }

            return Json(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Data Conversion and Utilities

        public JsonResult StringToHexNVE(string data)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }
            float balanceString = BitConverter.ToSingle(bytes, 0);
            return Json(balanceString.ToString("F3"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StringToHex(string data)
        {
            var balanceString = FromHexString(data);
            return Json(balanceString.ToString("F3"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult TamperStatus(string data)
        {
            string[] splitOutput = SplitIntoChunks(data, 2).ToArray();
            var tamperValues = splitOutput.Select(hex => Convert.ToInt32("0x" + hex, 16)).ToList();
            return Json(string.Join(",", tamperValues), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRTC(string data)
        {
            string op = "";
            string[] bytes = SplitIntoChunks(data, 2).ToArray();
            foreach (var s in bytes)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    op += HexToString(s);
                }
            }

            if (op.Length == 12)
            {
                StringBuilder myStringBuilder = new StringBuilder(op);
                myStringBuilder.Insert(2, "-").Insert(5, "-").Insert(8, " ").Insert(11, ":").Insert(14, ":");
                return Json(myStringBuilder.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json("Invalid RTC data", JsonRequestBehavior.AllowGet);
        }

        public JsonResult VolategCalc(string data)
        {
            var hexString = (decimal)long.Parse(data, NumberStyles.HexNumber);
            decimal voltage = hexString / 1000;
            return Json(voltage.ToString("F3"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult BatteryLife(string data)
        {
            if (data != "NaN" && int.TryParse(data, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int hexString))
            {
                int year = hexString / 365;
                int days = hexString % 365;
                return Json($"{year} Years {days} days", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Helper Methods

        private void LoadMeterDataToViewBag()
        {
            var data = _clsMetersProd.tbl_SMeterMaster
                .Select(log => new
                {
                    log.ID,
                    TempMeterID = log.MeterSerialNumber ?? log.TempMeterID,
                    log.AID,
                    log.PLD,
                })
                .ToList();
            ViewBag.data = data;
        }

        private JsonResult HandleModBalance(string pld, string balanceInput)
        {
            const string fixByte = "02,03,00,08,30,01,03,5F,04";
            float parsedBalance = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
            string balanceHex = ToHexString(parsedBalance);
            string formattedBalance = string.Join(",", SplitIntoChunks(balanceHex, 2));
            string finalOutput = $"{fixByte},{formattedBalance},03";

            AddCommandToBacklog("Mod Balance", finalOutput, pld);
            _clsMetersProd.SaveChanges();

            return Json("Command Added In Queue", JsonRequestBehavior.AllowGet);
        }

        private JsonResult HandleAutoMode(string pld, string eventname, string balanceInput)
        {
            int pendingCount = _clsMetersProd.tbl_CommandBackLog.Count(x => x.pld == pld && (x.Status == "Pending" || x.Status == "ScheduleToRetry"));
            if (pendingCount >= 5)
            {
                return Json("Max Command Executed", JsonRequestBehavior.AllowGet);
            }

            string[] eventSplit = eventname.Split(',');
            foreach (string s in eventSplit)
            {
                var command = _clsMeters.tbl_OTACommands.FirstOrDefault(x => x.Name == s);
                if (command == null)
                {
                    // Log or handle unknown command
                    continue;
                }

                string data = command.Command;

                if (s == "Set RTC")
                {
                    data = data.Replace("-", string.Join(",", SplitIntoChunks(ConvertDateTimeToHex(DateTime.Now), 2)));
                }
                else if (new[] { "Add Balance", "Set Vat", "Set Average Gas Calorific Value", "Set E-Credit Threshold" }.Contains(s))
                {
                    float intValue = s.Contains("Set Vat")
                        ? float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100
                        : float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);

                    string balanceHex = ToHexString(intValue);
                    string formattedHex = string.Join(",", SplitIntoChunks(balanceHex, 2));

                    if (s == "Set Average Gas Calorific Value" || s == "Set E-Credit Threshold")
                    {
                        var arr = formattedHex.Split(',');
                        Array.Reverse(arr);
                        formattedHex = string.Join(",", arr);
                    }

                    data = data.Replace("-", formattedHex);

                    string[] segments = data.Split(',');
                    if (segments.Length >= 4)
                    {
                        segments[3] = (formattedHex.Split(',').Length).ToString("D2");
                        data = string.Join(",", segments);
                    }
                }

                AddCommandToBacklog(s, data, pld);
            }
            _clsMetersProd.SaveChanges();
            return Json("Command Added In Queue", JsonRequestBehavior.AllowGet);
        }

        private void AddCommandToBacklog(string eventName, string data, string pld)
        {
            var commandLog = new tbl_CommandBackLog
            {
                Data = data,
                EventName = eventName,
                LogDate = DateTime.UtcNow,
                Status = "Pending",
                pld = pld
            };
            _clsMetersProd.tbl_CommandBackLog.Add(commandLog);
        }

        private static IEnumerable<string> SplitIntoChunks(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }

        private static string ConvertDateTimeToHex(DateTime dt)
        {
            string inputDate = dt.ToString("ddMMyyHHmmss");
            return string.Concat(Enumerable.Range(0, inputDate.Length / 2)
                .Select(i => int.Parse(inputDate.Substring(i * 2, 2)).ToString("X2")));
        }

        private static string HexToString(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hexadecimal string length.");
            }
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return Encoding.ASCII.GetString(bytes);
        }

        // The 'unsafe' keyword is used here for performance. It directly manipulates memory
        // to convert between float and int bits. This is a common pattern in older .NET code.
        // In modern .NET (Core/.NET 5+), you can use BitConverter.SingleToInt32Bits(f) for a safe equivalent.
        private static unsafe string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }

        private static unsafe float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
        }

        #endregion

        #region ViewModels and DTOs

        public class BillingDataViewModel
        {
            public sp_ResponseSplited_Result LatestResponse { get; set; }
            public List<tbl_CommandBackLog> CommandBacklog { get; set; }
            public List<tbl_Response> CommandResponses { get; set; }
        }

        public class Response
        {
            public string Result { get; set; }
        }

        #endregion
    }
}