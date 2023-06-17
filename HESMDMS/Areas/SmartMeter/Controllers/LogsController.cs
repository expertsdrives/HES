using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class LogsController : Controller
    {
        SmartMeterEntities clsDB = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
        // GET: SmartMeter/Logs
        public ActionResult Index()
        {

            return View();
        }

        
    }
}