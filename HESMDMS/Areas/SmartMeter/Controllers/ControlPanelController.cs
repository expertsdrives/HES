using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class ControlPanelController : Controller
    {
        // GET: SmartMeter/ControlPanel
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UpdateRates() { return View(); }

        public ActionResult PaymentTranscation() { return View(); }
    }
}