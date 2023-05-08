using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        // GET: Admin/Reports
        public ActionResult Index()
        {
            if (Convert.ToString(Session["FullName"]) != "")
            {
                DatabaseContext dbContext = new DatabaseContext();
                DataSet set15DayReport = new DataSet();
                DataTable table15DayReport = new DataTable();
                set15DayReport = dbContext.RunDynamicSP("sp_Get15DayReport", "WEB");
                table15DayReport = set15DayReport.Tables[0];
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.DateFormatString = "dd-MM-yyyy";
                string dayreport = JsonConvert.SerializeObject(table15DayReport, jsonSettings);
                ViewBag.dayreport = dayreport;
                return View();
            }
            else
            {
                return RedirectToAction("../../Login");
            }
        }
        public ActionResult LastMeterReading()
        {
            //string strDDLValue = Request.Form["BP"].ToString();
            //ViewBag.POCLocation= strDDLValue;
            return View();
        }
        public ActionResult LastMeterRevenue()
        {
            return View();
        }
        public ActionResult MagneticTemper()
        { 
            return View();
        }
        public ActionResult Tamper()
        {
            return View();
        }
        public ActionResult VayudutWise()
        {
            return View();
        }
        public ActionResult BatteryTemprature()
        {
            return View();
        }
    }
}