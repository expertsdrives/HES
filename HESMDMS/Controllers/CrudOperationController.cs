using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using HESMDMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HESMDMS.Controllers
{

    public class CrudOperationController : ApiController
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
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
                DateTime date1 = Convert.ToDateTime("1-11-2022");
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
                                                                where meta.Street == "Water Lily" || meta.Street == "Dreamland Appartment" || meta.Street == "Richmond Grand"
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
        [Route("d2c")]
        [HttpGet]
        public HttpResponseMessage d2c(DataSourceLoadOptions loaddata)
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
                            model.Add(new ModelParameter
                            {
                                ID = fData.ID,
                                StartingFrame = splitarray[0].Trim(),
                                InstrumentID = splitarray[1].Trim(),
                                Date = splitarray[2],
                                Time = splitarray[3],
                                Record = splitarray[4].Trim(),
                                ActivationStatus = splitarray[5].Trim(),
                                GasCount = splitarray[6].Trim(),
                                MeasurementValue = splitarray[7].Trim(),
                                TotalConsumption = splitarray[8].Trim(),
                                UnitofMeasurement = splitarray[9].Trim(),
                                BatteryVoltage = splitarray[10].Trim(),
                                TamperEvents = splitarray[11].Trim(),
                                AccountBalance = splitarray[12].Trim(),
                                eCreditBalance = splitarray[13].Trim(),
                                StandardCharge = splitarray[14].Trim(),
                                StandardChargeUnit = splitarray[15].Trim(),
                                eCreditonoff = splitarray[16].Trim(),
                                ValvePosition = splitarray[17].Trim(),
                                SystemHealth = splitarray[18].Trim(),
                                TransmissionPacket = splitarray[19].Trim(),
                                Temperature = splitarray[21].Trim(),
                                TarrifName = splitarray[22].Trim(),
                                GasCalorific = splitarray[20].Trim(),
                                Checksum = splitarray[23].Trim(),
                                EndOfFrame = splitarray[24].Trim()
                            });
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
                return Request.CreateResponse(DataSourceLoader.Load(clsMeters.FetchConsumption_VayudutWise(), loadOptions));
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
        [Route("GetAMRHealth")]
        [HttpGet]
        public HttpResponseMessage GetAMRHealth(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.sp_AMRHealth(), loadOptions));
        }

        [Route("SelectSmartMeter")]
        [HttpGet]
        public HttpResponseMessage SelectSmartMeter(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_SMeterMaster, loadOptions));
        }
        [Route("SmartMeterBackLogs")]
        [HttpGet]
        public HttpResponseMessage SmartMeterBackLogs(DataSourceLoadOptions loadOptions) {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters_Prod.tbl_CommandBackLog, loadOptions));
        }
        [Route("MeterFromPLD")]
        [HttpGet]
        public HttpResponseMessage MeterFromPLD(DataSourceLoadOptions loadOptions,string pld)
        {
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_SMeterMaster.Where(x=>x.PLD==pld), loadOptions));
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
            return Request.CreateResponse(DataSourceLoader.Load(clsMeters.tbl_SMeterMaster, loadOptions));
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
