using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.Entity;
using OfficeOpenXml;
using System.IO;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class MeterMasterController : Controller
    {
        SmartMeter_ProdEntities clsProd = new SmartMeter_ProdEntities();
        // GET: SmartMeter/MeterMaster
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMeterMasterData(DataSourceLoadOptions loadOptions)
        {
            var data = clsProd.tbl_SMeterMaster.ToList().OrderByDescending(x=>x.ID);
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(data, loadOptions)), "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> InsertMeterMaster(string values)
        {
            var newMeter = new tbl_SMeterMaster();
            JsonConvert.PopulateObject(values, newMeter);

            if (!TryValidateModel(newMeter))
                return new HttpStatusCodeResult(400, "Validation failed");

            clsProd.tbl_SMeterMaster.Add(newMeter);
            await clsProd.SaveChangesAsync();

            return new EmptyResult();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMeterMaster(int key, string values)
        {
            var meter = await clsProd.tbl_SMeterMaster.FirstOrDefaultAsync(m => m.ID == key);
            if (meter == null)
                return new HttpStatusCodeResult(404, "Meter not found");

            JsonConvert.PopulateObject(values, meter);

            if (!TryValidateModel(meter))
                return new HttpStatusCodeResult(400, "Validation failed");

            await clsProd.SaveChangesAsync();
            return new EmptyResult();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMeterMaster(int key)
        {
            var meter = await clsProd.tbl_SMeterMaster.FirstOrDefaultAsync(m => m.ID == key);
            if (meter == null)
                return new HttpStatusCodeResult(404, "Meter not found");

            clsProd.tbl_SMeterMaster.Remove(meter);
            await clsProd.SaveChangesAsync();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var pld = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var meterSerial = worksheet.Cells[row, 2].Value?.ToString().Trim();

                            if (!string.IsNullOrEmpty(pld) && !string.IsNullOrEmpty(meterSerial))
                            {
                                var existingMeter = clsProd.tbl_SMeterMaster.FirstOrDefault(m => m.PLD == pld || m.MUNumber == meterSerial);
                                if (existingMeter == null)
                                {
                                    var newMeter = new tbl_SMeterMaster
                                    {
                                        PLD = pld,
                                        MUNumber = meterSerial,
                                        SimICCID = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                                        SimNo = Convert.ToDouble(worksheet.Cells[row, 4].Value),
                                        IMSI = Convert.ToDouble(worksheet.Cells[row, 5].Value),
                                        IMEI = Convert.ToDouble(worksheet.Cells[row, 6].Value),
                                        DeviceName = worksheet.Cells[row, 7].Value?.ToString().Trim(),
                                        AID = worksheet.Cells[row, 8].Value?.ToString().Trim(),
                                       
                                    };
                                    clsProd.tbl_SMeterMaster.Add(newMeter);
                                }
                            }
                        }
                        clsProd.SaveChanges();
                        return Json(new { success = true, message = "Data imported successfully." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error importing data: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "No file uploaded." });
        }
    }
}