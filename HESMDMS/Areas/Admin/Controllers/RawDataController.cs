using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HESMDMS.Models;
namespace HESMDMS.Areas.Admin.Controllers
{
    public class RawDataController : Controller
    {
        SmartMeterEntities _db = new SmartMeterEntities();
        // GET: Admin/RawData
        public ActionResult Index()
        {
            return View(_db.FetchConsumption().ToList());
        }
    }
}