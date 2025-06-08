using System;
using System.Linq;
using System.Web.Mvc;
using HESMDMS.Models;
using System.Data.Entity;
using Org.BouncyCastle.Asn1;
using NPOI.SS.Formula.Functions;
using Stripe;
using Microsoft.WindowsAzure.Storage.Table.Queryable;
using System.Collections.Generic;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class ATGLRatesController : Controller   
    {
        private SmartMeterEntities1 clsMet = new SmartMeterEntities1();


         // Fetch Meter List for Dropdown
        public JsonResult GetMeters()
        {
            var meters = clsMet.tbl_SGMReg.Select(m => new
            {
                MeterID = m.ID,
                MeterName = m.SmartMeterSerialNumber
            }).ToList();

            return Json(meters, JsonRequestBehavior.AllowGet);
        }

        // Fetch Meter List for Dropdown
        public JsonResult GetGroups()
        {
            var meters = clsMet.tbl_MeterGroup.Select(m => new
            {
                GroupID = m.ID,
                GroupName = m.GroupName
            }).ToList();

            return Json(meters, JsonRequestBehavior.AllowGet);
        }





        // GET: Admin/ATGLRates
        public ActionResult GasRateSlab1()
        {
            return View();
        }

        // Fetch Gas Rate Slabs for DataGrid
        [HttpGet]
        public JsonResult GetGasRates()
        {
            try
            {
                var rates = clsMet.tbl_GasRateSlabs_1
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.GasRateSlab1,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        GasRateSlab1 = r.GasRateSlab1.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        // Add Gas Rate
        [HttpPost]
        public JsonResult AddGasRate(tbl_GasRateSlabs_1 gasRate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    gasRate.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasRateSlabs_1.Add(gasRate);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Rate Slab 1 added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        // Update Gas Rate
        [HttpPost]
        public JsonResult UpdateGasRate(tbl_GasRateSlabs_1 gasRate)
        {
            var existingRate = clsMet.tbl_GasRateSlabs_1.Find(gasRate.ID);
            if (existingRate != null)
            {
                existingRate.GroupName = gasRate.GroupName;
                existingRate.GasRateSlab1 = gasRate.GasRateSlab1;
                existingRate.Date = gasRate.Date;

                clsMet.SaveChanges();
                return Json(new { success = true, message = "Gas Rate Slab 1 updated successfully!" });
            }
            return Json(new { success = false, message = "Update failed!" });
        }

        // Delete Gas Rate
        [HttpPost]
        public JsonResult DeleteGasRate(int id)
        {
            var gasRate = clsMet.tbl_GasRateSlabs_1.Find(id);
            if (gasRate != null)
            {
                clsMet.tbl_GasRateSlabs_1.Remove(gasRate);
                clsMet.SaveChanges();
                return Json(new { success = true, message = "Gas Rate Slab 1 deleted successfully!" });
            }
            return Json(new { success = false, message = "Delete failed!" });
        }








        public ActionResult GasQtySlabs1()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasQtySlabs()
        {
            try
            {
                var rates = clsMet.tbl_GasqtySlabs_1
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.GasqtySlab1,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        GasqtySlab1 = r.GasqtySlab1.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasQtySlab(tbl_GasqtySlabs_1 model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasqtySlabs_1.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity Slab 1 added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasQtySlab(tbl_GasqtySlabs_1 model)
        {
            try
            {
                var existingQty = clsMet.tbl_GasqtySlabs_1.Find(model.ID);
                if (existingQty != null)
                {
                    existingQty.GroupName = model.GroupName;
                    existingQty.GasqtySlab1 = model.GasqtySlab1;
                    existingQty.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity Slab 1 updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasQtySlab(int id)
        {
            try
            {
                var gasQty = clsMet.tbl_GasqtySlabs_1.Find(id);
                if (gasQty != null)
                {
                    clsMet.tbl_GasqtySlabs_1.Remove(gasQty);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity Slab 1 deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }






        public ActionResult GasRateSlabs2()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetGasRateSlabs2()
        {
            try
            {
                var rates = clsMet.tbl_GasRateSlabs_2
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.GasrateSlab2,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        GasRateSlab2 = r.GasrateSlab2.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasRateSlab2(tbl_GasRateSlabs_2 model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set properly
                    clsMet.tbl_GasRateSlabs_2.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Rate Slab2 added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasrateSlab2(tbl_GasRateSlabs_2 model)
        {
            try
            {
                var existingrate = clsMet.tbl_GasRateSlabs_2.Find(model.ID);
                if (existingrate != null)
                {
                    existingrate.GroupName = model.GroupName;
                    existingrate.GasrateSlab2 = model.GasrateSlab2;
                    existingrate.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Rate Slab2 updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasRateSlab2(int id)
        {
            try
            {
                var gasrate = clsMet.tbl_GasRateSlabs_2.Find(id);
                if (gasrate != null)
                {
                    clsMet.tbl_GasRateSlabs_2.Remove(gasrate);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Rate deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }










        public ActionResult GasQtySlabs2()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasQtySlabs2()
        {
            try
            {
                var rates = clsMet.tbl_GasqtySlabs_2
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.GasqtySlab2,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        GasqtySlab2 = r.GasqtySlab2.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasQtySlab2(tbl_GasqtySlabs_2 model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasqtySlabs_2.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasQtySlab2(tbl_GasqtySlabs_2 model)
        {
            try
            {
                var existingQty = clsMet.tbl_GasqtySlabs_2.Find(model.ID);
                
                if (existingQty != null)
                {
                    existingQty.GroupName = model.GroupName;
                    existingQty.GasqtySlab2 = model.GasqtySlab2;
                    existingQty.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasQtySlab2(int id)
        {
            try
            {
                var gasQty2 = clsMet.tbl_GasqtySlabs_2.Find(id);
                if (gasQty2 != null)
                {
                    clsMet.tbl_GasqtySlabs_2.Remove(gasQty2);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Quantity deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }






        public ActionResult GasVAT()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasVAT()
        {
            try
            {
                var rates = clsMet.tbl_GasVAT
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.VAT,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        VAT = r.VAT.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasVat(tbl_GasVAT model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasVAT.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas VAT added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasVat(tbl_GasVAT model)
        {
            try
            {
                var existingQty = clsMet.tbl_GasVAT.Find(model.ID);

                if (existingQty != null)
                {
                    existingQty.GroupName = model.GroupName;
                    existingQty.VAT = model.VAT;
                    existingQty.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas VAT updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasVat(int id)
        {
            try
            {
                var gasVat = clsMet.tbl_GasVAT.Find(id);
                if (gasVat != null)
                {
                    clsMet.tbl_GasVAT.Remove(gasVat);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Vat deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }









        public ActionResult GasSurcharge()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasSurcharge()
        {
            try
            {
                var rates = clsMet.tbl_GasSurcharge
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.Surcharge,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        Surcharge = r.Surcharge.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasSurcharge(tbl_GasSurcharge model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasSurcharge.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Surcharge added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasSurcharge(tbl_GasSurcharge model)
        {
            try
            {
                var existingSu = clsMet.tbl_GasSurcharge.Find(model.ID);

                if (existingSu != null)
                {
                    existingSu.GroupName = model.GroupName;
                    existingSu.Surcharge = model.Surcharge;
                    existingSu.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Surcharge updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasSurcharge(int id)
        {
            try
            {
                var gasSu = clsMet.tbl_GasSurcharge.Find(id);
                if (gasSu != null)
                {
                    clsMet.tbl_GasSurcharge.Remove(gasSu);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Gas Surcharge deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }







        public ActionResult GasMiniCharge()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasMiniCharge()
        {
            try
            {
                var rates = clsMet.tbl_GasMinimumchargerate
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.Minimumchargerate,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        Minimumchargerate = r.Minimumchargerate.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasMiniCharge(tbl_GasMinimumchargerate model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasMinimumchargerate.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Minimum charge rate added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasMiniCharge(tbl_GasMinimumchargerate model)
        {
            try
            {
                var existingMini = clsMet.tbl_GasMinimumchargerate.Find(model.ID);

                if (existingMini != null)
                {
                    existingMini.GroupName = model.GroupName;
                    existingMini.Minimumchargerate = model.Minimumchargerate;
                    existingMini.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Minimum charge rate updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasMiniCharge(int id)
        {
            try
            {
                var gasMini = clsMet.tbl_GasMinimumchargerate.Find(id);
                if (gasMini != null)
                {
                    clsMet.tbl_GasMinimumchargerate.Remove(gasMini);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Minimum charge rate deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }






        public ActionResult GasRental()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasRental()
        {
            try
            {
                var rates = clsMet.tbl_GasRental
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.Rental,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        Rental = r.Rental.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasRental(tbl_GasRental model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasRental.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Rental added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasRental(tbl_GasRental model)
        {
            try
            {
                var existingRental = clsMet.tbl_GasRental.Find(model.ID);

                if (existingRental != null)
                {
                    existingRental.GroupName = model.GroupName;
                    existingRental.Rental = model.Rental;
                    existingRental.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Rental updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasRental(int id)
        {
            try
            {
                var gasRental = clsMet.tbl_GasRental.Find(id);
                if (gasRental != null)
                {
                    clsMet.tbl_GasRental.Remove(gasRental);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Rental deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }








        public ActionResult GasAMC()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasAMC()
        {
            try
            {
                var rates = clsMet.tbl_GasAMC
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.AMC,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        AMC = r.AMC.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasAMC(tbl_GasAMC model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasAMC.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "AMC added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasAMC(tbl_GasAMC model)
        {
            try
            {
                var existingAMC = clsMet.tbl_GasAMC.Find(model.ID);

                if (existingAMC != null)
                {
                    existingAMC.GroupName = model.GroupName;
                    existingAMC.AMC = model.AMC;
                    existingAMC.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "AMC updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasAMC(int id)
        {
            try
            {
                var gasAMC = clsMet.tbl_GasAMC.Find(id);
                if (gasAMC != null)
                {
                    clsMet.tbl_GasAMC.Remove(gasAMC);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "AMC deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }







        public ActionResult GasGCV()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetGasGCV()
        {
            try
            {
                var rates = clsMet.tbl_GasGCV
                    .Join(clsMet.tbl_MeterGroup,
                          slab => slab.GroupName,  // Ensure this is INT if matching with ID
                          meter => meter.GroupName, // Matching on the correct column
                          (slab, meter) => new
                          {
                              slab.ID,
                              GroupName = meter.GroupName,
                              slab.GCV,
                              slab.Date
                          })
                    .AsNoTracking()
                    .ToList()
                    .Select(r => new
                    {
                        r.ID,
                        r.GroupName,
                        GCV = r.GCV.ToString("0.000"), // Ensure 3 decimal places
                        Date = r.Date.ToString("yyyy-MM-dd HH:mm:ss")
                    })
                    .ToList();

                return Json(rates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddGasGCV(tbl_GasGCV model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Date = DateTime.Now; // Ensure date is set
                    clsMet.tbl_GasGCV.Add(model);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "GCV added successfully!" });
                }
                return Json(new { success = false, message = "Invalid Data" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGasGCV(tbl_GasGCV model)
        {
            try
            {
                var existingGCV = clsMet.tbl_GasGCV.Find(model.ID);

                if (existingGCV != null)
                {
                    existingGCV.GroupName = model.GroupName;
                    existingGCV.GCV = model.GCV;
                    existingGCV.Date = model.Date;

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "GCV updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGasGCV(int id)
        {
            try
            {
                var gasGCV = clsMet.tbl_GasGCV.Find(id);
                if (gasGCV != null)
                {
                    clsMet.tbl_GasGCV.Remove(gasGCV);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "GCV deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }











        public ActionResult MeterGroup()
        {
            return View();
        }

        // GET: Fetch Meter Groups with selected meters
        [HttpGet]
        public JsonResult GetMeterGroups()
        {
            try
            {
                var groups = clsMet.tbl_MeterGroup
                    .AsNoTracking()
                    .ToList()
                    .Select(g => new
                    {
                        ID = g.ID,
                        GroupName = g.GroupName,
                        MeterIds = g.MeterSelection,
                        Meters = g.MeterSelection != null
                            ? g.MeterSelection.Split(',')
                                .Select(meterId => clsMet.tbl_SGMReg
                                    .Where(m => m.ID.ToString() == meterId)
                                    .Select(m => m.SmartMeterSerialNumber)
                                    .FirstOrDefault())
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToList()
                            : new List<string>()
                    })
                    .ToList();

                return Json(groups, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error fetching data", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        

        // POST: Add Meter Group
        [HttpPost]
        public JsonResult AddMeterGroup(string groupName, List<int> meterIds)
        {
            if (!string.IsNullOrWhiteSpace(groupName) && meterIds != null && meterIds.Count > 0)
            {
                // Check for duplicate meters within the group
                if (meterIds.Count != meterIds.Distinct().Count())
                {
                    return Json(new { success = false, message = "Duplicate meters are not allowed in a group." });
                }

                tbl_MeterGroup group = new tbl_MeterGroup
                {
                    GroupName = groupName,
                    MeterSelection = string.Join(",", meterIds.Distinct()) // Save unique IDs only
                };

                clsMet.tbl_MeterGroup.Add(group);
                clsMet.SaveChanges();
                return Json(new { success = true, message = "Meter Group added successfully!" });
            }
            return Json(new { success = false, message = "Invalid Data" });
        }

        // POST: Edit Meter Group
        [HttpPost]
        public JsonResult EditMeterGroup(int id, string groupName, List<int> meterIds)
        {
            try
            {
                var group = clsMet.tbl_MeterGroup.Find(id);
                if (group != null)
                {
                    if (meterIds.Count != meterIds.Distinct().Count())
                    {
                        return Json(new { success = false, message = "Duplicate meters are not allowed in a group." });
                    }

                    group.GroupName = groupName;
                    group.MeterSelection = string.Join(",", meterIds.Distinct());

                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Meter Group updated successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating record", error = ex.Message });
            }
        }

        // POST: Delete Meter Group
        [HttpPost]
        public JsonResult DeleteMeterGroup(int id)
        {
            try
            {
                var group = clsMet.tbl_MeterGroup.Find(id);
                if (group != null)
                {
                    clsMet.tbl_MeterGroup.Remove(group);
                    clsMet.SaveChanges();
                    return Json(new { success = true, message = "Meter Group deleted successfully!" });
                }
                return Json(new { success = false, message = "Record not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting record", error = ex.Message });
            }
        }

    }
}
