using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using HESMDMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml.Drawing.Slicer.Style;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace HESMDMS.Controllers
{

    public class CrudOperationController : ApiController
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
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
            var date = Convert.ToDateTime("2024-01-15");
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.V_RawData.OrderByDescending(X => X.ID).Where(x => x.DateTime >= date).Take(100000), loadOptions));
        }

        [Route("DisplayVayudut")]
        [HttpGet]
        public HttpResponseMessage DisplayVayudut(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_VayudutRegistration, loadOptions));
        }

        [Route("AMRDataReceptionCRC")]
        [HttpGet]
        public HttpResponseMessage AMRDataReceptionCRC(DataSourceLoadOptions loadOptions, string roleid)
        {

            if (roleid == "5")
            {
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_Demo(), loadOptions));
            }
            else
            {
                DateTime date1 = Convert.ToDateTime("2024-01-01");
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_POC().Where(x => x.Date >= date1), loadOptions));
            }

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

            var newOrder = new tbl_VayudutRegistration();
            JsonConvert.PopulateObject(values, newOrder);
            newOrder.IsAssigned = false;
            newOrder.CreatedDate = DateTime.Now;
            Validate(newOrder);
            clsMeters.tbl_VayudutRegistration.Add(newOrder);
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
        [Route("UpdateVayudut")]
        public HttpResponseMessage UpdateVayudut(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var order = clsMeters.tbl_VayudutRegistration.First(o => o.ID == key);
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

        [Route("d2c")]
        [HttpGet]
        public HttpResponseMessage d2c(DataSourceLoadOptions loaddata, string roleid)
        {
            FetchJioLogs fetchLogs = new FetchJioLogs();
            DateTime date1 = Convert.ToDateTime("01-03-2023");

            var data = clsMeters_Prod.tbl_JioLogs.Where(x => x.DateTime >= date1).OrderByDescending(x => x.DateTime).ToList();
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
                                string syshealth = splitarray[18].Trim(); // Example input string

                                var syshealthchunks = Enumerable.Range(0, (syshealth.Length + chunkSize - 1) / chunkSize)
                                                       .Select(i => syshealth.Substring(i * chunkSize, Math.Min(chunkSize, syshealth.Length - i * chunkSize)));
                                string result = string.Join(",", syshealthchunks);
                                var NB = result.Split(',')[3];
                                var mwtmerge = result.Split(',')[0] + result.Split(',')[1];
                                var CC = result.Split(',')[2];
                                if (roleid == "9")
                                {
                                    if (deserializedProduct.pld == "PALTMaYpNqExrvpg")
                                    {
                                        model.Add(new ModelParameter
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
                                            ValvePosition = splitarray[17].Trim() == "0" ? "Unknown" : splitarray[17].Trim() == "1" ? "SOV Not Present" : splitarray[17].Trim() == "2" ? "SOV Opening" : splitarray[17].Trim() == "3" ? "SOV Closing" : splitarray[17].Trim() == "4" ? "SOV Open" : splitarray[17] == "5" ? "SOV Close" : splitarray[17] == "6" ? "SOV Stuck" : "",
                                            NBIoTRSSI = Convert.ToInt32(NB, 16).ToString(),
                                            ContinuousConsumption = Convert.ToInt32(CC, 16).ToString(),
                                            MWT = Convert.ToInt32(mwtmerge, 16).ToString(),
                                            TransmissionPacket = splitarray[19].Trim(),
                                            Temperature = splitarray[21].Trim(),
                                            TarrifName = splitarray[22].Trim() == "01" ? "Standard" : "Undefined",
                                            GasCalorific = splitarray[20].Trim(),
                                            Checksum = "Valid",//splitarray[23].Trim(),
                                        });
                                    }
                                }
                                else
                                {
                                    model.Add(new ModelParameter
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
                                        ValvePosition = splitarray[17].Trim() == "0" ? "Unknown" : splitarray[17].Trim() == "1" ? "SOV Not Present" : splitarray[17].Trim() == "2" ? "SOV Opening" : splitarray[17].Trim() == "3" ? "SOV Closing" : splitarray[17].Trim() == "4" ? "SOV Open" : splitarray[17] == "5" ? "SOV Close" : splitarray[17] == "6" ? "SOV Stuck" : "",
                                        NBIoTRSSI = Convert.ToInt32(NB, 16).ToString(),
                                        ContinuousConsumption = Convert.ToInt32(CC, 16).ToString(),
                                        MWT = Convert.ToInt32(mwtmerge, 16).ToString(),
                                        TransmissionPacket = splitarray[19].Trim(),
                                        Temperature = splitarray[21].Trim(),
                                        TarrifName = splitarray[22].Trim() == "01" ? "Standard" : "Undefined",
                                        GasCalorific = splitarray[20].Trim(),
                                        Checksum = "Valid",//splitarray[23].Trim(),
                                    });
                                }
                            }
                            else
                            {

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
            if (roleid == "5")
            {
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_VayudutWise_Demo(), loadOptions));
            }
            else
            {
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_VayudutWise().Take(10), loadOptions));
            }
        }
        [Route("LastMeterReading")]
        [HttpGet]
        public HttpResponseMessage LastMeterReading(DateTime fromdate, DateTime todate, DataSourceLoadOptions loadOptions, string roleid, string data)
        {
            if (data == "All" || data == "" || data == null)
            {
                DateTime start = Convert.ToDateTime(fromdate);
                //DateTime end = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-1), INDIAN_ZONE);
                DateTime end = Convert.ToDateTime(todate);
                if (roleid == "5")
                {
                    return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_Demo().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
                }
                else
                {

                    var a = clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate && x.Street == data).Count();
                    return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
                }
            }
            else
            {
                DateTime start = Convert.ToDateTime(fromdate);
                //DateTime end = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(-1), INDIAN_ZONE);
                DateTime end = Convert.ToDateTime(todate);
                if (roleid == "5")
                {
                    return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_Demo().Where(x => x.Date >= fromdate && x.Date <= todate), loadOptions));
                }
                else
                {
                    var a = clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate && x.Street == data).Count();
                    return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_POC().Where(x => x.Date >= fromdate && x.Date <= todate && x.Street == data), loadOptions));
                }

            }
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
                return Request.CreateResponse(DataSourceLoader.Load(data.Where(x => x.PLD == "PALTMaYpNqExrvpg"), loadOptions));
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
                            });
                return Request.CreateResponse(DataSourceLoader.Load(data, loadOptions));
            }

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

    }
}
