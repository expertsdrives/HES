using HESMDMS.Models;
using Newtonsoft.Json;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace HESMDMS.Areas.Admin.Controllers
{
    public class AccountingController : Controller
    {
        SmartMeterEntities clsDb = new SmartMeterEntities();
        // GET: Admin/Accounting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GenerateInvoice()
        {
            var monthName = from post in clsDb.sp_GenerateInvoice()
                            select new { MonthName = post.MonthOfSales };
            ViewBag.monthName = JsonConvert.SerializeObject(monthName.Distinct());
            return View();
        }   
        public ActionResult InvoiceGenerate()
        {
            string strDDLValue = Request.Form["MonthName"].ToString();
            var custData = clsDb.sp_GenerateInvoice().Where(x=>x.MonthOfSales== strDDLValue).ToList();  
            foreach (var data in custData)
            {
                var path = HttpContext.Server.MapPath("~/Invoice/");
                var htmlPath = HttpContext.Server.MapPath("~/Invoice/Index.html");
                var pdfPath = HttpContext.Server.MapPath("~/Invoice/test.pdf");
                string htmlData = ExportPDF.ReplaceHTML(htmlPath);
                //htmlData = htmlData.Replace("#customerName", data.FullName);
                //htmlData = htmlData.Replace("#customerID", data.CustomerID);
                ExportPDF.GenerateHTMLtoPDF(pdfPath, htmlData);
            }
            return View();
        }
    }
}