using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Controllers
{
    public class GISController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeterProd = new SmartMeter_ProdEntities();
        // GET: GIS
        public ActionResult Index()
        {
            return View();
        }
        public class inactiveAMR
        {
            public string SerialNumber { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }
        public class inactiveVayudut
        {
            public string SerialNumber { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }
        public class activeAMR
        {
            public string AMRSerialNumber { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }
        [HttpPost]
        public JsonResult CallToMap(string startDate, string endDate)
        {
            DateTime sdate = Convert.ToDateTime(startDate);
            DateTime edate = Convert.ToDateTime(endDate);
            clsMeter.Database.CommandTimeout = 300;
            var activeAMR = clsMeter.Database.SqlQuery<activeAMR>(
          "exec sp_ActiveAMR @p0, @p1",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate)  // Pass an empty string or any other parameter if required
      ).ToList();
            var missingConsumptionSerials = clsMeter.Database.SqlQuery<inactiveAMR>(
          "exec sp_InactiveAMR @p0, @p1",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate)  // Pass an empty string or any other parameter if required
      ).ToList();
            var activeVayudut = (from consumption in clsMeter.tbl_Consumption
                                 join customer in clsMeter.tbl_VayadutMasterDetails
                                 on consumption.Vayudut_ID equals customer.VayadutId
                                 where consumption.Date >= sdate && consumption.Date <= edate
                                 select new
                                 {
                                     VayudutID = consumption.Vayudut_ID,
                                     Latitude = customer.Latitude,
                                     Longitude = customer.Longitude
                                 })
                     .Distinct()
                     .ToList();

            
            var missingYayudutSerials = clsMeter.Database.SqlQuery<inactiveVayudut>(
          "exec sp_InactiveVayudut @p0, @p1",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate)  // Pass an empty string or any other parameter if required
      ).ToList();

            var ActiveAMRJson = activeAMR
      .Select(x => new List<object>
      {
        double.TryParse(x.Latitude, out double lat) ? lat : 0.0,   // Parse Latitude to double, default to 0.0 on failure
        double.TryParse(x.Longitude, out double lng) ? lng : 0.0,  // Parse Longitude to double, default to 0.0 on failure
        x.AMRSerialNumber // Add string data (replace 'SomeStringData' with the actual field/property name)
      })
      .ToList();



            var missingARMJson = missingConsumptionSerials
      .Select(x => new List<object>
      {
        double.TryParse(x.Latitude, out double lat) ? lat : 0.0,   // Parse Latitude to double, default to 0.0 on failure
        double.TryParse(x.Longitude, out double lng) ? lng : 0.0,
        x.SerialNumber// Parse Longitude to double, default to 0.0 on failure
      })
      .ToList();

            var ActiveVayudutJson = activeVayudut
    .Select(x => new List<object> { x.Latitude, x.Longitude,x.VayudutID })
    .ToList();
            var missingVayudutJson = missingYayudutSerials
     .Select(x => new List<object>
     {
        double.TryParse(x.Latitude, out double lat) ? lat : 0.0,   // Parse Latitude to double, default to 0.0 on failure
        double.TryParse(x.Longitude, out double lng) ? lng : 0.0,
        x.SerialNumber// Parse Longitude to double, default to 0.0 on failure
     })
     .ToList();
            var serialNumber = activeAMR
      .Select(x => new List<string>
      {
        x.AMRSerialNumber
      })
      .ToList();
            return Json(new
            {
                ActiveAMRJson = Newtonsoft.Json.JsonConvert.SerializeObject(ActiveAMRJson),
                ActiveAMRCount = activeAMR.Count(),
                missingARMJson = Newtonsoft.Json.JsonConvert.SerializeObject(missingARMJson),
                inactiveAMRCount= missingConsumptionSerials.Count(),
                ActiveVayudutJson = Newtonsoft.Json.JsonConvert.SerializeObject(ActiveVayudutJson),
                ActiveVayudutCount=activeVayudut.Count(),
                missingVayudutJson = Newtonsoft.Json.JsonConvert.SerializeObject(missingVayudutJson),
                missingVayudutCount= missingYayudutSerials.Count(),
                sr= Newtonsoft.Json.JsonConvert.SerializeObject(serialNumber)
            });
        }
    }
}