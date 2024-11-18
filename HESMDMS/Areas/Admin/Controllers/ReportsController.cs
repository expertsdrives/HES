using DevExtreme.AspNet.Data;
using HESMDMS.Models;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static HESMDMS.Controllers.GISController;

namespace HESMDMS.Areas.Admin.Controllers
{
    [Authorize]
    [SessionRequired]
    public class ReportsController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        SmartMeterEntities1 clsMeter1 = new SmartMeterEntities1();
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
            DateTime start = Convert.ToDateTime("2024-10-31");
            DateTime end = Convert.ToDateTime("2024-11-06");
            var dd = clsMeter1.sp_FetchConsumptionData(start, end).ToList();
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
        public ActionResult OutofRange()
        {
            return View();
        }
        public ActionResult magnetictamper()
        {
            return View();
        }
        public ActionResult SLAReport()
        {
            return View();
        }
        public ActionResult Circle1Report() { 
            return View();
        }
        public JsonResult circle1Data(string startDate,string endDate) {
            DateTime sdate = Convert.ToDateTime(startDate);
            DateTime edate = Convert.ToDateTime(endDate);
            clsMeter.Database.CommandTimeout = 300;
            var activeAMR = clsMeter.Database.SqlQuery<tbl_Consumption>(
          "exec sp_Circle1Temp @p0, @p1",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate)  // Pass an empty string or any other parameter if required
      ).ToList();
            return Json(new
            {
                amr = Newtonsoft.Json.JsonConvert.SerializeObject(activeAMR),
                
              
            });
        }
        public ActionResult ClusterReport()
        {
            return View();
        }
        public JsonResult clusterReportData(string startDate, string endDate)
        {
            DateTime sdate = Convert.ToDateTime(startDate);
            DateTime edate = Convert.ToDateTime(endDate);
            clsMeter.Database.CommandTimeout = 300;
            var activeAMR = clsMeter1.sp_ClusterWiseReport(sdate, edate).ToList();
            var com  = activeAMR.Where(x=>x.communication_status== "Communication").Count();
            var noncom  = activeAMR.Where(x=>x.communication_status== "Non Communication").Count();
            var result = new
            {
                ActiveAMR = activeAMR,
                OtherData1 = com,
                OtherData2 = noncom
            };

            return new JsonResult
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }
    }
}