using HESMDMS.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using EmailLib;
using Newtonsoft.Json;

namespace HESMDMS.Controllers
{
    public class LoginController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        // GET: Login
        public ActionResult Index()
        {
            string jsonData = @"{
    'idType': 'PLD', 'id': 'PAKrAad9vFUAtoAg','transactionId': '250119944541','retentionTime': '2022 - 05 - 09T13: 45:00.000Z','data': [
    {
                'aid': 'A609587662',
      'dataformat': 'cp',
      'dataType': 'JSON',
      'ext': '{\'tid\': \'250119944541\',  \'ack\': true, \'cmd\': \'closeValue\'}'
    }

  ]
}";
            var json = JsonConvert.DeserializeObject(jsonData);
            ViewBag.bodydata = json;
            tbl_AdminCredentials clsAdmin = new tbl_AdminCredentials();
            ViewBag.unauthorized = "false";
            return View(clsAdmin);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Authenticate(tbl_AdminCredentials clsAdmin)
        {
            if (Request.Browser.IsMobileDevice)
            {
                // Code for mobile devices
                ViewBag.DeviceType = "Mobile";
            }
            var data = clsMeter.tbl_AdminCredentials.Where(x => x.Username == clsAdmin.Username && x.Password == clsAdmin.Password).FirstOrDefault();

            if (Convert.ToString(data.Username) != "")
            {
                if (Convert.ToString(Session["Uthorized"]) == "Uthorized")
                {
                    Session.Remove("Uthorized");
                }
                Session.Add("RoleID", data.RoleID);
                Session.Add("FullName", data.FullName);
                Session.Add("Username", data.Username);
                Session.Add("UserID", data.ID);
                FormsAuthentication.SetAuthCookie(data.FullName, false);
                if (data.RoleID == 5)
                {

                    return RedirectToAction("../SmartMeter/User/");
                }
                if (data.RoleID == 8)
                {

                    return RedirectToAction("../SmartMeter/SamrtMeterData/ElectricMeterView");
                }
                else
                {
                    Session.Add("Admin", "Admin");
                    return RedirectToAction("../Admin/Dashboad/Index");
                }
            }
            else
            {
                Session.Add("Uthorized", "Uthorized");
                return RedirectToAction("Index");
            }


        }
        [Authorize]

        [SessionRequired]
        public ActionResult Dashboard()
        {
            int totalMeter = clsMeter.tbl_CustomerRegistration.Where(x => x.Address.Contains("Khurja")).Count();
            int totalApproved = clsMeter.tbl_CustomerRegistration.Where(x => x.Status == "Approved" && x.Address.Contains("Khurja")).Count();
            int totalPending = clsMeter.tbl_CustomerRegistration.Where(x => x.Status == "Pending" && x.Address.Contains("Khurja")).Count();
            ViewBag.totalmeter = totalMeter;
            ViewBag.totalappmeter = totalApproved;
            ViewBag.totalpenmeter = totalPending;
            return View();
        }
        public JsonResult ApproveMeter(int ID)
        {
            string fullname = Convert.ToString(Session["FullName"]);
            clsMeter.Database.ExecuteSqlCommand("UPDATE tbl_CustomerRegistration SET Status = 'Approved',ApprovedBy='" + fullname + "' WHERE ID = '" + ID + "'");
            return Json("Done", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}