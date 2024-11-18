using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class ElectricBLEController : Controller
    {
        // GET: SmartMeter/ElectricBLE

        ElectricMeterEntities clsElectric = new ElectricMeterEntities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult sendToHES(string current)
        {
            tbl_BLEDemo tbl_BLEDemo = new tbl_BLEDemo();
            tbl_BLEDemo.Currentkw = current;
            tbl_BLEDemo.MeterID = "10000245";
            tbl_BLEDemo.CapturedBy = Session["Username"].ToString();
            tbl_BLEDemo.User_Type = Session["usertype"].ToString();
            tbl_BLEDemo.CreatedDate = DateTime.Now;
            clsElectric.tbl_BLEDemo.Add(tbl_BLEDemo);
            clsElectric.SaveChanges();
            return Json(clsElectric, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BLELogs()
        {
            return View();
        }
    }
}