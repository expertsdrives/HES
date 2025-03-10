using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using HESMDMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.Drawing.Slicer.Style;
using Sentry;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static HESMDMS.Controllers.GISController;

namespace HESMDMS.Controllers
{

    public class CrudOperationController : ApiController
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeterEntities1 clsMeters1 = new SmartMeterEntities1();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
        SmartMeter_ProdEntities1 clsMeters_Prods = new SmartMeter_ProdEntities1();
        ElectricMeterEntities electric = new ElectricMeterEntities();
        [Route("CustomerLoad")]
        [HttpGet]
        public HttpResponseMessage CustomerLoad(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_CustomerRegistration.OrderByDescending(X => X.ID), loadOptions));
        }
        [Route("AMRDataReception")]
        [HttpGet]
        public HttpResponseMessage AMRDataReception(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_RawDataAPI.OrderByDescending(X => X.ID), loadOptions));
        }
        [Route("AMRRawData")]
        [HttpGet]
        public HttpResponseMessage AMRRawData(DataSourceLoadOptions loadOptions)
        {
            var date = Convert.ToDateTime("2024-05-15");
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.V_RawData.OrderByDescending(X => X.ID).Where(x => x.DateTime >= date).Take(100000), loadOptions));
        }
        [Route("CallToMap1")]
        [HttpPost]
        public HttpResponseMessage CallToMap1(string startDate, string endDate, string area)
        {
            DateTime sdate = Convert.ToDateTime(startDate);
            DateTime edate = Convert.ToDateTime(endDate);
            clsMeters.Database.CommandTimeout = 300;
            var activeAMR = clsMeters.Database.SqlQuery<activeAMR>(
          "exec sp_ActiveAMR @p0, @p1, @p2",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate),
           new SqlParameter("@p2", area)// Pass an empty string or any other parameter if required
      ).ToList();
            var missingConsumptionSerials = clsMeters.Database.SqlQuery<inactiveAMR>(
          "exec sp_InactiveAMR @p0, @p1, @p2",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate),
            new SqlParameter("@p2", area)// Pass an empty string or any other parameter if required
      ).ToList();
            var activeVayudut = (from consumption in clsMeters.tbl_DataReception
                                 join customer in clsMeters.tbl_VayadutMasterDetails
                                 on consumption.VAYUDUT_ID equals customer.VayadutId
                                 where consumption.Date >= sdate && consumption.Date <= edate
                                 select new
                                 {
                                     VayudutID = consumption.VAYUDUT_ID,
                                     Latitude = customer.Latitude,
                                     Longitude = customer.Longitude
                                 })
                     .Distinct()
                     .ToList();


            var missingYayudutSerials = clsMeters.Database.SqlQuery<inactiveVayudut>(
          "exec sp_InactiveVayudut @p0, @p1",
          new SqlParameter("@p0", sdate),
          new SqlParameter("@p1", edate)  // Pass an empty string or any other parameter if required
      ).ToList();





            var ActiveAMRJson = activeAMR
     .Select(x => new
     {
         id = x.AMRSerialNumber, // Replace with the property that provides the unique identifier for the AMR
         lat = double.TryParse(x.Latitude, out double lat) ? lat : 0.0,
         lng = double.TryParse(x.Longitude, out double lng) ? lng : 0.0,
         functional = true // Replace with the boolean field/property indicating functionality
     })
     .ToList();



            var missingARMJson = missingConsumptionSerials
      .Select(x => new
      {
          id = x.SerialNumber, // Replace with the property that provides the unique identifier for the AMR
          lat = double.TryParse(x.Latitude, out double lat) ? lat : 0.0,
          lng = double.TryParse(x.Longitude, out double lng) ? lng : 0.0,
          functional = false // Replace with the boolean field/property indicating functionality
      })
      .ToList();

            var ActiveVayudutJson = activeVayudut
   .Select(x => new
   {
       id = x.VayudutID, // Replace with the property that provides the unique identifier for the AMR
       lat = double.TryParse(Convert.ToString(x.Latitude), out double lat) ? lat : 0.0,
       lng = double.TryParse(Convert.ToString(x.Longitude), out double lng) ? lng : 0.0,
       functional = true // Replace with the boolean field/property indicating functionality
   })
    .ToList();


            var missingVayudutJson = missingYayudutSerials
     .Select(x => new
     {
         id = x.SerialNumber, // Replace with the property that provides the unique identifier for the AMR
         lat = double.TryParse(Convert.ToString(x.Latitude), out double lat) ? lat : 0.0,
         lng = double.TryParse(Convert.ToString(x.Longitude), out double lng) ? lng : 0.0,
         functional = false // Replace with the boolean field/property indicating functionality
     })
     .ToList();
            var serialNumber = activeAMR
      .Select(x => new List<string>
      {
        x.AMRSerialNumber
      })
      .ToList();
            var serializedData = new
            {
                functionalGateways = JsonConvert.SerializeObject(ActiveVayudutJson),
                nonFunctionalGateways = JsonConvert.SerializeObject(missingVayudutJson),
                functionalAMRs = JsonConvert.SerializeObject(ActiveAMRJson),
                nonFunctionalAMRs = JsonConvert.SerializeObject(missingARMJson)
            };
            var gatewayDataJson = JsonConvert.SerializeObject(serializedData);
            return Request.CreateResponse(HttpStatusCode.OK, gatewayDataJson);
        }
        [Route("DisplayVayudut")]
        [HttpGet]
        public HttpResponseMessage DisplayVayudut(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_VayadutMasterDetails, loadOptions));
        }

        [Route("AMRDataReceptionCRC")]
        [HttpGet]
        public HttpResponseMessage AMRDataReceptionCRC(DataSourceLoadOptions loadOptions)
        {

                clsMeters.Database.CommandTimeout = 500;

            DateTime date1 = Convert.ToDateTime("2024-12-01");
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_POC().Where(x => x.Date >= date1), loadOptions));
            

        }
        [Route("OutofRange")]
        [HttpGet]
        public HttpResponseMessage OutofRange(DataSourceLoadOptions loadOptions)
        {

            DateTime date1 = Convert.ToDateTime("2024-01-01");
            decimal upperLimit = Convert.ToDecimal(1.5);
            decimal lowerLimit = Convert.ToDecimal(0);
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_POC().Where(x => x.Date >= date1 && (x.DailyConsumption > upperLimit || x.DailyConsumption < lowerLimit) && x.Street.ToLower() == "khurja"), loadOptions));
        }
        [Route("LoadMeterMaster")]
        [HttpGet]
        public HttpResponseMessage LoadMeterMaster(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_MeterMaster, loadOptions));
        }

        [Route("InsertMeterMaster")]
        [HttpPost]
        public HttpResponseMessage InsertMeterMaster(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_MeterMaster();
            JsonConvert.PopulateObject(values, newOrder);
            if (newOrder.IsActive == null)
            {
                newOrder.IsActive = true;
            }
            newOrder.CreatedDate = DateTime.Now;
            newOrder.CreatedBy = "";
            Validate(newOrder);
            clsMeters.tbl_MeterMaster.Add(newOrder);
            clsMeters.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);
        }
        [Route("InsertVayudut")]
        [HttpPost]
        public HttpResponseMessage InsertVayudut(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_VayadutMasterDetails();
            JsonConvert.PopulateObject(values, newOrder);

            Validate(newOrder);
            clsMeters.tbl_VayadutMasterDetails.Add(newOrder);
            clsMeters.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);
        }
        [HttpPut]
        [Route("UpdateMeterMaster")]
        public HttpResponseMessage UpdateMeterMaster(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var order = clsMeters.tbl_MeterMaster.First(o => o.ID == key);
            JsonConvert.PopulateObject(values, order);
            Validate(order);
            clsMeters.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, order);
        }
        [HttpPut]
        [Route("UpdateSmartMeterCommands")]
        public HttpResponseMessage UpdateSmartMeterCommands(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var order = clsMeters_Prods.tbl_epsettings.First(o => o.ID == key);
            JsonConvert.PopulateObject(values, order);
            Validate(order);
            clsMeters_Prods.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, order);
        }
        [HttpPut]
        [Route("UpdateVayudut")]
        public HttpResponseMessage UpdateVayudut(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var order = clsMeters.tbl_VayadutMasterDetails.First(o => o.ID == key);
            JsonConvert.PopulateObject(values, order);
            Validate(order);
            clsMeters.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, order);
        }
        [Route("LoadCustomerRegistration")]
        [HttpGet]
        public HttpResponseMessage LoadCustomerRegistration(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_CustomerDetails, loadOptions));
        }
        [Route("LoadMeterLookup")]
        [HttpGet]
        public HttpResponseMessage LoadMeterLookup(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(from post in clsMeters.tbl_MeterMaster
                                                                join meta in clsMeters.tbl_CustomerDetails on post.Number equals meta.SerialNumber
                                                                where meta.Street5.ToLower() == "khurja" || meta.Street5.ToLower() == "plant"
                                                                select new { ID = post.ID, Number = post.Number }, loadOptions));
        }
        [Route("SelectBus")]
        [HttpGet]
        public HttpResponseMessage SelectBus(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(from post in clsMeters.tbl_MeterMaster
                                                                join meta in clsMeters.tbl_CustomerDetails on post.Number equals meta.SerialNumber
                                                                where meta.City.ToLower() == "khurja" || meta.Street5 == "plant"
                                                                select new { ID = post.ID, Number = post.Number }, loadOptions));
        }
        [Route("InsertCustomerRegistration")]
        [HttpPost]
        public HttpResponseMessage InsertCustomerRegistration(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_CustomerDetails();
            JsonConvert.PopulateObject(values, newOrder);
            Validate(newOrder);
            clsMeters.tbl_CustomerDetails.Add(newOrder);
            clsMeters.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);
        }
        [Route("InsertSmartMeterCommands")]
        [HttpPost]
        public HttpResponseMessage InsertSmartMeterCommands(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_epsettings();
            JsonConvert.PopulateObject(values, newOrder);
            Validate(newOrder);
            clsMeters_Prods.tbl_epsettings.Add(newOrder);
            clsMeters_Prods.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);
        }
        [Route("InsertElectricMeterMaster")]
        [HttpPost]
        public HttpResponseMessage InsertElectricMeterMaster(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_MeterMasterRelay();
            JsonConvert.PopulateObject(values, newOrder);
            newOrder.Port = "4059";
            newOrder.ClientAddress = "48";
            newOrder.ServerAddres = "1";
            newOrder.Authentication = "2";
            newOrder.Password = "wwwwwwwwwwwwwwww";
            newOrder.InterfaceType = "1";
            newOrder.Security = "48";
            newOrder.BlockCipherKey = "62626262626262626262626262626262";
            newOrder.AuthenticationKey = "62626262626262626262626262626262";
            newOrder.InvocationCounter = "0.0.43.1.3.255";
            Validate(newOrder);
            electric.tbl_MeterMasterRelay.Add(newOrder);
            electric.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);
        }

        [Route("smartmeterdata")]
        [HttpGet]
        public HttpResponseMessage smartmeterdata(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.sp_SmartMeterData(), loaddata));
        }
        [Route("electricsmartmeterdata")]
        [HttpGet]
        public HttpResponseMessage electricsmartmeterdata(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(electric.sp_FetchData(), loaddata));
        }
        [Route("blelogs")]
        [HttpGet]
        public HttpResponseMessage blelogs(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(electric.tbl_BLEDemo, loaddata));
        }
        [Route("ElectricMeterMaster")]
        [HttpGet]
        public HttpResponseMessage ElectricMeterMaster(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(electric.tbl_MeterMasterRelay, loaddata));
        }

        [Route("DeltaCustomers")]
        [HttpGet]
        public HttpResponseMessage DeltaCustomers(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_Customers, loaddata));
        }
        [Route("SGMReg")]
        [HttpGet]
        public HttpResponseMessage SGMReg(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters1.tbl_SGMReg, loaddata));
        } 
        [Route("ATGLRates")]
        [HttpGet]
        public HttpResponseMessage ATGLRates(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters1.tbl_ATGLRates, loaddata));
        }
        [Route("SGMMeterFetch")]
        [HttpGet]
        public HttpResponseMessage SGMMeterFetch(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.tbl_SMeterMaster.Where(x=>x.MeterSerialNumber.StartsWith("240")), loaddata));
        }
        [Route("lorarawdata")]
        [HttpGet]
        public HttpResponseMessage lorarawdata(DataSourceLoadOptions loaddata)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_MQTTBroker.OrderByDescending(x => x.ID), loaddata));
        }
        public class brokerData
        {
            public string meterid { get; set; }
            public string data { get; set; }
        }
        [Route("d2cCustomer")]
        [HttpGet]
        public HttpResponseMessage d2cCustomer(DataSourceLoadOptions loaddata)
        {

            SmartMeter_ProdEntities1 clsMetersProd1 = new SmartMeter_ProdEntities1();
            clsMetersProd1.Database.CommandTimeout = 500;
            clsMeters_Prod.Database.CommandTimeout = 300;
            var activeAMR = clsMeters_Prod.Database.SqlQuery<SCustomerData>("exec sp_CustomerLogs").ToList();
            return Request.CreateResponse(DataSourceLoader.Load(activeAMR, loaddata));
        }
        [Route("SmartMeterCommands")]
        [HttpGet]
        public HttpResponseMessage SmartMeterCommands(DataSourceLoadOptions loaddata)
        {

            SmartMeter_ProdEntities1 clsMetersProd1 = new SmartMeter_ProdEntities1();
            clsMetersProd1.Database.CommandTimeout = 500;
            clsMeters_Prod.Database.CommandTimeout = 300;
            var activeAMR = clsMetersProd1.tbl_epsettings.ToList();
            return Request.CreateResponse(DataSourceLoader.Load(activeAMR, loaddata));
        }
        [Route("smtplmqtt")]
        [HttpPost]
        public async Task<HttpResponseMessage> DataReceptionwithCRC()
        {
            tbl_MQTTBroker broker = new tbl_MQTTBroker();
            string result = await Request.Content.ReadAsStringAsync();
            if (result == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid data.");
            }
            tbl_MQTTBroker meterData = JsonConvert.DeserializeObject<tbl_MQTTBroker>(result);
            meterData.CreatedDate = DateTime.Now;
            clsMeters.tbl_MQTTBroker.Add(meterData);
            clsMeters.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, meterData);
        }

        [Route("d2c")]
        [HttpGet]
        public HttpResponseMessage d2c(DataSourceLoadOptions loaddata, string roleid, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Default to the past seven days if dates are not provided
            DateTime defaultStartDate = DateTime.Now.AddDays(-1);
            DateTime defaultEndDate = DateTime.Now;

            // Use provided dates or defaults
            DateTime filterStartDate = startDate ?? defaultStartDate;
            DateTime filterEndDate = endDate ?? defaultEndDate;

            // Fetch data within the date range
            var data = clsMeters_Prod.tbl_JioLogs
                .Where(x => x.DateTime >= filterStartDate && x.DateTime <= filterEndDate)
                .ToList();

            List<ModelParameter> model = new List<ModelParameter>();
            foreach (var fData in data)
            {
                if (IsValidJson(fData.SMTPLResponse))
                {
                    FetchJioLogs deserializedProduct = JsonConvert.DeserializeObject<FetchJioLogs>(fData.SMTPLResponse);
                    if (deserializedProduct.pld != null && deserializedProduct.Data != null)
                    {
                        string[] splitarray = deserializedProduct.Data.ToString().Split(',');
                        int sizes = splitarray.Length;
                        if (sizes == 25)
                        {
                            DateTime? datetime = fData.DateTime;
                            DateTime nonNullableDateTime = DateTime.Now;
                            if (datetime.HasValue)
                            {
                                nonNullableDateTime = datetime.Value;
                                TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                                nonNullableDateTime = TimeZoneInfo.ConvertTimeFromUtc(nonNullableDateTime, istTimeZone);
                            }

                            if (decimal.TryParse(splitarray[10].Trim(), out decimal number))
                            {
                                var bt = Convert.ToDecimal(splitarray[10].Trim()) / 1000;
                                string input = splitarray[11].Trim();
                                int chunkSize = 2;
                                string chunk = "";
                                for (int i = 0; i < input.Length; i += chunkSize)
                                {
                                    chunk += input.Substring(i, Math.Min(chunkSize, input.Length - i)) + ",";
                                }
                                var splitChunks = chunk.Split(',');
                                string syshealth = splitarray[18].Trim();

                                var syshealthchunks = Enumerable.Range(0, (syshealth.Length + chunkSize - 1) / chunkSize)
                                               .Select(i => syshealth.Substring(i * chunkSize, Math.Min(chunkSize, syshealth.Length - i * chunkSize)));
                                string result = string.Join(",", syshealthchunks);
                                var NB = result.Split(',')[3];
                                var mwtmerge = result.Split(',')[0] + result.Split(',')[1];
                                var CC = result.Split(',')[2];
                                var param = new ModelParameter
                                {
                                    ID = fData.ID,
                                    InstrumentID = splitarray[1].Trim(),
                                    Date = splitarray[2],
                                    Time = splitarray[3],
                                    DateRx = nonNullableDateTime.Date.ToString(),
                                    TimeRx = nonNullableDateTime.ToString("HH:mm:ss"),
                                    Record = splitarray[4].Trim(),
                                    MeasurementValue = splitarray[7].Trim(),
                                    TotalConsumption = splitarray[8].Trim(),
                                    BatteryVoltage = bt,
                                    MagnetTamper = Convert.ToInt32(splitChunks[0], 16).ToString(),
                                    CaseTamper = Convert.ToInt32(splitChunks[1], 16).ToString(),
                                    BatteryRemovalCount = Convert.ToInt32(splitChunks[2], 16).ToString(),
                                    ExcessiveGasFlow = Convert.ToInt32(splitChunks[3], 16).ToString(),
                                    ExcessivePushKey = Convert.ToInt32(splitChunks[4], 16).ToString(),
                                    SOVTamper = Convert.ToInt32(splitChunks[5], 16).ToString(),
                                    TiltTamper = Convert.ToInt32(splitChunks[6], 16).ToString(),
                                    InvalidUserLoginTamper = Convert.ToInt32(splitChunks[7], 16).ToString(),
                                    NBIoTModuleError = Convert.ToInt32(splitChunks[8], 16).ToString(),
                                    AccountBalance = splitarray[12].Trim().Replace("+", ""),
                                    eCreditBalance = splitarray[13].Trim().Replace("+", ""),
                                    StandardCharge = splitarray[14].Trim(),
                                    ValvePosition = splitarray[17].Trim() == "0" ? "Unknown" : splitarray[17].Trim() == "1" ?
                                                                        "SOV Not Present" : splitarray[17].Trim() == "2" ?
                                                                        "SOV Opening" : splitarray[17].Trim() == "3" ?
                                                                        "SOV Closing" : splitarray[17].Trim() == "4" ?
                                                                        "SOV Open" : splitarray[17] == "5" ?
                                                                        "SOV Close" : splitarray[17] == "6" ? "SOV Stuck" : "",
                                    NBIoTRSSI = Convert.ToInt32(NB, 16).ToString(),
                                    ContinuousConsumption = Convert.ToInt32(CC, 16).ToString(),
                                    MWT = Convert.ToInt32(mwtmerge, 16).ToString(),
                                    TransmissionPacket = splitarray[19].Trim(),
                                    Temperature = splitarray[21].Trim(),
                                    TarrifName = splitarray[22].Trim() == "01" ? "Standard" : "Undefined",
                                    GasCalorific = splitarray[20].Trim(),
                                    Checksum = "Valid", //splitarray[23].Trim(),
                                };

                                if (roleid == "9")
                                {
                                    if (splitarray[1].Trim() == "00100111" || splitarray[1].Trim() == "00100115")
                                    {
                                        model.Add(param);
                                    }
                                }
                                else
                                {
                                    model.Add(param);
                                }
                            }
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(model);
            return Request.CreateResponse(DataSourceLoader.Load(model, loaddata));
        }
        //[Route("Load15DayReport")]
        //[HttpGet]
        //public HttpResponseMessage Load15DayReport(DataSourceLoadOptions loadOptions)
        //{


        //    return Request.CreateResponse(DataSourceLoader.Load(table15DayReport, loadOptions));
        //}
        private bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var js = new Newtonsoft.Json.JsonSerializer();
                    js.Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(strInput)));
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;

                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        [Route("MagneticTemper")]
        [HttpGet]
        public HttpResponseMessage MagneticTemper(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_MagneticTemper(), loadOptions));
        }
        [Route("CaseTemper")]
        [HttpGet]
        public HttpResponseMessage CaseTemper(DataSourceLoadOptions loadOptions, string roleid)
        {
            if (roleid == "5")
            {

                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_CaseTemper_Demo(), loadOptions));
            }
            else
            {
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_CaseTemper(), loadOptions));
            }
        }
        [Route("VayudutWise")]
        [HttpGet]
        public HttpResponseMessage VayudutWise(DataSourceLoadOptions loadOptions, string roleid)
        {

            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_VayudutWise(), loadOptions));

        }
        [Route("LastMeterReading")]
        [HttpGet]
        public HttpResponseMessage LastMeterReading(DateTime fromdate, DateTime todate, DataSourceLoadOptions loadOptions, string roleid, string data)
        {

            //if (data == "All" || data == "" || data == null)
            //{
            DateTime start = Convert.ToDateTime(fromdate);
            //DateTime end = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-1), INDIAN_ZONE);
            DateTime end = Convert.ToDateTime(todate);
            //    if (roleid == "5")
            //    {
            //        return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_Demo().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
            //    }
            //    else
            //    {
            //        clsMeters.Database.CommandTimeout = 300;
            //        //var a = clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate && x.Street == data).Count();
            //        return Request.CreateResponse(DataSourceLoader.Load(clsMeters1.sp_FetchConsumptionData(fromdate,todate), loadOptions));
            //    }
            //}
            //else
            //{
            //    DateTime start = Convert.ToDateTime(fromdate);
            //    //DateTime end = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-1), INDIAN_ZONE);
            //    DateTime end = Convert.ToDateTime(todate);
            //    if (roleid == "5")
            //    {
            //        return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_Demo().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
            //    }
            //    else
            //    {
            //var a = clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate && x.Street == data).Count();
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters1.sp_FetchConsumptionData(start, end), loadOptions));
            //}

            //}
            //DateTime start = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-15), INDIAN_ZONE);

        }
        [Route("LastMeterRevenue")]
        [HttpGet]
        public HttpResponseMessage LastMeterRevenue(DateTime fromdate, DateTime todate, DataSourceLoadOptions loadOptions, string roleid)
        {
            if (roleid == "5")
            {

                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_15DayRevenue_Demo().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
            }
            else
            {
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_15DayRevenue().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
            }
        }
        [Route("GetDataBilling")]
        [HttpGet]
        public HttpResponseMessage GetDataBilling(DataSourceLoadOptions loadOptions)
        {


            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.BillingCustomer(), loadOptions));


        }
        [Route("GetDataBillingAMR")]
        [HttpGet]
        public HttpResponseMessage GetDataBillingAMR(DataSourceLoadOptions loadOptions)
        {


            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.sp_AMRBill().OrderByDescending(x => x.Date), loadOptions));


        }
        [Route("GetAMRHealth")]
        [HttpGet]
        public HttpResponseMessage GetAMRHealth(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.sp_AMRHealth(), loadOptions));
        }

        [Route("SelectSmartMeter")]
        [HttpGet]
        public HttpResponseMessage SelectSmartMeter(DataSourceLoadOptions loadOptions, string roleid)
        {
            if (roleid == "9")
            {
                var data = (from log in clsMeters_Prod.tbl_SMeterMaster
                            select new
                            {
                                ID = log.ID,
                                TempMeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                                AID = log.AID,
                                PLD = log.PLD,
                            });
                return Request.CreateResponse(DataSourceLoader.Load(data.Where(x => x.PLD == "PALTMaYpNqExrvpg" || x.PLD == "PAJcdKb3Wx0iVqJc"), loadOptions));
            }
            else
            {
                var data = (from log in clsMeters_Prod.tbl_SMeterMaster
                            select new
                            {
                                ID = log.ID,
                                TempMeterID = log.MeterSerialNumber == null ? log.TempMeterID : log.MeterSerialNumber,
                                AID = log.AID,
                                PLD = log.PLD,
                            }).OrderByDescending(x => x.ID);
                return Request.CreateResponse(DataSourceLoader.Load(data, loadOptions));
            }

        }
        [HttpGet]
        [Route("SendC2DAsync")]
        public async Task<HttpResponseMessage> SendC2DAsync(string pld)
        {
            var abc = clsMeters_Prod.Database.ExecuteSqlCommand(
                                    "UPDATE tbl_FirmwareHistoty " +
                                            "SET JioResponse = 'Started' " +
                                        "WHERE pld = {0}",
                                                                    pld);
            Random rnd = new Random();
            double rndNumber = Convert.ToDouble(DateTime.Now.ToString("yyMMddHHmmsss"));
            int size = rndNumber.ToString().Length;
            if (size != 12)
            {
                rndNumber = Convert.ToDouble(string.Format("{0}{1}", 0, rndNumber));
            }
            var meterDetails = clsMeters_Prod.tbl_SMeterMaster.Where(x => x.PLD == pld).FirstOrDefault();
            var chkFirmware = clsMeters_Prod.tbl_FirmwareHistoty.Where(x => x.JioResponse == "Started" && x.pld == pld).ToList();
            foreach (var meter in chkFirmware)
            {
                var existingEntity1 = clsMeters_Prod.tbl_FirmwareHistoty.Find(meter.ID);
                existingEntity1.JioResponse = "Completed";
                clsMeters_Prod.Entry(existingEntity1).State = EntityState.Modified;
                clsMeters_Prod.SaveChanges();
                var bob1 = new
                {
                    idType = "PLD",
                    id = pld,
                    transactionId = rndNumber,
                    retentionTime = DateTime.Now.ToString(),
                    data = new[]
                    {
            new { aid = meterDetails.AID, dataformat = "cp", dataType = "JSON", ext = "{'data': '" + meter.PacketData + "'}" }
        }
                };

                var content1 = JsonConvert.SerializeObject(bob1);

                // Login request
                Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login");
                var objClient3 = new System.Net.Http.HttpClient();
                c2dProd users1 = new c2dProd
                {
                    grant_type = "password",
                    username = "2025000_2890001@iot.jio.com",
                    password = "a737b902951ec15cff735357a850b09cd941818095527a1925760b5a4e471464",
                    client_id = "db2f04a5e72547cbb68331f406946494",
                    client_secret = "d95578aa9b1eb30e"
                };

                string json1 = JsonConvert.SerializeObject(users1);
                HttpResponseMessage response3 = await objClient3.PostAsync(requestUri1, new StringContent(json1, System.Text.Encoding.UTF8, "application/json"));

                if (!response3.IsSuccessStatusCode)
                {
                    // Return error response
                    return new HttpResponseMessage(response3.StatusCode)
                    {
                        Content = new StringContent("Failed to authenticate.")
                    };
                }

                string responseJsonText1 = await response3.Content.ReadAsStringAsync();
                var bearerToken1 = JsonConvert.DeserializeObject<c2dTokenProd>(responseJsonText1);

                if (bearerToken1?.access_token == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Authentication token not received.")
                    };
                }
                string eid = Convert.ToString(meterDetails.EID);
                // Send command
                Uri requestUri2 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified");
                var objClient2 = new System.Net.Http.HttpClient();
                objClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objClient2.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken1.access_token);
                objClient2.DefaultRequestHeaders.Add("eid", eid);

                HttpResponseMessage response1 = await objClient2.PostAsync(requestUri2, new StringContent(content1, System.Text.Encoding.UTF8, "application/json"));

                if (!response1.IsSuccessStatusCode)
                {
                    return new HttpResponseMessage(response1.StatusCode)
                    {
                        Content = new StringContent("Failed to send C2D command.")
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Command sent successfully.")

                };
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent("No Data")
            };
        }
        [Route("SmartMeterBackLogs")]
        [HttpGet]
        public HttpResponseMessage SmartMeterBackLogs(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.tbl_CommandBackLog, loadOptions));
        }
        [Route("MeterFromPLD")]
        [HttpGet]
        public HttpResponseMessage MeterFromPLD(DataSourceLoadOptions loadOptions, string pld)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.tbl_SMeterMaster.Where(x => x.PLD == pld), loadOptions));
        }
        [Route("LoadSmartUser")]
        [HttpGet]
        public HttpResponseMessage LoadSmartUser(DataSourceLoadOptions loadOptions)
        {

            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.sp_SmartUser(), loadOptions));
        }

        [Route("LoadSMeterLookup")]
        [HttpGet]
        public HttpResponseMessage LoadSMeterLookup(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.tbl_SMeterMaster, loadOptions));
        }
        [Route("InsertSmartUser")]
        [HttpPost]
        public HttpResponseMessage InsertSmartUser(FormDataCollection form)
        {
            var values = form.Get("values");

            var newOrder = new tbl_AdminCredentials();
            newOrder.RoleID = 5;
            JsonConvert.PopulateObject(values, newOrder);
            Validate(newOrder);
            clsMeters.tbl_AdminCredentials.Add(newOrder);
            clsMeters.SaveChanges();
            var json = JsonConvert.DeserializeObject<sp_SmartUser_Result>(values);
            var us = json.Username;
            var LastUser = clsMeters.tbl_AdminCredentials.Where(x => x.Username == us).FirstOrDefault();
            var SUser = new tbl_SmartMeterUser();
            SUser.UserID = LastUser.ID;
            SUser.MeterID = json.MeterID.ToString();
            clsMeters_Prod.tbl_SmartMeterUser.Add(SUser);
            clsMeters_Prod.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);

        }
        [Route("InsertSGMReg")]
        [HttpPost]
        public HttpResponseMessage InsertSGMReg(FormDataCollection form)
        {
            var values = form.Get("values");
            var newOrder = new tbl_SGMReg();
            newOrder.CreatedDate=DateTime.Now;
            JsonConvert.PopulateObject(values, newOrder);
            bool exists = clsMeters1.tbl_SGMReg.Any(x => x.CustomreID == newOrder.CustomreID || x.SmartMeterSerialNumber == newOrder.SmartMeterSerialNumber);

            if (exists)
            {
                // Return a conflict response indicating the record already exists
                return Request.CreateResponse(HttpStatusCode.Conflict, "Record with the same CustomerID or Meter Number already exists.");
            }
            Validate(newOrder);
            clsMeters1.tbl_SGMReg.Add(newOrder);
            clsMeters1.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);

        }
        [Route("ATGLRatesInsert")]
        [HttpPost]
        public HttpResponseMessage ATGLRatesInsert(FormDataCollection form)
        {
            var values = form.Get("values");
            var newOrder = new tbl_ATGLRates();
            newOrder.CreatedDate = DateTime.Now;
            
            
            JsonConvert.PopulateObject(values, newOrder);
            Validate(newOrder);
            clsMeters1.tbl_ATGLRates.Add(newOrder);
            clsMeters1.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created, newOrder);

        }
        public class RequestModel
        {
            public List<string> PlaceholderParameters { get; set; }
        }
        [Route("sendWAAsync")]
        [HttpPost]
        public async Task sendWAAsync(string meterNumber,string WpTemplateId, [FromBody] RequestModel request1)
        {
            var SGMREg = clsMeters1.tbl_SGMReg.Where(x=>x.SmartMeterSerialNumber==meterNumber).FirstOrDefault();
            var customerID = SGMREg.CustomreID;
            var customerDetails= clsMeters.tbl_Customers.Where(x=>x.ContractAcct==customerID).FirstOrDefault();
            string mobileNo = customerDetails.MobilNumber;
            string wpTemplateId = WpTemplateId;
            List<string> placeholderParameters = request1.PlaceholderParameters;
            placeholderParameters[0] = customerID;
            var placeholderParamsString = string.Join("\", \"", placeholderParameters);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://devspotbill.adani.com/WhatsappServices/api/ThirdParty/SendWhatsappAsync");
            
            // Add the headers
            request.Headers.Add("x-api-key", "fc3a55e5-d01e-4e79-a18f-7b22606fad83");
            request.Headers.Add("User-Agent", "okhttp");

            // Create the JSON body with dynamic amount
            string jsonContent = $@"
        {{
            ""mobileNo"": [
                ""{mobileNo}""
            ],
            ""indicator"": ""ZIS_U_BILL_SSF_INDU_NEW_2"",
            ""wpTemplateId"": ""{wpTemplateId}"",
            ""placeholderParameters"": [
                  ""{placeholderParamsString}""
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
                tbl_ATGLWaACK wa = new tbl_ATGLWaACK();
                wa.MeterNumber=meterNumber;
                wa.Remarks = WpTemplateId;
                wa.Status = true;
                wa.CreatedDate = DateTime.Now.Date;
                clsMeters_Prods.tbl_ATGLWaACK.Add(wa);
                clsMeters_Prods.SaveChanges();
                // Output the response body
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Request failed: {ex.Message}");
            }

        }
        [Route("GetBalance")]
        [HttpPost]
        public async Task GetBalance(string pld, string aid, string eid)
        {
            string eventname = "";

        }
    }
}
