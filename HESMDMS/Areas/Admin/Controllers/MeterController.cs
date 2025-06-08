using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class MeterController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        // GET: Admin/Meter
        [Authorize]
        [SessionRequired]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReplaceMeter()
        {
            var amrRegistration = clsMeters.tbl_CustomerRegistration.ToList();
            ViewBag.AMRRegistration = JsonConvert.SerializeObject(amrRegistration);
            return View();
        }
        public ActionResult AMRBilling()
        {
            return View();
        }
        public class AMRData
        {
            public string Installation { get; set; }
            public string Material { get; set; }
            public string Serialno { get; set; }
            public string Register { get; set; }
            public string Mrreason { get; set; }
            public string Mridnumber { get; set; }
            public string Readingresult { get; set; }
            public string Meterreadingnote { get; set; }
            public string Actualcustomermrtype { get; set; }
            public string Meterreader { get; set; }
            public string Mrdateforbilling { get; set; }
            public string Mrtimeforbilling { get; set; }
            public string Actualmrdate { get; set; }
            public string Actualmrtime { get; set; }
            public string Mrdateofmaximum { get; set; }
            public string Mrtimeofmaximum { get; set; }
            public string Smorder { get; set; }
            public string Active { get; set; }
            public string Refnumber { get; set; }
            public string Targetmrdate { get; set; }
            public string ExtUi { get; set; }
            public string Mrdateforidentif { get; set; }
        }
        public class AMRDataRequest
        {
            public List<AMRData> AMRData { get; set; }
        }
        public async Task<ActionResult> GenerateBillAsyncAMRAsync(string bpnumber, string Date, string ReadingCount)
        {
            var amrDataRequest = new AMRDataRequest
            {
                AMRData = new List<AMRData>()
            };

            using (FileStream fs = new FileStream("D:\\KhurjaData050220241.xlsx", FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs); // For Excel 2007 or newer

                ISheet sheet = workbook.GetSheetAt(0); // Assuming the first sheet is of interest
                if (sheet != null)
                {
                    int rowCount = sheet.LastRowNum + 1;

                    // Extract data from the worksheet
                    var data = new List<List<string>>();
                    for (int i = 0; i < rowCount; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            var rowData = new List<string>();
                            for (int j = 0; j < row.LastCellNum; j++)
                            {
                                rowData.Add(row.GetCell(j)?.ToString() ?? "");
                            }
                            data.Add(rowData);
                            string bp = rowData[0].ToString();

                            var amrRecord = new AMRData
                            {
                                Installation = "1000341244",
                                Material = "",
                                Serialno = "17335704",
                                Register = "001",
                                Mrreason = "01",
                                Mridnumber = "",
                                Readingresult = "1467.644",
                                Meterreadingnote = "",
                                Actualcustomermrtype = "01",
                                Meterreader = "",
                                Mrdateforbilling = "20250430",
                                Mrtimeforbilling = "20250430",
                                Actualmrdate = "20250430",
                                Actualmrtime = "0600",
                                Mrdateofmaximum = "",
                                Mrtimeofmaximum = "",
                                Smorder = "",
                                Active = "",
                                Refnumber = "",
                                Targetmrdate = "",
                                ExtUi = "",
                                Mrdateforidentif = ""
                            };
                            amrDataRequest.AMRData.Add(amrRecord);
                            // Add other AMRData items similarly
                        }


                    }
                    string json = JsonConvert.SerializeObject(amrDataRequest);
                    string Url = "https://dbcixdp.adani.com:443/RESTAdapter/hesmdms/AMRData";
                    string Username = "INTF_SMARTMETER";
                    string Password = "SmA&T25m&@";
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}")));
                        var json1 = JsonConvert.SerializeObject(json);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                        var response = await client.PostAsync(Url, content);
                        if (response.IsSuccessStatusCode)
                        {
                            //Thread.Sleep(1000);
                            // Handle success1

                        }

                    }

                    // Process the data as needed
                    return Json(new { success = true, data = data });
                }
            }



            return Json(bpnumber, JsonRequestBehavior.AllowGet);


        }
    }
}