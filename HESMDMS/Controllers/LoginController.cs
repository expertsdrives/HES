using HESMDMS.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Sentry;

namespace HESMDMS.Controllers
{
    #region Best Practices: Constants
    // Using constants for session keys and roles prevents typos and makes the code easier to maintain.
    public static class UserRoles
    {
        public const int SmartMeterUser = 5;
        public const int ElectricAdmin = 8;
        public const int MeterReader = 11;
        public const int Customer = 12;
    }

    public static class SessionKeys
    {
        public const string RoleId = "RoleID";
        public const string UserType = "usertype";
        public const string FullName = "FullName";
        public const string Username = "Username";
        public const string UserId = "UserID";
        public const string IsAdmin = "Admin";
        public const string OtpUthorized = "UthorizedOTP";
        public const string Uthorized = "Uthorized";
        public const string MobileNotFound = "MobileNotfound";
        public const string OtpValue = "OTPValue";
    }
    #endregion

    public class LoginController : Controller
    {
        #region Best Practices: Dependency Injection
        // The DbContext should be injected using a dependency injection container.
        // This improves testability and decouples the controller from the data access layer.
        // For now, we'll manage its lifecycle manually, but DI is the recommended approach.
        private readonly SmartMeterEntities _db;

        public LoginController()
        {
            _db = new SmartMeterEntities();
        }
        #endregion

