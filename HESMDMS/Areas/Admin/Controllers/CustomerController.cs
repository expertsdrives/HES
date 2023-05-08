using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
       
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        // GET: Admin/Customer
        [Authorize]
        public ActionResult Index()
        {
            var Meters = clsMeters.tbl_MeterMaster.Where(x => x.IsActive == true).ToList();
            ViewBag.Meters = Meters;
            return View();
        }
    }
}