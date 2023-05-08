using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HESMDMS.Models;
namespace HESMDMS.Controllers
{
    [Authorize]
    public class DataReceptionController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        // GET: AMRData
        public ActionResult Index()
        {
            var rawData = clsMeter.tbl_RawDataAPI.Count();
            ViewBag.AMRData = rawData;
            return View();
        }
        public ActionResult AMRDataReceptionWithCRC()
        {
            if (Convert.ToString(Session["FullName"]) != "")
            {
                DateTime date1 = Convert.ToDateTime("1-11-2022");
                var countBusinness = clsMeter.tbl_CustomerRegistration.Select(x => x.BusinessPartnerNo).Distinct().Count();
                ViewBag.DistinctBusiness = countBusinness;

                var countMeter = clsMeter.tbl_CustomerRegistration.Select(x => x.MeterNumber).Distinct().Count();
                ViewBag.DistinctMeterNumber = countMeter;
                //var rawDataCRC = clsMeter.FetchConsumption().Count();
                //ViewBag.AMRDataCRC = rawDataCRC;
                return View();
            }
            else
            {
                return RedirectToAction("../Login");

            }

        }
        public ActionResult InductiveAMR()
        {
            return View();
        }

    }
}