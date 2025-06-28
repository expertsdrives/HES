using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class FotaViewModel
    {
        public string SelectedMeterId { get; set; }
    }

    public enum FotaActionType
    {
        Initiate,
        Continue,
        Abort
    }

    public class FotaController : Controller
    {
        //TODO: Using two different DbContexts is unusual. This should be reviewed.
        //For now, we will assume this is intentional.
        private readonly SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        private readonly SmartMeter_ProdEntities1 clsMetersProd1 = new SmartMeter_ProdEntities1();

        // GET: Admin/Fota
        public ActionResult Index(string meterId)
        {
            var model = new FotaViewModel
            {
                SelectedMeterId = meterId
            };
            return View(model);
        }

        [HttpGet]
        public JsonResult GetMeterDetails(string meterId)
        {
            if (string.IsNullOrWhiteSpace(meterId))
            {
                return Json(new { success = false, message = "Meter ID cannot be empty." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var meterDetails = (from log in clsMetersProd.tbl_SMeterMaster
                                    where log.MeterSerialNumber == meterId || log.TempMeterID == meterId
                                    select new
                                    {
                                        log.AID,
                                        log.PLD
                                    }).FirstOrDefault();

                if (meterDetails != null)
                {
                    return Json(new { success = true, data = meterDetails }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Meter not found." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // It's crucial to log the exception details (ex.ToString()) for debugging.
                // Elmah or other logging frameworks would be ideal here.
                return Json(new { success = false, message = "An error occurred while fetching meter details." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> uploadedFiles, string selectedMeter)
        {
            if (string.IsNullOrWhiteSpace(selectedMeter))
            {
                TempData["ErrorMessage"] = "No meter selected.";
                return RedirectToAction("Index");
            }

            var meter = clsMetersProd.tbl_SMeterMaster.FirstOrDefault(x => x.MeterSerialNumber == selectedMeter || x.TempMeterID == selectedMeter);
            if (meter == null)
            {
                TempData["ErrorMessage"] = $"Meter with ID {selectedMeter} not found.";
                return RedirectToAction("Index");
            }

            var validFiles = uploadedFiles?.Where(f => f != null && f.ContentLength > 0).ToList();
            if (validFiles == null || !validFiles.Any())
            {
                TempData["ErrorMessage"] = "No valid files were uploaded.";
                return RedirectToAction("Index", new { meterId = selectedMeter });
            }

            try
            {
                ProcessUploadedFiles(validFiles, selectedMeter);
                TempData["Message"] = $"{validFiles.Count} file(s) uploaded successfully for meter {selectedMeter}.";
            }
            catch (Exception ex)
            {
                // Log the full exception (ex.ToString()) for diagnostics.
                TempData["ErrorMessage"] = "An error occurred during file upload. Please check the logs for details.";
            }

            return RedirectToAction("Index", new { meterId = selectedMeter });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WriteCommand(string selectedMeter, FotaActionType fotaAction)
        {
            if (string.IsNullOrWhiteSpace(selectedMeter))
            {
                TempData["ErrorMessage"] = "No meter selected.";
                return RedirectToAction("Index");
            }

            var meter = clsMetersProd.tbl_SMeterMaster.FirstOrDefault(x => x.MeterSerialNumber == selectedMeter || x.TempMeterID == selectedMeter);
            if (meter == null)
            {
                TempData["ErrorMessage"] = $"Meter with ID {selectedMeter} not found.";
                return RedirectToAction("Index");
            }

            try
            {
                string command = GenerateFotaCommand(fotaAction, out string actionMessage);

                var cmdBacklog = new tbl_CommandBackLog
                {
                    Data = command,
                    EventName = "Fota",
                    Status = "Pending",
                    LogDate = DateTime.Now,
                    pld = meter.PLD
                };
                clsMetersProd.tbl_CommandBackLog.Add(cmdBacklog);
                clsMetersProd.SaveChanges();

                TempData["Message"] = actionMessage;
            }
            catch (Exception ex)
            {
                // Log the full exception (ex.ToString()) for diagnostics.
                TempData["ErrorMessage"] = "An error occurred during the FOTA command generation. Please check the logs for details.";
            }

            return RedirectToAction("Index", new { meterId = selectedMeter });
        }

        private void ProcessUploadedFiles(List<HttpPostedFileBase> files, string meterId)
        {
            string folderPath = Server.MapPath("~/UploadedFiles/");
            Directory.CreateDirectory(folderPath); // Ensures the directory exists.

            string meterIdStr = meterId;
            DateTime timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            // 1. Clean up existing files and records
            var existingFiles = clsMetersProd1.tbl_FotaFileUpload.Where(f => f.MeterID == meterIdStr).ToList();
            if (existingFiles.Any())
            {
                foreach (var file in existingFiles)
                {
                    string fullPath = Path.Combine(folderPath, file.FileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                clsMetersProd1.tbl_FotaFileUpload.RemoveRange(existingFiles);
            }

            // 2. Save new files and create records
            foreach (var uploadedFile in files)
            {
                string fileName = Path.GetFileName(uploadedFile.FileName);
                string fullPath = Path.Combine(folderPath, fileName);
                uploadedFile.SaveAs(fullPath);

                var fotaRecord = new tbl_FotaFileUpload
                {
                    MeterID = meterIdStr,
                    FileName = fileName,
                    CreatedDate = timestamp
                };
                clsMetersProd1.tbl_FotaFileUpload.Add(fotaRecord);
            }

            // 3. Save all changes
            clsMetersProd1.SaveChanges();
        }

        private string GenerateFotaCommand(FotaActionType action, out string message)
        {
            DateTime now = DateTime.Now;
            string datetime_hex = $"{now.Day:X2},{now.Month:X2},{now.Year:X4},{now.Hour:X2},{now.Minute:X2},{now.Second:X2}";
            string commandCode;

            switch (action)
            {
                case FotaActionType.Initiate:
                    commandCode = "30";
                    message = "Initiate FOTA command generated.";
                    break;
                case FotaActionType.Continue:
                    commandCode = "31";
                    message = "Continue FOTA command generated.";
                    break;
                case FotaActionType.Abort:
                    commandCode = "32";
                    message = "Abort FOTA command generated.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), "Invalid FOTA action specified.");
            }

            return $"02,03,00,07,40,{commandCode},{datetime_hex},03";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clsMetersProd.Dispose();
                clsMetersProd1.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}