        // GET: Login
        public ActionResult Index()
        {
            // The hardcoded JSON is removed as it seemed to be for testing purposes.
            // If it's needed, it should be loaded from a more appropriate source.
            ViewBag.unauthorized = "false";
            return View(new tbl_AdminCredentials());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuthenticateOTP(GenerateOTP model)
        {
            var originalOtp = Session[SessionKeys.OtpValue]?.ToString();

            if (model.OTP == originalOtp)
            {
                Session.Remove(SessionKeys.OtpValue);
                return RedirectToAction("Index", "Dashboad", new { area = "Admin" });
            }

            Session[SessionKeys.OtpUthorized] = "UthorizedOTP";
            // Add user feedback about the invalid OTP
            ModelState.AddModelError("OTP", "Invalid OTP provided.");
            return View("GetOTP");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authenticate(tbl_AdminCredentials model)
        {
            #region Best Practices: Password Hashing
            // SECURITY ALERT: Passwords should NEVER be stored in plaintext.
            // The following line is a major security vulnerability.
            // You should store hashed and salted passwords and compare the hashes.
            // Example: var user = _db.tbl_AdminCredentials.FirstOrDefault(u => u.Username == model.Username);
            // if (user != null && PasswordHasher.VerifyPassword(model.Password, user.PasswordHash)) { ... }
            #endregion
            var user = _db.tbl_AdminCredentials.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            if (user == null)
            {
                Session[SessionKeys.Uthorized] = "Uthorized";
                // Add user feedback about invalid credentials
                ModelState.AddModelError("", "Invalid username or password.");
                return RedirectToAction("Index");
            }

            EstablishSession(user);

            if (user.SendOTP == true)
            {
                if (string.IsNullOrWhiteSpace(user.MobileNumber))
                {
                    Session[SessionKeys.MobileNotFound] = "MobileNotfound";
                    return RedirectToAction("Index");
                }
                // Redirect to GetOTP which will handle sending the OTP
                return RedirectToAction("GetOTP");
            }

            return RedirectToRoleBasedDashboard(user.RoleID);
        }

        [Authorize]
        public async Task<ActionResult> GetOTP()
        {
            if (Session[SessionKeys.UserId] == null || !long.TryParse(Session[SessionKeys.UserId].ToString(), out long userId))
            {
                // Handle case where user ID is not in session or invalid
                return RedirectToAction("Index");
            }

            var userDetails = await _db.tbl_AdminCredentials.FirstOrDefaultAsync(x => x.ID == userId);
            if (userDetails == null || string.IsNullOrWhiteSpace(userDetails.MobileNumber))
            {
                Session[SessionKeys.MobileNotFound] = "MobileNotfound";
                return RedirectToAction("Index");
            }

            var otpValue = new Random().Next(100000, 999999);
            Session[SessionKeys.OtpValue] = otpValue.ToString();
            ViewBag.OTPValue = otpValue; // For display/debugging purposes, consider removing in production

            await SendOtpSmsAsync(userDetails.MobileNumber, otpValue.ToString());

            return View();
        }

        [SessionRequired]
        public async Task<ActionResult> Dashboard()
        {
            #region Performance Optimization
            // The three separate queries can be combined into a single database round-trip using grouping.
            var stats = await _db.tbl_CustomerRegistration
                .Where(x => x.Address.Contains("Khurja"))
                .GroupBy(x => 1) // Group by a constant to aggregate all results
                .Select(g => new
                {
                    TotalMeter = g.Count(),
                    TotalApproved = g.Count(c => c.Status == "Approved"),
                    TotalPending = g.Count(c => c.Status == "Pending")
                })
                .FirstOrDefaultAsync();

            ViewBag.totalmeter = stats?.TotalMeter ?? 0;
            ViewBag.totalappmeter = stats?.TotalApproved ?? 0;
            ViewBag.totalpenmeter = stats?.TotalPending ?? 0;
            #endregion

            return View();
        }
        public JsonResult ApproveMeter(int ID)
        {
            string fullname = Convert.ToString(Session["FullName"]);
            _db.Database.ExecuteSqlCommand("UPDATE tbl_CustomerRegistration SET Status = 'Approved',ApprovedBy='" + fullname + "' WHERE ID = '" + ID + "'");
            return Json("Done", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); // Use Clear() for a more definitive session reset
            return RedirectToAction("Index");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        #region Private Helper Methods
        // Encapsulating logic in private methods improves readability and reusability.
        private void EstablishSession(tbl_AdminCredentials user)
        {
            Session.Remove(SessionKeys.Uthorized);
            Session[SessionKeys.RoleId] = user.RoleID;
            Session[SessionKeys.FullName] = user.FullName;
            Session[SessionKeys.Username] = user.Username;
            Session[SessionKeys.UserId] = user.ID;

            // Set a generic user type and then specify for admin roles
            Session[SessionKeys.UserType] = "General";
            if (user.RoleID == UserRoles.MeterReader) Session[SessionKeys.UserType] = "Meter Reader";
            if (user.RoleID == UserRoles.Customer) Session[SessionKeys.UserType] = "Customer";
            if (user.RoleID == UserRoles.ElectricAdmin) Session[SessionKeys.UserType] = "Electric Admin";

            FormsAuthentication.SetAuthCookie(user.FullName, false);
        }

        private ActionResult RedirectToRoleBasedDashboard(int? roleId)
        {
            switch (roleId)
            {
                case UserRoles.SmartMeterUser:
                    return RedirectToAction("Index", "User", new { area = "SmartMeter" });
                case UserRoles.ElectricAdmin:
                    return RedirectToAction("ElectricMeterView", "SamrtMeterData", new { area = "SmartMeter" });
                case UserRoles.MeterReader:
                case UserRoles.Customer:
                    return RedirectToAction("Index", "ElectricBLE", new { area = "SmartMeter" });
                default:
                    Session[SessionKeys.IsAdmin] = "Admin";
                    return RedirectToAction("Index", "Dashboad", new { area = "Admin" });
            }
        }

        private async Task SendOtpSmsAsync(string mobileNumber, string otp)
        {
            #region Best Practices: Configuration Management
            // API keys, URLs, and other sensitive data should not be hardcoded.
            // They should be stored securely in Web.config's <appSettings> or Azure Key Vault.
            // Example:
            // var apiUrl = ConfigurationManager.AppSettings["SmsApiUrl"];
            // var user = ConfigurationManager.AppSettings["SmsApiUser"];
            #endregion
            var hardcodedUrl = $"http://api.bulksmsgateway.in/sendmessage.php?user=krutish&password=krutish123$&mobile={mobileNumber}&message=Your One Time Password (OTP) for SMTPL HES is {otp}. Thanks EDCLLP&sender=EDCLLP&type=3&template_id=1007415933773306729";

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, hardcodedUrl))
                {
                    // Use await to ensure the call completes and to handle potential exceptions.
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for troubleshooting.
                SentrySdk.CaptureException(ex);
                // Optionally, handle the failed SMS sending case (e.g., notify an admin).
            }
        }
        #endregion

        #region Best Practices: Resource Management
        // The DbContext should be disposed of properly.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}