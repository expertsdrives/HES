using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using HESMDMS.Areas.Admin.Data;
using HESMDMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HESMDMS.Hubs;
using Microsoft.AspNet.SignalR;

namespace HESMDMS.Areas.Admin.Controllers
{

    public class BillingController : Controller
    {
        private readonly SmartMeterEntities _clsDb;
        SmartMeter_ProdEntities _clsDb1 = new SmartMeter_ProdEntities();
     

        public BillingController(SmartMeterEntities clsDb)
        {
            _clsDb = clsDb;
        }

        public BillingController() : this(new SmartMeterEntities())
        {
            
        }

        // GET: Admin/Billing
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetBillingData(DataSourceLoadOptions loadOptions)
        {
            var billingAndCustomerData = (from billing in _clsDb.tbl_BillingAckData
                                          join customer in _clsDb.tbl_Customers on billing.BusinessPartner equals customer.BusinessPatner
                                          select new { billing, customer }).ToList();

            var assignAndMeterData = (from assign in _clsDb1.tbl_AssignSmartMeter
                                      join meter in _clsDb1.tbl_SMeterMaster on assign.pld equals meter.PLD
                                      select new { assign, meter }).ToList();

            var source = from bc in billingAndCustomerData
                         join am in assignAndMeterData on bc.customer.ID equals am.assign.CustomerInstallationID
                         select new BillingViewModel
                         {
                             ID = bc.billing.ID,
                             BusinessPartner = bc.billing.BusinessPartner,
                             Installation = bc.billing.Installation,
                             Date = bc.billing.Date,
                             Material = bc.billing.Material,
                             Meter_SerialNo = bc.billing.Meter_SerialNo,
                             MessageType = bc.billing.MessageType,
                             MessageID = bc.billing.MessageID,
                             MessageDescription = bc.billing.MessageDescription,
                             CustomerID = bc.customer.ContractAcct,
                             MeterSerialNumber = am.meter.MeterSerialNumber
                         };

            if (loadOptions.Sort == null || loadOptions.Sort.Length == 0) {
                loadOptions.Sort = new[] {
                    new SortingInfo { Selector = "Date", Desc = true }
                };
            }

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpPost]
        public void NotifyNewBillingData()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<GridHub>();
            hubContext.Clients.All.updateData();
        }
    }
}