using HESMDMS.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using EmailLib;
using Newtonsoft.Json;
using System.Net.Http;
using Sentry;

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
        public ActionResult AuthenticateOTP(GenerateOTP clsAdmin)
        {
            var ogOTP = Convert.ToString(Session["OTPValue"]);
            var gOTp = clsAdmin.OTP;
            if (gOTp == ogOTP)
            {
                return RedirectToAction("../Admin/Dashboad/Index");
            }
            else
            {
                Session.Add("UthorizedOTP", "UthorizedOTP");
                return View("GetOTP");
            }
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
            if (data != null)
            {
                if (Convert.ToString(data.Username) != "")
                {
                    if (Convert.ToString(Session["Uthorized"]) == "Uthorized")
                    {
                        Session.Remove("Uthorized");
                    }
                    Session.Add("RoleID", data.RoleID);
                    if (data.RoleID == 11)
                    {
                        Session.Add("usertype", "Meter Reader");
                    }
                    if (data.RoleID == 12)
                    {
                        Session.Add("usertype", "Customer");
                    }
                    if (data.RoleID == 8)
                    {
                        Session.Add("usertype", "Electic Admin");
                    }
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
                    if (data.RoleID == 11)
                    {
                        return RedirectToAction("../SmartMeter/ElectricBLE");
                    }
                    if (data.RoleID == 12)
                    {
                        return RedirectToAction("../SmartMeter/ElectricBLE");
                    }
                    else
                    {
                        Session.Add("Admin", "Admin");
                        if (data.RoleID != 7 || data.ID == 114)
                        {
                            if (data.MobileNumber != null)
                            {
                                return RedirectToAction("../Login/GetOTP");
                            }
                            else
                            {
                                Session.Add("MobileNotfound", "MobileNotfound");
                                return RedirectToAction("Index");
                            }
                        }
                        else {
                            return RedirectToAction("../Admin/Dashboad/Index");
                        }
                    }
                }
                else
                {
                    Session.Add("Uthorized", "Uthorized");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Session.Add("Uthorized", "Uthorized");
                return RedirectToAction("Index");
            }
        }
        [Authorize]

        public ActionResult GetOTP()
        {
            var userId = Convert.ToInt64(Session["UserID"]);
            var userdetails = clsMeter.tbl_AdminCredentials.Where(x => x.ID == userId).FirstOrDefault();
            Random random = new Random();
            int otpValue = random.Next(100000, 999999);
            Session.Add("OTPValue", otpValue);
            ViewBag.OTPValue = otpValue;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://api.bulksmsgateway.in/sendmessage.php?user=krutish&password=krutish123$&mobile=" + userdetails.MobileNumber + "&message=Your One Time Password (OTP) for SMTPL HES is " + otpValue + ". Thanks EDCLLP&sender=EDCLLP&type=3&template_id=1007415933773306729");
            var response = client.SendAsync(request);

            return View();
        }

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