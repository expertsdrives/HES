using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}