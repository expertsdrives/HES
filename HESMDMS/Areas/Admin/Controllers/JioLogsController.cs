using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace HESMDMS.Areas.Admin.Controllers
{
    [SessionRequired]
    public class JioLogsController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        // GET: Admin/JioLogs
        public ActionResult Index()
        {
            
            return View();
        }
    }
}