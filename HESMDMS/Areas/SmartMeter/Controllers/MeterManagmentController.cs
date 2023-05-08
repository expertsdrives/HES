using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class MeterManagmentController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        // GET: SmartMeter/MeterManagment
        public ActionResult Index()
        {
            var data= clsMeters.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();
        }
    }
}