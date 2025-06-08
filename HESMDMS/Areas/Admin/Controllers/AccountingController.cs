using HESMDMS.Models;
using HESMDMS.Services; // Assuming a service layer for business logic
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] // Secure the controller
    public class AccountingController : Controller
    {
        private readonly SmartMeterEntities _clsDb;
        private readonly IInvoiceService _invoiceService;

        // Use Dependency Injection for DbContext and services
        public AccountingController(SmartMeterEntities clsDb, IInvoiceService invoiceService)
        {
            _clsDb = clsDb;
            _invoiceService = invoiceService;
        }

        public AccountingController() : this(new SmartMeterEntities(), new InvoiceService())
        {
            // This constructor is for the default model binder.
            // A proper DI container would handle this.
        }

        // GET: Admin/Accounting
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Accounting/GenerateInvoice
        public ActionResult GenerateInvoice()
        {
            try
            {
                var monthNames = _clsDb.sp_GenerateInvoice()
                                       .Select(p => p.MonthOfSales)
                                       .Distinct()
                                       .ToList();

                ViewBag.MonthNames = new SelectList(monthNames);
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using Elmah, Serilog, etc.)
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View("Error", new HandleErrorInfo(ex, "Accounting", "GenerateInvoice"));
            }
        }

        // POST: Admin/Accounting/GenerateInvoice
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public ActionResult GenerateInvoice(string monthName)
        {
            if (string.IsNullOrEmpty(monthName))
            {
                ModelState.AddModelError("", "Please select a month.");
                // Re-populate the dropdown if there's an error
                var monthNames = _clsDb.sp_GenerateInvoice()
                                       .Select(p => p.MonthOfSales)
                                       .Distinct()
                                       .ToList();
                ViewBag.MonthNames = new SelectList(monthNames);
                return View();
            }

            try
            {
                var invoiceData = _clsDb.sp_GenerateInvoice()
                                        .Where(x => x.MonthOfSales == monthName)
                                        .ToList();

                foreach (var data in invoiceData)
                {
                    _invoiceService.GenerateAndSaveInvoice(data, Server);
                }

                TempData["SuccessMessage"] = $"Invoices for {monthName} generated successfully.";
                return RedirectToAction("GenerateInvoice");
            }
            catch (Exception ex)
            {
                // Log the exception
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View("Error", new HandleErrorInfo(ex, "Accounting", "GenerateInvoice"));
            }
        }
    }
}