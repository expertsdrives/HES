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
        public int? SelectedMeterId { get; set; }
    }

    public class FotaController : Controller
    {
        //TODO: Using two different DbContexts is unusual. This should be reviewed.
        //For now, we will assume this is intentional.
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        SmartMeter_ProdEntities1 clsMetersProd1 = new SmartMeter_ProdEntities1();

        // GET: Admin/Fota
        public ActionResult Index(int? meterId)
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
                // Log the exception
                return Json(new { success = false, message = "An error occurred while fetching meter details." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadWithMeter(IEnumerable<HttpPostedFileBase> uploadedFiles, int selectedMeter)
        {
            if (uploadedFiles == null || !uploadedFiles.Any() || uploadedFiles.All(f => f == null || f.ContentLength == 0))
            {
                TempData["Message"] = "No files selected or files are empty.";
                return RedirectToAction("Index", new { meterId = selectedMeter });
            }

            try
            {
                string folderPath = Server.MapPath("~/UploadedFiles/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Delete existing files for the meter
                var existingFiles = clsMetersProd1.tbl_FotaFileUpload.Where(f => f.MeterID == selectedMeter.ToString()).ToList();
                foreach (var file in existingFiles)
                {
                    string fullPath = Path.Combine(folderPath, file.FileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                clsMetersProd1.tbl_FotaFileUpload.RemoveRange(existingFiles);
                
                int chunkNumber = 0;
                DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                foreach (var uploadedFile in uploadedFiles.Where(f => f != null && f.ContentLength > 0))
                {
                    string formattedChunk = chunkNumber.ToString("D4");
                    string newFileName = $"chunk_{selectedMeter}_{formattedChunk}.bin";
                    string fullPath = Path.Combine(folderPath, newFileName);
                    
                    uploadedFile.SaveAs(fullPath);

                    tbl_FotaFileUpload fota = new tbl_FotaFileUpload
                    {
                        MeterID = selectedMeter.ToString(),
                        FileName = newFileName,
                        CreatedDate = istTime
                    };
                    clsMetersProd1.tbl_FotaFileUpload.Add(fota);
                    chunkNumber++;
                }

                clsMetersProd1.SaveChanges();
                TempData["Message"] = $"{chunkNumber} file(s) uploaded successfully for meter {selectedMeter}.";
            }
            catch (Exception ex)
            {
                // Log the exception ex
                TempData["Message"] = "An error occurred during file upload.";
            }

            return RedirectToAction("Index", new { meterId = selectedMeter });
        }
    }
}