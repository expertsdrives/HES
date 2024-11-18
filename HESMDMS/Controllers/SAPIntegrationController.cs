using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using EmailLib;
using HESMDMS.Models;
using IronPdf;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
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




        [System.Web.Http.Route("ATGLPaymnetACK")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage ATGLPaymnetACK(tbl_ATGLPaymentAck tbl_ATGLPaymentAck)
        {
            try
            {
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
                    return Request.CreateResponse(HttpStatusCode.OK, "Success");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

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
                var getMeterDetails = clsMetersProd.tbl_AssignSmartMeter.Where(c => c.CustomerInstallationID == getData.ID).FirstOrDefault();
                if (getMeterDetails == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Meter details not found");
                }
                var pld = getMeterDetails.pld;
                var data = clsMetersProd.sp_ResponseSplited_Billing(pld, fromDate, toDate).ToList().OrderBy(x => x.Date);
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
                var getMeterDetails = clsMetersProd.tbl_AssignSmartMeter.Where(c => c.CustomerInstallationID == getData.ID).FirstOrDefault();
                if (getMeterDetails == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Meter details not found");
                }
                var pld = getMeterDetails.pld;
                var data = clsMetersProd.sp_ResponseSplited_Billing(pld, FromDate, todate).OrderByDescending(x=>x.Date).FirstOrDefault();
                if (data == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Data Found");
                }
                var accountBalance = data.AccountBalance;
                var ebal= data.eCreditBalance;
                var totalBalance= Convert.ToDecimal(ebal)+Convert.ToDecimal(accountBalance);
                var balance = new BalanceData
                {
                    Date = data.Date,
                
                    Balance = totalBalance  // Adding gas amount and VAT charges
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