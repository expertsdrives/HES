﻿using HESMDMS.Models;
using Syncfusion.EJ2.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.Admin.Controllers
{
    [Authorize]
    [SessionRequired]
    public class DashboadController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        // GET: Admin/Dashboad
        public ActionResult Index()
        {
            var custmers = clsMeters.tbl_CustomerDetails.Count();
            ViewBag.Cus = custmers;
            var amr = clsMeters.tbl_CustomerRegistration.Where(x=>x.Address.Contains("Khurja")).Count();
            ViewBag.amr = amr;
            //if (Convert.ToString(Session["FullName"]) != "")
            //{
            //    List<double> lstZero = new List<double>();
            //    List<double> lstConsump = new List<double>();
            //    List<string> lstMonth = new List<string>();
            //    double[] n = new double[7];
            //    double[] d = new double[7];
            //    string[] s = new string[12];
            //    var totalMeters = clsMeters.tbl_MeterMaster.Count();
            //    ViewBag.TotalMeters = totalMeters;

            //    var totalCustomers = clsMeters.tbl_CustomerDetails.Count();
            //    ViewBag.TotalCustomers = totalCustomers;

            //    var totalFianlAMR = clsMeters.tbl_CustomerRegistration.Count();
            //    ViewBag.TotalFianlAMR = totalFianlAMR;

            //    var totalAMR = clsMeters.sp_TotalAMR().Count();
            //    ViewBag.TotalAMR = totalAMR;

            //    var dataquery = clsMeters.FetchConsumption_POC();
            //    var totalRAMR = dataquery.Where(x => x.Street.ToLower() == "khurja" && x.DailyConsumption > 0).Select(y => y.BusinessPartnerNo).Distinct().Count();
            //    ViewBag.TotalRAMR = totalRAMR;

            //    ViewBag.Nosales = totalAMR - totalRAMR;
            //    var vayudut = clsMeters.tbl_Consumption
            //           .Select(x => x.Vayudut_ID)
            //           .Distinct().Count();
            //    ViewBag.totalVayudut = vayudut;
            //    var AMRWithSales = clsMeters.sp_ConsumptionGraph().Count();
            //    ViewBag.AMRWithSales = AMRWithSales;

            //    var totalZeroCinsumption = totalAMR - AMRWithSales;
            //    ViewBag.totalZeroCinsumption = totalZeroCinsumption;

            //    var totalRevenue = clsMeters.sp_TotalRevenue().FirstOrDefault();
            //    ViewBag.Revenue = totalRevenue.Revenue;

            //    var pendingAMR = clsMeters.tbl_CustomerRegistration.Where(x => x.Status == "Pending").Count();
            //    ViewBag.PendingAMR = pendingAMR;

            //    var current = DateTime.Now.AddMonths(-1).Date;
            //    var tc = clsMeters.FetchConsumption_POC().Where(x => x.DailyConsumption >= 0 && x.Street.ToLower() == "khurja" && x.DailyConsumption < 1000 && x.Date>=current && x.Date<= DateTime.Now.Date).Sum(x => x.DailyConsumption);
            //    NumberFormatInfo setPrecision = new NumberFormatInfo();
            //    setPrecision.NumberDecimalDigits = 2;
            //    decimal test = Convert.ToDecimal(tc);
            //    ViewBag.TotalConsumption = test.ToString("N", setPrecision);

            //    //var Months = clsMeters.sp_GenerateInvoice().Select(x => x.MonthOfSales).Distinct().ToList();
            //    //for (int i = 0; i < Months.Count; i++)
            //    //{
            //    //    s[i] = Months[i].ToString();
            //    //    lstMonth.Add(Months[i].ToString());
            //    //    var countZeroSales = clsMeters.sp_GenerateInvoice().Where(x => x.GasSales == 0 && x.MonthOfSales == Months[i].ToString()).Count();
            //    //    var countSales = clsMeters.sp_GenerateInvoice().Where(x => x.GasSales != 0 && x.MonthOfSales == Months[i].ToString()).Count();
            //    //    lstZero.Add(countZeroSales);
            //    //    lstConsump.Add(countSales);
            //    //}
            //    ViewBag.ZeroArray = lstZero.ToArray();
            //    ViewBag.IntArray = lstConsump.ToArray();
            //    ViewBag.MonthArray = lstMonth.ToArray();
            return View();
            //}
            //else
            //{
            //    return RedirectToAction("../../Login");
            //}
        }
    }
}