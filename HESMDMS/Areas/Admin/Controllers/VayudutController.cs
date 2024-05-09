using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class VayudutController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        // GET: Admin/Vayudut
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InsertVayudut(tbl_VayudutRegistration vayudut)
        {
            var model = clsMeter.tbl_VayudutRegistration;
            model.Add(vayudut);
            clsMeter.SaveChanges();
            return View();
        }
    }
}