using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using EmailLib;
using HESMDMS.Models;
using IronPdf;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using static HESMDMS.Controllers.SAPIntegrationController;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace HESMDMS.Controllers
{
    [BasicAuthentication(Realm = "YourRealm")]
    public class SAPIntegrationController : ApiController
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeterEntities1 clsMeters1 = new SmartMeterEntities1();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        [System.Web.Http.Route("CustomerRegistrationStage")]
        [System.Web.Http.HttpPost]

        public HttpResponseMessage CustomerRegistrationStage([FromBody] JObject jsonData)
        {
            var billingData = jsonData["Customer_DeltaData"]["Customer_DeltaData_Record"];
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var buss = (string)rec["BusinessPartner"];
                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = clsMeters.tbl_Customers.Where(x => x.BusinessPatner == buss).FirstOrDefault();

                    if (existingCustomer != null)
                    {

                        // Update existing customer data
                        existingCustomer.BusinessPatner = (string)rec["BusinessPartner"];
                        existingCustomer.ContractAcct = (string)rec["ContractAcct"];
                        existingCustomer.Contract = (string)rec["Contract"];
                        existingCustomer.FullName = (string)rec["FullName"];
                        existingCustomer.FirstName = (string)rec["FirstName"];
                        existingCustomer.LastName = (string)rec["LastName"];
                        existingCustomer.HouseNo = (string)rec["HouseNo"];
                        existingCustomer.Street = (string)rec["Street"];
                        existingCustomer.Street2 = (string)rec["Street2"];
                        existingCustomer.Street3 = (string)rec["Street3"];
                        existingCustomer.Street4 = (string)rec["Street4"];
                        existingCustomer.Street5 = (string)rec["Street5"];
                        existingCustomer.City = (string)rec["City"];
                        existingCustomer.PostalCode = (string)rec["PostalCode"];
                        existingCustomer.RegStGrp = (string)rec["RegStGrp"];
                        existingCustomer.ConnectionObject = (string)rec["ConnectionObject"];
                        var installationate1 = (string)rec["Installation"];
                        existingCustomer.Installation = installationate1;
                        existingCustomer.Premise = (string)rec["Premise"];
                        existingCustomer.DeviceLocation = (string)rec["DeviceLocation"];
                        existingCustomer.MeterType = (string)rec["MeterType"];
                        existingCustomer.MeterNumber = (string)rec["MeterNumber"];
                        existingCustomer.ManufacturerMeterNumber = (string)rec["Manufacturer"];
                        var createdate = (string)rec["BPCreationDate"] == "00000000" ? "19000101" : (string)rec["BPCreationDate"];
                        existingCustomer.CreationDate = DateTime.ParseExact(createdate, "yyyyMMdd", null);
                        var MoveInDate = (string)rec["MoveInDate"] == "00000000" ? "19000101" : (string)rec["MoveInDate"];
                        existingCustomer.MoveInDate = DateTime.ParseExact(MoveInDate, "yyyyMMdd", null);

                        var MoveOutDate = (string)rec["MoveOutDate"] == "00000000" ? "19000101" : (string)rec["MoveOutDate"];
                        existingCustomer.MoveOutDate = DateTime.ParseExact(MoveOutDate, "yyyyMMdd", null);

                        var MoveInCreatedOn = (string)rec["MoveInCreatedOn"] == "00000000" ? "19000101" : (string)rec["MoveInCreatedOn"];
                        existingCustomer.MoveInCreatedOn = DateTime.ParseExact(MoveInCreatedOn, "yyyyMMdd", null);

                        var BPReleaseDate = (string)rec["BPReleaseDate"] == "00000000" ? "19000101" : (string)rec["BPReleaseDate"];
                        existingCustomer.BPReleaseDate = DateTime.ParseExact(BPReleaseDate, "yyyyMMdd", null);

                        var DisconnectionDate = (string)rec["DisconnectionDate"] == "00000000" ? "19000101" : (string)rec["DisconnectionDate"];
                        existingCustomer.DisconnectionDate = DateTime.ParseExact(DisconnectionDate, "yyyyMMdd", null);

                        existingCustomer.DisconnectionType = (string)rec["DisconnectionType"];

                        existingCustomer.ServiceRegulator = (string)rec["ServiceRegulator"];

                        existingCustomer.DRS = (string)rec["DRS"];
                        existingCustomer.BPType = (string)rec["BPType"];
                        existingCustomer.IDType = (string)rec["IDType"];
                        existingCustomer.Plan = (string)rec["Plan"];
                        existingCustomer.MobilNumber = (string)rec["MobilNumber"];
                        existingCustomer.MobilNumber2 = (string)rec["MobilNumber2"];
                        existingCustomer.MobilNumber3 = (string)rec["MobilNumber3"];
                        existingCustomer.MobilNumber4 = (string)rec["MobilNumber4"];
                        existingCustomer.MobilNumber5 = (string)rec["MobilNumber5"];
                        existingCustomer.EmailID = (string)rec["EmailID"];
                        existingCustomer.EmailID2 = (string)rec["EmailID2"];
                        existingCustomer.EmailID3 = (string)rec["EmailID3"];
                        existingCustomer.EmailID4 = (string)rec["EmailID4"];
                        existingCustomer.EmailID5 = (string)rec["EmailID5"];
                        existingCustomer.Plant = (string)rec["Plant"];
                        existingCustomer.MeterReadingUnit = (string)rec["MeterReadingUnit"];
                        existingCustomer.BPSubType = (string)rec["BPSubType"];
                        existingCustomer.ScheduleDate = (string)rec["ScheduleDate"];
                        existingCustomer.RentalFlag = (string)rec["RentalFlag"];
                        existingCustomer.Installment_Amount = (string)rec["Installment_Amount"];
                        existingCustomer.Installment_Numbers = (string)rec["Installment_Numbers"];
                        existingCustomer.Discount_Amount = (string)rec["Discount_Amount"];
                        existingCustomer.Valid_from = (string)rec["Valid_from"];
                        existingCustomer.valid_to = (string)rec["Valid_to"];
                        // Date in UTC
                        DateTime utcDate = DateTime.UtcNow;

                        // Time zone info for IST
                        TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                        // Convert UTC date to IST
                        DateTime istDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, istTimeZone);
                        existingCustomer.LastUpdatedDate = istDate;
                        clsMeters.Entry(existingCustomer).State = EntityState.Modified;
                    }
                    else
                    {
                        var existingCustomer1 = new tbl_Customers();
                        existingCustomer1.BusinessPatner = (string)rec["BusinessPartner"];
                        existingCustomer1.ContractAcct = (string)rec["ContractAcct"];
                        existingCustomer1.Contract = (string)rec["Contract"];
                        existingCustomer1.FullName = (string)rec["FullName"];
                        existingCustomer1.FirstName = (string)rec["FirstName"];
                        existingCustomer1.LastName = (string)rec["LastName"];
                        existingCustomer1.HouseNo = (string)rec["HouseNo"];
                        existingCustomer1.Street = (string)rec["Street"];
                        existingCustomer1.Street2 = (string)rec["Street2"];
                        existingCustomer1.Street3 = (string)rec["Street3"];
                        existingCustomer1.Street4 = (string)rec["Street4"];
                        existingCustomer1.Street5 = (string)rec["Street5"];
                        existingCustomer1.City = (string)rec["City"];
                        existingCustomer1.PostalCode = (string)rec["PostalCode"];
                        existingCustomer1.RegStGrp = (string)rec["RegStGrp"];
                        existingCustomer1.ConnectionObject = (string)rec["ConnectionObject"];
                        var installationdate = (string)rec["Installation"];
                        existingCustomer1.Installation = installationdate;
                        existingCustomer1.Premise = (string)rec["Premise"];
                        existingCustomer1.DeviceLocation = (string)rec["DeviceLocation"];
                        existingCustomer1.MeterType = (string)rec["MeterType"];
                        existingCustomer1.MeterNumber = (string)rec["MeterNumber"];
                        existingCustomer1.ManufacturerMeterNumber = (string)rec["Manufacturer"];
                        var createdate = (string)rec["BPCreationDate"] == "00000000" ? "19000101" : (string)rec["BPCreationDate"];
                        existingCustomer1.CreationDate = DateTime.ParseExact(createdate, "yyyyMMdd", null);
                        var MoveInDate = (string)rec["MoveInDate"] == "00000000" ? "19000101" : (string)rec["MoveInDate"];
                        existingCustomer1.MoveInDate = DateTime.ParseExact(MoveInDate, "yyyyMMdd", null);

                        var MoveOutDate = (string)rec["MoveOutDate"] == "00000000" ? "19000101" : (string)rec["MoveOutDate"];
                        existingCustomer1.MoveOutDate = DateTime.ParseExact(MoveOutDate, "yyyyMMdd", null);

                        var MoveInCreatedOn = (string)rec["MoveInCreatedOn"] == "00000000" ? "19000101" : (string)rec["MoveInCreatedOn"];
                        existingCustomer1.MoveInCreatedOn = DateTime.ParseExact(MoveInCreatedOn, "yyyyMMdd", null);

                        var BPReleaseDate = (string)rec["BPReleaseDate"] == "00000000" ? "19000101" : (string)rec["BPReleaseDate"];
                        existingCustomer1.BPReleaseDate = DateTime.ParseExact(BPReleaseDate, "yyyyMMdd", null);

                        var DisconnectionDate = (string)rec["DisconnectionDate"] == "00000000" ? "19000101" : (string)rec["DisconnectionDate"];
                        existingCustomer1.DisconnectionDate = DateTime.ParseExact(DisconnectionDate, "yyyyMMdd", null);

                        existingCustomer1.DisconnectionType = (string)rec["DisconnectionType"];

                        existingCustomer1.ServiceRegulator = (string)rec["ServiceRegulator"];

                        existingCustomer1.DRS = (string)rec["DRS"];
                        existingCustomer1.BPType = (string)rec["BPType"];
                        existingCustomer1.IDType = (string)rec["IDType"];
                        existingCustomer1.Plan = (string)rec["Plan"];
                        existingCustomer1.MobilNumber = (string)rec["MobilNumber"];
                        existingCustomer1.MobilNumber2 = (string)rec["MobilNumber2"];
                        existingCustomer1.MobilNumber3 = (string)rec["MobilNumber3"];
                        existingCustomer1.MobilNumber4 = (string)rec["MobilNumber4"];
                        existingCustomer1.MobilNumber5 = (string)rec["MobilNumber5"];
                        existingCustomer1.EmailID = (string)rec["EmailID"];
                        existingCustomer1.EmailID2 = (string)rec["EmailID2"];
                        existingCustomer1.EmailID3 = (string)rec["EmailID3"];
                        existingCustomer1.EmailID4 = (string)rec["EmailID4"];
                        existingCustomer1.EmailID5 = (string)rec["EmailID5"];
                        existingCustomer1.Plant = (string)rec["Plant"];
                        existingCustomer1.MeterReadingUnit = (string)rec["MeterReadingUnit"];
                        existingCustomer1.BPSubType = (string)rec["BPSubType"];
                        existingCustomer1.Installment_Amount = (string)rec["Installment_Amount"];
                        existingCustomer1.Installment_Numbers = (string)rec["Installment_Numbers"];
                        existingCustomer1.Discount_Amount = (string)rec["Discount_Amount"];
                        existingCustomer1.Valid_from = (string)rec["Valid_from"];
                        existingCustomer1.valid_to = (string)rec["Valid_to"];

                        DateTime utcDate1 = DateTime.UtcNow;
                        TimeZoneInfo istTimeZone1 = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                        // Convert UTC date to IST
                        DateTime istDate1 = TimeZoneInfo.ConvertTimeFromUtc(utcDate1, istTimeZone1);

                        existingCustomer1.CreatedDate = istDate1;
                        existingCustomer1.ScheduleDate = (string)rec["ScheduleDate"];
                        existingCustomer1.RentalFlag = (string)rec["RentalFlag"];
                        // Add new customer
                        clsMeters.tbl_Customers.Add(existingCustomer1);
                    }

                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Customer data processed successfully");
        }
        [System.Web.Http.Route("CustomerRegistration")]
        [System.Web.Http.HttpPost]

        public HttpResponseMessage CustomerRegistration([FromBody] JObject jsonData)
        {
            var billingData = jsonData["Customer_DeltaData"]["Customer_DeltaData_Record"];
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var buss = (string)rec["BusinessPartner"];
                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = clsMeters.tbl_Customers.Where(x => x.BusinessPatner == buss).FirstOrDefault();

                    if (existingCustomer != null)
                    {

                        // Update existing customer data
                        existingCustomer.BusinessPatner = (string)rec["BusinessPartner"];
                        existingCustomer.ContractAcct = (string)rec["ContractAcct"];
                        existingCustomer.Contract = (string)rec["Contract"];
                        existingCustomer.FullName = (string)rec["FullName"];
                        existingCustomer.FirstName = (string)rec["FirstName"];
                        existingCustomer.LastName = (string)rec["LastName"];
                        existingCustomer.HouseNo = (string)rec["HouseNo"];
                        existingCustomer.Street = (string)rec["Street"];
                        existingCustomer.Street2 = (string)rec["Street2"];
                        existingCustomer.Street3 = (string)rec["Street3"];
                        existingCustomer.Street4 = (string)rec["Street4"];
                        existingCustomer.Street5 = (string)rec["Street5"];
                        existingCustomer.City = (string)rec["City"];
                        existingCustomer.PostalCode = (string)rec["PostalCode"];
                        existingCustomer.RegStGrp = (string)rec["RegStGrp"];
                        existingCustomer.ConnectionObject = (string)rec["ConnectionObject"];
                        var installationate1 = (string)rec["Installation"];
                        existingCustomer.Installation = installationate1;
                        existingCustomer.Premise = (string)rec["Premise"];
                        existingCustomer.DeviceLocation = (string)rec["DeviceLocation"];
                        existingCustomer.MeterType = (string)rec["MeterType"];
                        existingCustomer.MeterNumber = (string)rec["MeterNumber"];
                        existingCustomer.ManufacturerMeterNumber = (string)rec["Manufacturer"];
                        var createdate = (string)rec["BPCreationDate"] == "00000000" ? "19000101" : (string)rec["BPCreationDate"];
                        existingCustomer.CreationDate = DateTime.ParseExact(createdate, "yyyyMMdd", null);
                        var MoveInDate = (string)rec["MoveInDate"] == "00000000" ? "19000101" : (string)rec["MoveInDate"];
                        existingCustomer.MoveInDate = DateTime.ParseExact(MoveInDate, "yyyyMMdd", null);

                        var MoveOutDate = (string)rec["MoveOutDate"] == "00000000" ? "19000101" : (string)rec["MoveOutDate"];
                        existingCustomer.MoveOutDate = DateTime.ParseExact(MoveOutDate, "yyyyMMdd", null);

                        var MoveInCreatedOn = (string)rec["MoveInCreatedOn"] == "00000000" ? "19000101" : (string)rec["MoveInCreatedOn"];
                        existingCustomer.MoveInCreatedOn = DateTime.ParseExact(MoveInCreatedOn, "yyyyMMdd", null);

                        var BPReleaseDate = (string)rec["BPReleaseDate"] == "00000000" ? "19000101" : (string)rec["BPReleaseDate"];
                        existingCustomer.BPReleaseDate = DateTime.ParseExact(BPReleaseDate, "yyyyMMdd", null);

                        var DisconnectionDate = (string)rec["DisconnectionDate"] == "00000000" ? "19000101" : (string)rec["DisconnectionDate"];
                        existingCustomer.DisconnectionDate = DateTime.ParseExact(DisconnectionDate, "yyyyMMdd", null);

                        existingCustomer.DisconnectionType = (string)rec["DisconnectionType"];

                        existingCustomer.ServiceRegulator = (string)rec["ServiceRegulator"];

                        existingCustomer.DRS = (string)rec["DRS"];
                        existingCustomer.BPType = (string)rec["BPType"];
                        existingCustomer.IDType = (string)rec["IDType"];
                        existingCustomer.Plan = (string)rec["Plan"];
                        existingCustomer.MobilNumber = (string)rec["MobilNumber"];
                        existingCustomer.MobilNumber2 = (string)rec["MobilNumber2"];
                        existingCustomer.MobilNumber3 = (string)rec["MobilNumber3"];
                        existingCustomer.MobilNumber4 = (string)rec["MobilNumber4"];
                        existingCustomer.MobilNumber5 = (string)rec["MobilNumber5"];
                        existingCustomer.EmailID = (string)rec["EmailID"];
                        existingCustomer.EmailID2 = (string)rec["EmailID2"];
                        existingCustomer.EmailID3 = (string)rec["EmailID3"];
                        existingCustomer.EmailID4 = (string)rec["EmailID4"];
                        existingCustomer.EmailID5 = (string)rec["EmailID5"];
                        existingCustomer.Plant = (string)rec["Plant"];
                        existingCustomer.MeterReadingUnit = (string)rec["MeterReadingUnit"];
                        existingCustomer.BPSubType = (string)rec["BPSubType"];
                        existingCustomer.ScheduleDate = (string)rec["ScheduleDate"];
                        existingCustomer.RentalFlag = (string)rec["RentalFlag"];
                        existingCustomer.Installment_Amount = (string)rec["Installment_Amount"];
                        existingCustomer.Installment_Numbers = (string)rec["Installment_Numbers"];
                        existingCustomer.Discount_Amount = (string)rec["Discount_Amount"];
                        existingCustomer.Valid_from = (string)rec["Valid_from"];
                        existingCustomer.valid_to = (string)rec["Valid_to"];
                        // Date in UTC
                        DateTime utcDate = DateTime.UtcNow;

                        // Time zone info for IST
                        TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                        // Convert UTC date to IST
                        DateTime istDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, istTimeZone);
                        existingCustomer.LastUpdatedDate = istDate;
                        clsMeters.Entry(existingCustomer).State = EntityState.Modified;
                    }
                    else
                    {
                        var existingCustomer1 = new tbl_Customers();
                        existingCustomer1.BusinessPatner = (string)rec["BusinessPartner"];
                        existingCustomer1.ContractAcct = (string)rec["ContractAcct"];
                        existingCustomer1.Contract = (string)rec["Contract"];
                        existingCustomer1.FullName = (string)rec["FullName"];
                        existingCustomer1.FirstName = (string)rec["FirstName"];
                        existingCustomer1.LastName = (string)rec["LastName"];
                        existingCustomer1.HouseNo = (string)rec["HouseNo"];
                        existingCustomer1.Street = (string)rec["Street"];
                        existingCustomer1.Street2 = (string)rec["Street2"];
                        existingCustomer1.Street3 = (string)rec["Street3"];
                        existingCustomer1.Street4 = (string)rec["Street4"];
                        existingCustomer1.Street5 = (string)rec["Street5"];
                        existingCustomer1.City = (string)rec["City"];
                        existingCustomer1.PostalCode = (string)rec["PostalCode"];
                        existingCustomer1.RegStGrp = (string)rec["RegStGrp"];
                        existingCustomer1.ConnectionObject = (string)rec["ConnectionObject"];
                        var installationdate = (string)rec["Installation"];
                        existingCustomer1.Installation = installationdate;
                        existingCustomer1.Premise = (string)rec["Premise"];
                        existingCustomer1.DeviceLocation = (string)rec["DeviceLocation"];
                        existingCustomer1.MeterType = (string)rec["MeterType"];
                        existingCustomer1.MeterNumber = (string)rec["MeterNumber"];
                        existingCustomer1.ManufacturerMeterNumber = (string)rec["Manufacturer"];
                        var createdate = (string)rec["BPCreationDate"] == "00000000" ? "19000101" : (string)rec["BPCreationDate"];
                        existingCustomer1.CreationDate = DateTime.ParseExact(createdate, "yyyyMMdd", null);
                        var MoveInDate = (string)rec["MoveInDate"] == "00000000" ? "19000101" : (string)rec["MoveInDate"];
                        existingCustomer1.MoveInDate = DateTime.ParseExact(MoveInDate, "yyyyMMdd", null);

                        var MoveOutDate = (string)rec["MoveOutDate"] == "00000000" ? "19000101" : (string)rec["MoveOutDate"];
                        existingCustomer1.MoveOutDate = DateTime.ParseExact(MoveOutDate, "yyyyMMdd", null);

                        var MoveInCreatedOn = (string)rec["MoveInCreatedOn"] == "00000000" ? "19000101" : (string)rec["MoveInCreatedOn"];
                        existingCustomer1.MoveInCreatedOn = DateTime.ParseExact(MoveInCreatedOn, "yyyyMMdd", null);

                        var BPReleaseDate = (string)rec["BPReleaseDate"] == "00000000" ? "19000101" : (string)rec["BPReleaseDate"];
                        existingCustomer1.BPReleaseDate = DateTime.ParseExact(BPReleaseDate, "yyyyMMdd", null);

                        var DisconnectionDate = (string)rec["DisconnectionDate"] == "00000000" ? "19000101" : (string)rec["DisconnectionDate"];
                        existingCustomer1.DisconnectionDate = DateTime.ParseExact(DisconnectionDate, "yyyyMMdd", null);

                        existingCustomer1.DisconnectionType = (string)rec["DisconnectionType"];

                        existingCustomer1.ServiceRegulator = (string)rec["ServiceRegulator"];

                        existingCustomer1.DRS = (string)rec["DRS"];
                        existingCustomer1.BPType = (string)rec["BPType"];
                        existingCustomer1.IDType = (string)rec["IDType"];
                        existingCustomer1.Plan = (string)rec["Plan"];
                        existingCustomer1.MobilNumber = (string)rec["MobilNumber"];
                        existingCustomer1.MobilNumber2 = (string)rec["MobilNumber2"];
                        existingCustomer1.MobilNumber3 = (string)rec["MobilNumber3"];
                        existingCustomer1.MobilNumber4 = (string)rec["MobilNumber4"];
                        existingCustomer1.MobilNumber5 = (string)rec["MobilNumber5"];
                        existingCustomer1.EmailID = (string)rec["EmailID"];
                        existingCustomer1.EmailID2 = (string)rec["EmailID2"];
                        existingCustomer1.EmailID3 = (string)rec["EmailID3"];
                        existingCustomer1.EmailID4 = (string)rec["EmailID4"];
                        existingCustomer1.EmailID5 = (string)rec["EmailID5"];
                        existingCustomer1.Plant = (string)rec["Plant"];
                        existingCustomer1.MeterReadingUnit = (string)rec["MeterReadingUnit"];
                        existingCustomer1.BPSubType = (string)rec["BPSubType"];
                        existingCustomer1.Installment_Amount = (string)rec["Installment_Amount"];
                        existingCustomer1.Installment_Numbers = (string)rec["Installment_Numbers"];
                        existingCustomer1.Discount_Amount = (string)rec["Discount_Amount"];
                        existingCustomer1.Valid_from = (string)rec["Valid_from"];
                        existingCustomer1.valid_to = (string)rec["Valid_to"];

                        DateTime utcDate1 = DateTime.UtcNow;
                        TimeZoneInfo istTimeZone1 = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                        // Convert UTC date to IST
                        DateTime istDate1 = TimeZoneInfo.ConvertTimeFromUtc(utcDate1, istTimeZone1);

                        existingCustomer1.CreatedDate = istDate1;
                        existingCustomer1.ScheduleDate = (string)rec["ScheduleDate"];
                        existingCustomer1.RentalFlag = (string)rec["RentalFlag"];
                        // Add new customer
                        clsMeters.tbl_Customers.Add(existingCustomer1);
                    }

                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Customer data processed successfully");
        }
        public class Customer_DeltaData_Record
        {
            public Nullable<int> BusinessPatner { get; set; }
            public Nullable<int> ContractAcct { get; set; }
            public Nullable<int> Contract { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string HouseNo { get; set; }
            public string Street { get; set; }
            public string Street2 { get; set; }
            public string Street3 { get; set; }
            public string Street4 { get; set; }
            public string Street5 { get; set; }
            public string City { get; set; }
            public Nullable<int> PostalCode { get; set; }
            public string RegStGrp { get; set; }
            public string ConnectionObject { get; set; }
            public Nullable<System.DateTime> Installation { get; set; }
            public Nullable<int> Premise { get; set; }
            public string DeviceLocation { get; set; }
            public string MeterType { get; set; }
            public string MeterNumber { get; set; }
            public string ManufacturerMeterNumber { get; set; }
            public Nullable<System.DateTime> CreationDate { get; set; }
            public Nullable<System.DateTime> MoveInDate { get; set; }
            public Nullable<System.DateTime> MoveOutDate { get; set; }
            public Nullable<System.DateTime> MoveInCreatedOn { get; set; }
            public Nullable<System.DateTime> BPReleaseDate { get; set; }
            public Nullable<System.DateTime> DisconnectionDate { get; set; }
            public string DisconnectionType { get; set; }
            public Nullable<int> ServiceRegulator { get; set; }
            public string DRS { get; set; }
            public string BPType { get; set; }
            public string IDType { get; set; }
            public string Plan { get; set; }
            public string MobilNumber { get; set; }
            public string EmailID { get; set; }
            public string Plant { get; set; }
            public string MeterReadingUnit { get; set; }
            public string BPSubType { get; set; }
            public string MobilNumber2 { get; set; }
            public string MobilNumber3 { get; set; }
            public string MobilNumber4 { get; set; }
            public string MobilNumber5 { get; set; }
            public string EmailID2 { get; set; }
            public string EmailID3 { get; set; }
            public string EmailID4 { get; set; }
            public string EmailID5 { get; set; }
            public string ScheduleDate { get; set; }
            public string RentalFlag { get; set; }
        }
        public class BillingAckData
        {
            public int BusinessPartner { get; set; }
            public int Installation { get; set; }
            public DateTime Date { get; set; }
            public string MessageType { get; set; }
            public string MessageDiscription { get; set; }
            public string Material { get; set; }
            public string MessageDescription { get; set; }
            public long MeterSerialNo { get; set; }
        }
        public class FetchBalance
        {
            public string CustomerID { get; set; }

        }

        public class AMRAck_Data
        {
            public int Installation { get; set; }
            public int Date { get; set; }

            public string Material { get; set; }
            public string Message_Type { get; set; }
            public string Message_ID { get; set; }
            public string MessageDescription { get; set; }
            public long MeterSerialNo { get; set; }
        }

        public class AMR_Data_Acknowledgement
        {
            public List<AMRAck_Data> AMRAck_Data { get; set; }
        }
        public class BillingDataAcknowledgement
        {
            public List<BillingAckData> BillingAckData { get; set; }
        }
        public class Customer_DeltaData
        {
            public List<Customer_DeltaData_Record> Customer_DeltaData_Record { get; set; }
        }
        [System.Web.Http.Route("AMRAckStage")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AMRAckStage([FromBody] JObject jsonData)
        {
            var billingData = jsonData["AMR_Data_Acknowledgement"]["AMRAck_Data"];
            //var data = JsonConvert.DeserializeObject<BillingDataAcknowledgement>(clsBillingACK.ToString());
            //return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var Installation = (int)rec["Installation"];
                    var Date = (int)rec["Date"];
                    var Material = (string)rec["Material"];

                    var Meter_Serial_Number = (string)rec["Meter_Serial_Number"];
                    var Message_Type = (string)rec["Message_Type"];
                    var Message_ID = (string)rec["Message_ID"];
                    var Message_Description = (string)rec["Message_Description"];

                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = new tbl_AMRAckData();



                    // Update existing customer data
                    existingCustomer.Installation = Installation;
                    existingCustomer.Date = Date;
                    existingCustomer.Material = Material;
                    existingCustomer.Meter_Serial_Number = Meter_Serial_Number;
                    existingCustomer.Message_Type = Message_Type;
                    existingCustomer.Message_ID = Message_ID;
                    existingCustomer.Message_Description = Message_Description;



                    // Add new customer
                    clsMeters.tbl_AMRAckData.Add(existingCustomer);


                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
        }

        [System.Web.Http.Route("AMRAck")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AMRAck([FromBody] JObject jsonData)
        {
            var billingData = jsonData["AMR_Data_Acknowledgement"]["AMRAck_Data"];
            //var data = JsonConvert.DeserializeObject<BillingDataAcknowledgement>(clsBillingACK.ToString());
            //return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var Installation = (int)rec["Installation"];
                    var Date = (int)rec["Date"];
                    var Material = (string)rec["Material"];

                    var Meter_Serial_Number = (string)rec["Meter_Serial_Number"];
                    var Message_Type = (string)rec["Message_Type"];
                    var Message_ID = (string)rec["Message_ID"];
                    var Message_Description = (string)rec["Message_Description"];

                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = new tbl_AMRAckData();



                    // Update existing customer data
                    existingCustomer.Installation = Installation;
                    existingCustomer.Date = Date;
                    existingCustomer.Material = Material;
                    existingCustomer.Meter_Serial_Number = Meter_Serial_Number;
                    existingCustomer.Message_Type = Message_Type;
                    existingCustomer.Message_ID = Message_ID;
                    existingCustomer.Message_Description = Message_Description;



                    // Add new customer
                    clsMeters.tbl_AMRAckData.Add(existingCustomer);


                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
        }





        [System.Web.Http.Route("BillingAckStage")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage BillingAckStage([FromBody] JObject jsonData)
        {
            var billingData = jsonData["Billng_Data_Acknowledgement"]["BillingAck_Data"];
            //var data = JsonConvert.DeserializeObject<BillingDataAcknowledgement>(clsBillingACK.ToString());
            //return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var businessPartner = (string)rec["BusinessPartner"];
                    var installation = (string)rec["Installation"];
                    var date = (string)rec["Date"];
                    var datetime = DateTime.ParseExact(date, "yyyyMMdd", null);
                    var messageType = (string)rec["MessageType"];
                    var messageDescription = (string)rec["Message_Description"];
                    var material = (string)rec["Material"];
                    var meterSerialNo = (string)rec["Meter_SerialNo"];
                    var MessageID = (string)rec["Meter_SerialNo"];
                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = new tbl_BillingAckData();



                    // Update existing customer data
                    existingCustomer.BusinessPartner = businessPartner;
                    existingCustomer.Installation = installation;
                    existingCustomer.Date = datetime;
                    existingCustomer.Material = material;
                    existingCustomer.Meter_SerialNo = meterSerialNo;
                    existingCustomer.MessageType = messageType;
                    existingCustomer.MessageID = "";
                    existingCustomer.MessageDescription = messageDescription;


                    // Add new customer
                    clsMeters.tbl_BillingAckData.Add(existingCustomer);


                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
        }


        [System.Web.Http.Route("BillingAck")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage BillingAck([FromBody] JObject jsonData)
        {
            var billingData = jsonData["Billng_Data_Acknowledgement"]["BillingAck_Data"];
            //var data = JsonConvert.DeserializeObject<BillingDataAcknowledgement>(clsBillingACK.ToString());
            //return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
            foreach (var rec in billingData)
            {
                if (billingData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data");
                }

                try
                {
                    var businessPartner = (string)rec["BusinessPartner"];
                    var installation = (string)rec["Installation"];
                    var date = (string)rec["Date"];
                    var datetime = DateTime.ParseExact(date, "yyyyMMdd", null);
                    var messageType = (string)rec["MessageType"];
                    var messageDescription = (string)rec["Message_Description"];
                    var material = (string)rec["Material"];
                    var meterSerialNo = (string)rec["Meter_SerialNo"];
                    var MessageID = (string)rec["Meter_SerialNo"];
                    // Check if the customer already exists based on a unique attribute, e.g., Email
                    var existingCustomer = new tbl_BillingAckData();



                    // Update existing customer data
                    existingCustomer.BusinessPartner = businessPartner;
                    existingCustomer.Installation = installation;
                    existingCustomer.Date = datetime;
                    existingCustomer.Material = material;
                    existingCustomer.Meter_SerialNo = meterSerialNo;
                    existingCustomer.MessageType = messageType;
                    existingCustomer.MessageID = "";
                    existingCustomer.MessageDescription = messageDescription;


                    // Add new customer
                    clsMeters.tbl_BillingAckData.Add(existingCustomer);


                    clsMeters.SaveChanges();

                }
                catch (Exception ex)
                {
                    // Log the exception
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "Billing Data Acknowledgement  successfully");
        }

        public static IEnumerable<string> SplitIntoChunks(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }
        unsafe string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }
        public class D
        {
            public string Cust_No { get; set; }
            public string Name { get; set; }
            public string Mobile_No { get; set; }
            public string Email_ID { get; set; }
            public string Bill_No { get; set; }
            public string Bill_Date { get; set; }
            public string Due_Date { get; set; }
            public string Amount { get; set; }
            public string Current_Outstanding_Amount { get; set; }
            public string Partner_Type { get; set; }
            public string ConnType { get; set; }
            public string Execution_time { get; set; }
            public string Msg_Flag { get; set; }
            public string Message { get; set; }
            public int time_diff { get; set; }
        }

        public class ApiResponse
        {
            public D d { get; set; }
        }
        [System.Web.Http.Route("ATGLPaymnetACK")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage ATGLPaymnetACK(tbl_ATGLPaymentAck tbl_ATGLPaymentAck)
        {
            try
            {

                var customerid = tbl_ATGLPaymentAck.CustomerID;
                var tid = clsMeters.tbl_ATGLPaymentAck.Where(x => x.TransactionID == tbl_ATGLPaymentAck.TransactionID).Count();
                if (tid != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Duplicate Transcation ID Not Allowed");
                }
                else
                {
                    tbl_ATGLPaymentAck.CreatedDate = DateTime.Now;
                    clsMeters.tbl_ATGLPaymentAck.Add(tbl_ATGLPaymentAck);
                    clsMeters.SaveChanges();
                    if (tbl_ATGLPaymentAck.Status == "success")
                    {
                        decimal amount = Convert.ToDecimal(tbl_ATGLPaymentAck.PaymentAmount);
                        var outstandingAmount1 = GetCurrentOutstandingAsync(customerid);
                        decimal outstandingAmount = Convert.ToDecimal(GetCurrentOutstandingAsync(customerid));
                        var diffAmount = outstandingAmount - amount;
                        //if (eventname == "Set Average Gas Calorific Value")
                        //{

                        //    balanceString = ConvertMsbToLsb(balanceString);
                        //}
                        var smartchck = clsMeters1.tbl_SGMReg.Where(x => x.CustomreID == customerid).FirstOrDefault();
                        smartchck = clsMeters1.tbl_SGMReg.Where(x => x.CustomreID == customerid).FirstOrDefault();
                        var getCumDetails = clsMeters.tbl_Customers.Where(x => x.ContractAcct == customerid).FirstOrDefault();
                        if (outstandingAmount < amount)
                        {

                            var difmount = amount - outstandingAmount;
                            if (smartchck != null)
                            {
                                var meterNumber1 = smartchck.SmartMeterSerialNumber;
                                var pldfind = clsMetersProd.tbl_SMeterMaster.Where(x => x.MeterSerialNumber == meterNumber1).FirstOrDefault();
                                var getAccountData = clsMetersProd.tbl_Response.Where(x => x.pld == pldfind.PLD && x.Data.StartsWith("$")).OrderByDescending(x => x.LogDate).FirstOrDefault();
                                var ecredit = "";

                                ecredit = getAccountData.Data.Split(',')[13].Trim().Replace("+", "");

                                decimal ecreditBalance = 0;

                                decimal ecreditdecimal = Convert.ToDecimal(ecredit);
                                if (ecreditdecimal < 250)
                                {
                                    ecreditBalance = difmount >= 250 ? 250 : difmount;
                                    amount = difmount - ecreditBalance;

                                    float eintValue = float.Parse(ecreditBalance.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    string ebalanceString = ToHexString(eintValue);
                                    var ebalanceutput = string.Join(",", SplitIntoChunks(ebalanceString, 2));
                                    string[] splitArray = ebalanceutput.Split(',');

                                    // Reverse the array
                                    Array.Reverse(splitArray);

                                    // Join the array elements back into a string using a comma as the separator
                                    ebalanceutput = string.Join(",", splitArray);
                                    var edata1 = "02,03,00,04,F1,-,03";
                                    string emodifiedString = edata1.Replace("-", ebalanceutput);
                                    var elength = ebalanceutput.Split(',').Length.ToString("D2");
                                    edata1 = emodifiedString;
                                    string[] evalues = edata1.Split(',');
                                    if (evalues.Length >= 3)
                                    {
                                        // Replace the third value with the new variable
                                        evalues[3] = elength;

                                        // Join the array back into a string with commas
                                        string resultString = string.Join(",", evalues);

                                        // Now, resultString will be "value1,value2,newvalue,value4,value5"

                                        // You can use the resultString in your view or further processing
                                        edata1 = resultString;
                                    }
                                    clsMetersProd.Database.ExecuteSqlCommand($"insert into tbl_CommandBackLog values ('{edata1}','Set E-Credit Threshold','Pending','{DateTime.Now}','{pldfind.PLD}',NULL)");
                                }
                                if (amount > 0)
                                {
                                    float intValue = float.Parse(amount.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                                    string balanceString = ToHexString(intValue);
                                    var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));

                                    var data1 = "02,03,00,03,29,-,03";
                                    string modifiedString = data1.Replace("-", balanceutput);
                                    var length = balanceutput.Split(',').Length.ToString("D2");
                                    data1 = modifiedString;
                                    string[] values = data1.Split(',');
                                    if (values.Length >= 3)
                                    {
                                        // Replace the third value with the new variable
                                        values[3] = length;

                                        // Join the array back into a string with commas
                                        string resultString = string.Join(",", values);

                                        // Now, resultString will be "value1,value2,newvalue,value4,value5"

                                        // You can use the resultString in your view or further processing
                                        data1 = resultString;
                                    }
                                    clsMetersProd.Database.ExecuteSqlCommand($"insert into tbl_CommandBackLog values ('{data1}','Add Balance','Pending','{DateTime.Now}','{pldfind.PLD}',NULL)");
                                }
                                //var cData1 = clsMetersProd.tbl_CommandBackLog;
                                //tbl_CommandBackLog clsCmd1 = new tbl_CommandBackLog();
                                //clsCmd1.Data = data1;
                                //clsCmd1.EventName = "Add Balance";
                                //clsCmd1.LogDate = DateTime.UtcNow;
                                //clsCmd1.Status = "Pending";
                                //clsCmd1.pld = pldfind.PLD;
                                //clsMetersProd.tbl_CommandBackLog.Add(clsCmd1);
                                //clsMetersProd.SaveChanges();
                                //var sendsms = sendSMSAsync("Rs." + amount + "/-", customerid, DateTime.Now.ToString("dd-MM-yyyy"), getCumDetails.MobilNumber);
                                sendWAAsync("Rs." + tbl_ATGLPaymentAck.PaymentAmount + "/-", customerid, DateTime.Now.ToString("dd-MM-yyyy"), getCumDetails.MobilNumber);
                            }

                        }
                        else if ((outstandingAmount > amount))
                        {
                            var diffAmount1 = outstandingAmount - amount;
                            using (HttpClient client = new HttpClient())
                            {
                                // API endpoint
                                string url = $"http://hesmdms.com/sendWAAsync?meterNumber={smartchck.SmartMeterSerialNumber}&WpTemplateId=697433";

                                // Prepare the body for the POST request
                                var bodywa = new
                                {
                                    placeholderParameters = new string[]
                                    {
                    customerid,
                    "Rs. "+amount+"/-",
                   "Rs. "+outstandingAmount.ToString()+"/-",
                   "Rs. "+diffAmount1.ToString()+"/-"
                                    }
                                };

                                // Serialize the body to JSON
                                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(bodywa);

                                // Create the StringContent for the POST request
                                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                                try
                                {
                                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                                    // Send the POST request synchronously by blocking the async call
                                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                                    if (response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("Request was successful!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Request failed. Status Code: " + response.StatusCode);
                                    }
                                    //sendWAAsyncOutstanding("Rs." + tbl_ATGLPaymentAck.PaymentAmount + "/-", customerid, DateTime.Now.ToString("dd-MM-yyyy"), getCumDetails.MobilNumber,"Rs."+);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error: " + ex.Message);
                                }
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "Success");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
        [System.Web.Http.Route("GetCurrentOutstandingAsync")]
        [System.Web.Http.HttpPost]
        public string GetCurrentOutstandingAsync(string custoemrID)
        {
            string url = $"https://devpgateway.myagl.in/project/public/index.php/quick_bill_set_customer_details/{custoemrID}";
            string username = "pmt_gwt_api";
            string password = "\"kloud@123";

            // Create the Basic Authentication header value
            string authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeaderValue);
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                    HttpResponseMessage response = client.GetAsync(url).Result;  // .Result makes the call synchronous
                    response.EnsureSuccessStatusCode();



                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                    // Extracting variables
                    string custNo = apiResponse.d.Cust_No;
                    string name = apiResponse.d.Name;
                    string mobileNo = apiResponse.d.Mobile_No;
                    string emailId = apiResponse.d.Email_ID;
                    string billNo = apiResponse.d.Bill_No;
                    string billDate = apiResponse.d.Bill_Date;
                    string dueDate = apiResponse.d.Due_Date;
                    string amount = apiResponse.d.Amount;
                    string outstandingAmount = apiResponse.d.Current_Outstanding_Amount;
                    string partnerType = apiResponse.d.Partner_Type;
                    string connType = apiResponse.d.ConnType;
                    string executionTime = apiResponse.d.Execution_time;
                    string msgFlag = apiResponse.d.Msg_Flag;
                    string message = apiResponse.d.Message;
                    int timeDiff = apiResponse.d.time_diff;

                    // Print the extracted values
                    Console.WriteLine($"Cust_No: {custNo}");
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Mobile_No: {mobileNo}");
                    Console.WriteLine($"Email_ID: {emailId}");
                    Console.WriteLine($"Bill_No: {billNo}");
                    Console.WriteLine($"Bill_Date: {billDate}");
                    Console.WriteLine($"Due_Date: {dueDate}");
                    Console.WriteLine($"Amount: {amount}");
                    Console.WriteLine($"Current_Outstanding_Amount: {outstandingAmount}");
                    Console.WriteLine($"Partner_Type: {partnerType}");
                    Console.WriteLine($"ConnType: {connType}");
                    Console.WriteLine($"Execution_time: {executionTime}");
                    Console.WriteLine($"Msg_Flag: {msgFlag}");
                    Console.WriteLine($"Message: {message}");
                    Console.WriteLine($"time_diff: {timeDiff}");
                    return outstandingAmount;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return "1";
                }
            }
        }

        public async Task sendSMSAsync(string amount, string customerid, string date, string mobileNumber)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://www.smsjust.com/sms/user/urlsms.php?username=adanigas&pass=May@2017&senderid=ATGLTD&dest_mobileno={mobileNumber}&msgtype=TXT&message=Dear Customer, Recharge of amount {amount} received for customer id {customerid} on date {date}. In case the recharge amount is not updated on your meter press the button on meter to update the same - Adani Total Gas&response=Y");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

        }
        public async Task sendWAAsync(string amount, string customerid, string date, string mobileNumber)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://devspotbill.adani.com/WhatsappServices/api/ThirdParty/SendWhatsappAsync");

            // Add the headers
            request.Headers.Add("x-api-key", "fc3a55e5-d01e-4e79-a18f-7b22606fad83");
            request.Headers.Add("User-Agent", "okhttp");

            // Create the JSON body with dynamic amount
            string jsonContent = $@"
        {{
            ""mobileNo"": [
                ""{mobileNumber}""
            ],
            ""indicator"": ""ZIS_U_BILL_SSF_INDU_NEW_2"",
            ""wpTemplateId"": ""692177"",
            ""placeholderParameters"": [
                ""{amount}"",
                ""{customerid}"",
                ""{date}""
            ]
        }}";

            // Set the content
            var content = new StringContent(jsonContent, null, "application/json");
            request.Content = content;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                // Send the request and get the response
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Output the response body
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Request failed: {ex.Message}");
            }

        }
        public async Task sendWAAsyncOutstanding(string amount, string customerid, string date, string mobileNumber, string totaloutstanding, string diffamount)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://devspotbill.adani.com/WhatsappServices/api/ThirdParty/SendWhatsappAsync");

            // Add the headers
            request.Headers.Add("x-api-key", "fc3a55e5-d01e-4e79-a18f-7b22606fad83");
            request.Headers.Add("User-Agent", "okhttp");

            // Create the JSON body with dynamic amount
            string jsonContent = $@"
        {{
            ""mobileNo"": [
                ""{mobileNumber}""
            ],
            ""indicator"": ""ZIS_U_BILL_SSF_INDU_NEW_2"",
            ""wpTemplateId"": ""697433"",
            ""placeholderParameters"": [
                ""{customerid}"",
                ""{amount}"",
                ""{totaloutstanding}"",""{diffamount}""
            ]
        }}";

            // Set the content
            var content = new StringContent(jsonContent, null, "application/json");
            request.Content = content;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                // Send the request and get the response
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Output the response body
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Request failed: {ex.Message}");
            }

        }
        public class BillingData
        {
            public DateTime? Date { get; set; }
            public decimal MMBtu { get; set; }
            public decimal SCM { get; set; }
            public decimal Amount { get; set; }
        }
        public class BalanceData
        {
            public DateTime? Date { get; set; }
            public decimal Balance { get; set; }
            public decimal PendingBal { get; set; }
        }
        [System.Web.Http.Route("GetSmartCustomerData")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage GetSmartCustomerData(string CustomerID, string startdate, string enddate)
        {
            try
            {
                DateTime fromDate = Convert.ToDateTime(startdate);
                DateTime toDate = Convert.ToDateTime(enddate);
                var getData = clsMeters.tbl_Customers.Where(c => c.ContractAcct == CustomerID).FirstOrDefault();
                if (getData == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer not found");
                }
                var getMeterDetailscus = clsMeters1.tbl_SGMReg.Where(c => c.CustomreID == getData.ContractAcct).FirstOrDefault();
                if (getMeterDetailscus == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Meter details not found");
                }
                var getmeterDetails = clsMetersProd.tbl_SMeterMaster.Where(x => x.MeterSerialNumber == getMeterDetailscus.SmartMeterSerialNumber).FirstOrDefault();
                var pld = getmeterDetails.PLD;
                var data = clsMetersProd.sp_ResponseSplited_Billing(pld, fromDate, toDate).OrderByDescending(x => x.Date).ToList();
                if (data == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Data Found");
                }
                var billingDataList = new List<BillingData>();
                foreach (var billingData in data)
                {
                    var gcv = "10085.968";
                    var kkcal = Convert.ToDecimal(billingData.TotalConsumptionDifference) * Convert.ToDecimal(10085.968);
                    var btu = Convert.ToDecimal(kkcal) * Convert.ToDecimal(3.968321);
                    var mmbtu = Convert.ToDecimal(btu) / Convert.ToDecimal(Math.Pow(10, 6));
                    var gasAmount = Convert.ToDecimal(billingData.StandardCharge) * mmbtu;
                    var vatCharges = gasAmount * Convert.ToDecimal(0.15);
                    var billingDataItem = new BillingData
                    {
                        Date = billingData.Date,
                        MMBtu = mmbtu,
                        SCM = Convert.ToDecimal(billingData.TotalConsumptionDifference),  // Assuming TotalConsumptionDifference is the SCM
                        Amount = gasAmount + vatCharges  // Adding gas amount and VAT charges
                    };

                    // Add the object to the list
                    billingDataList.Add(billingDataItem);
                }
                return Request.CreateResponse(HttpStatusCode.OK, billingDataList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
        public string GetBalance(string data)
        {

            long longValue = long.Parse(data, System.Globalization.NumberStyles.HexNumber);
            double doubleValue = BitConverter.Int64BitsToDouble(longValue);
            var bal = doubleValue.ToString("F2");

            return bal;

        }
        public string GetCal(string data)
        {
            var balanceutput = string.Join(",", SplitIntoChunks(data, 2));

            // Reverse the array
            string[] splitArray = balanceutput.Split(',');

            // Reverse the array
            Array.Reverse(splitArray);

            // Join the array elements back into a string using a comma as the separator
            balanceutput = string.Join(",", splitArray);

            data = balanceutput.Replace(",", "");
            float creditE = FromHexString(data);


            return creditE.ToString("F1");

        }
        unsafe float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
        }
        [System.Web.Http.Route("FetchLatestBalance")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage FetchLatestBalance(string CustomerID)
        {
            try
            {
                DateTime FromDate = DateTime.Now.AddDays(-15).Date;
                DateTime todate = DateTime.Now.Date;
                var getData = clsMeters.tbl_Customers.Where(c => c.ContractAcct == CustomerID).FirstOrDefault();
                if (getData == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Customer not found");
                }
                var getMeterDetailscus = clsMeters1.tbl_SGMReg.Where(c => c.CustomreID == getData.ContractAcct).FirstOrDefault();
                if (getMeterDetailscus == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Meter details not found");
                }
                decimal pendingBal = 0;
                var getmeterDetails = clsMetersProd.tbl_SMeterMaster.Where(x => x.MeterSerialNumber == getMeterDetailscus.SmartMeterSerialNumber).FirstOrDefault();
                var pld = getmeterDetails.PLD;
                var data = clsMetersProd.sp_ResponseSplited(pld, FromDate, todate).OrderByDescending(x => x.Date).FirstOrDefault();
                var getBlancesql = clsMetersProd.tbl_CommandBackLog.Where(x => x.EventName == "Add Balance" && x.Status == "Pending" &&x.pld==pld).ToList();
                if (getBlancesql != null)
                {
                    foreach (var d in getBlancesql)
                    {
                        var baldata = d.Data;
                        var csting = baldata.Split(',');
                        var balancestring = csting[5] + csting[6] + csting[7] + csting[8];
                        var bal = FromHexString(balancestring);
                        pendingBal = pendingBal + Convert.ToDecimal(bal);
                    }
                }
                var egetBlancesql1 = clsMetersProd.tbl_CommandBackLog.Where(x => x.EventName == "Set E-Credit Threshold" && x.Status == "Pending" && x.pld==pld).FirstOrDefault();
                if (egetBlancesql1 != null)
                {
                    var baldata = egetBlancesql1.Data;
                    var csting = baldata.Split(',');
                    var balancestring = csting[5] + "," + csting[6] + "," + csting[7] + "," + csting[8];
                    string[] array1 = balancestring.Split(',');
                    Array.Reverse(array1);
                    var ebalanceutput = string.Join(",", array1);
                    var bal = FromHexString(balancestring);
                    pendingBal = Convert.ToDecimal(pendingBal) + Convert.ToDecimal(pendingBal);
                }

                if (data == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Data Found");
                }
                var accountBalance = data.AccountBalance;
                var ebal = data.eCreditBalance;
                var totalBalance = Convert.ToDecimal(ebal) + Convert.ToDecimal(accountBalance);
                var balance = new BalanceData
                {
                    Date = data.Date,

                    Balance = totalBalance,  // Adding gas amount and VAT charges
                    PendingBal = pendingBal
                };
                return Request.CreateResponse(HttpStatusCode.OK, balance);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }
        }
        //[System.Web.Http.Route("FetchBalancePrepaid")]
        //[System.Web.Http.HttpPost]
        //public HttpResponseMessage FetchBalancePrepaid(FetchBalance FetchBalance)
        //{
        //    try
        //    {
        //        var customerid = FetchBalance.CustomerID;
        //        var BpNumber = clsMeters.tbl_Customers.Where(x => x.ContractAcct == customerid).FirstOrDefault();
        //        var data = clsMetersProd.sp_ResponseSplited_Billing().Where(x => x.BusinessPatner == BpNumber.BusinessPatner).OrderByDescending(x => x.Date).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

        //    }
        //}
    }
}