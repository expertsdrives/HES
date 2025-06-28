using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HESMDMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace HESMDMS.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        // Recommended: Add database indexes on tbl_CustomerRegistration.BusinessPartnerNo,
        // tbl_Consumption.BusinessPartnerNo, tbl_Customers.BusinessPatner, and tbl_CustomerRegistration.InstallationDate
        // to improve query performance.
        public async Task<ActionResult> CustomerDataAnalysis(DateTime? startDate, DateTime? endDate)
        {
            using (var db = new SmartMeterEntities())
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    endDate = DateTime.Now;
                    startDate = endDate.Value.AddDays(-30);
                }

                var query = from reg in db.tbl_CustomerRegistration
                            join cons in db.tbl_Consumption on reg.BusinessPartnerNo equals cons.BusinessPartnerNo
                            join cust in db.tbl_Customers on reg.BusinessPartnerNo.ToString() equals cust.BusinessPatner
                            where reg.InstallationDate >= startDate && reg.InstallationDate <= endDate
                            select new CustomerDataViewModel
                            {
                                FullName = reg.FullName,
                                MeterNumber = reg.MeterNumber,
                                AMRReading = cons.OpeningAMRReading,
                                InstallationDate = reg.InstallationDate,
                                City = cust.City,
                                ChargeAreaZone = cust.RegStGrp
                            };

                var customerData = await query.ToListAsync();
                return View(customerData);
            }
        }

        public async Task<ActionResult> AdvancedDataAnalysis(DateTime? startDate, DateTime? endDate)
        {
            using (var db = new SmartMeterEntities())
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    endDate = DateTime.Now;
                    startDate = endDate.Value.AddDays(-30);
                }

                var query = from reg in db.tbl_CustomerRegistration
                            join cons in db.tbl_Consumption on reg.BusinessPartnerNo equals cons.BusinessPartnerNo
                            join cust in db.tbl_Customers on reg.BusinessPartnerNo.ToString() equals cust.BusinessPatner
                             where reg.InstallationDate >= startDate && reg.InstallationDate <= endDate
                            select new CustomerDataViewModel
                            {
                                FullName = reg.FullName,
                                MeterNumber = reg.MeterNumber,
                                AMRReading = cons.OpeningAMRReading,
                                InstallationDate = reg.InstallationDate,
                                City = cust.City,
                                ChargeAreaZone = cust.RegStGrp
                            };

                var customerData = await query.ToListAsync();
                var customersByCity = customerData.GroupBy(c => c.City)
                                                  .ToDictionary(g => g.Key, g => g.Count());

                var viewModel = new AdvancedDataAnalysisViewModel
                {
                    CustomerData = customerData,
                    CustomersByCity = customersByCity
                };

                return View(viewModel);
            }
        }
    }

    public class CustomerDataViewModel
    {
        public string FullName { get; set; }
        public string MeterNumber { get; set; }
        public decimal? AMRReading { get; set; }
        public System.DateTime? InstallationDate { get; set; }
        public string City { get; set; }
        public string ChargeAreaZone { get; set; }
    }

    public class AdvancedDataAnalysisViewModel
    {
        public List<CustomerDataViewModel> CustomerData { get; set; }
        public Dictionary<string, int> CustomersByCity { get; set; }
    }
}