using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HESMDMS.Models;
namespace HESMDMS.Controllers
{
    public class MobileAppController : ApiController
    {
        SmartMeterEntities clsDB = new SmartMeterEntities();

        [HttpGet]
        [Route("LoginAPI")]
        public HttpResponseMessage Login(string username, string password)
        {
            var loginCred = clsDB.tbl_AdminCredentials.Where(c => c.Username == username && c.Password == password).Count();
            if (loginCred > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, loginCred);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, loginCred);
            }
            //return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerData")]
        public HttpResponseMessage GetCustomerData()
        {
            var loginCred = clsDB.FetchConsumption().OrderByDescending(x => x.Date).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetails")]
        public HttpResponseMessage GetCustomerDetails()
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdoo().OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetails1")]
        public HttpResponseMessage GetCustomerDetails1()
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdoo().Where(x=>x.MonthOfSales== "July" || x.MonthOfSales == "August").OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [Route("GetCustomerDetailsDateWise")]
        public HttpResponseMessage GetCustomerDetailsDateWise(DateTime startdate, DateTime enddate)
        {
            var loginCred = clsDB.sp_GenerateInvoiceForOdooDateWise(startdate, enddate).OrderBy(x => x.FullName).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, loginCred, Configuration.Formatters.JsonFormatter);
        }
    }
}
