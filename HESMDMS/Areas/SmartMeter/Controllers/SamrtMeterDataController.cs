using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection.Emit;
using Microsoft.AspNet.SignalR.Hosting;
using System.Data.SqlClient;
using Microsoft.Azure.Amqp.Framing;

namespace HESMDMS.Areas.SmartMeter.Controllers
{

    public class SamrtMeterDataController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMeters_Prod = new SmartMeter_ProdEntities();
        ElectricMeterEntities clsElectric = new ElectricMeterEntities();
        // GET: SmartMeter/SamrtMeterData
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Customers()
        {
            return View();
        }

        //public async ActionResult GenerateBillAsync()
        //{
        //    sendBillAsync();
        //    return View();
        //}
        public async Task<ActionResult> sendBillAsync()
        {
            //DateTime toDate = Convert.ToDateTime("2024-02-18");
            //DateTime fromDate = Convert.ToDateTime("2024-02-18");
            //TimeSpan difference = toDate - fromDate;
            //int dateDiff = difference.Days;
            //string pld = "PAO8pXBIiEUSomns";
            //string json = "";

            //var data = clsMeters_Prod.sp_ResponseSplited_Billing(pld, fromDate, toDate).ToList().OrderBy(x => x.Date);

            //foreach (var billingData in data)
            //{
            //    var billingDataList = new List<BillingData>();
            //    var gcv = "10085.968";
            //    var kkcal = Convert.ToDecimal(billingData.TotalConsumptionDifference) * Convert.ToDecimal(10085.968);
            //    var btu = Convert.ToDecimal(kkcal) * Convert.ToDecimal(3.968321);
            //    var mmbtu = Convert.ToDecimal(btu) / Convert.ToDecimal(Math.Pow(10, 6));
            //    var gasAmount = Convert.ToDecimal(billingData.StandardCharge) * mmbtu;
            //    var vatCharges = gasAmount * Convert.ToDecimal(0.15);
            //    var date1 = billingData.Date;
            //    if (date1 != toDate)
            //    {
            //        billingDataList.Add(new BillingData
            //        {
            //            BusinessPartner = billingData.BusinessPatner,
            //            Installation = billingData.Installation,
            //            GASAmount = gasAmount.ToString("#0.#####"),
            //            VATAmount = vatCharges.ToString("#0.#####"),
            //            DiscountAmount = "",
            //            MinimumAmount = "",
            //            CGST_on_MinimumCharge = "",
            //            SGST_on_MinimumCharge = "",
            //            AMCAmount = "",
            //            CGST_on_AMC_Amount = "",
            //            SGST_on_AMC_Amount = "",
            //            TotalAmount = "",
            //            Disconnection_Flag = "",
            //            GCV = "10085.968",
            //            MaterialNumber = "",
            //            MeterNo = "",
            //            ReadingDate = "",
            //            CurrentMeterReading = "",
            //            GASPrice = billingData.StandardCharge,
            //            VATPercentage = "15",
            //            Recharge_Balance = billingData.AccountBalance.ToString(),
            //            FromDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
            //            DueDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
            //            PrePaidSCM_Consumption = billingData.TotalConsumptionDifference.ToString(),
            //            PrePaid_Kcal = kkcal.ToString("#0.#####"),
            //            PrePaid_BTU = btu.ToString("#0.#####"), /*Convert.ToDecimal(Convert.ToDouble(billingData.TotalConsumptionDifference) * 35.314 * 1000 * 1.055).ToString("#0.#####"),*/
            //            PrePaid_MMBTU = mmbtu.ToString("#0.#####"),
            //            PrePaid_GasRate = billingData.StandardCharge
            //        });
            //    }
            //    else
            //    {
            //        billingDataList.Add(new BillingData
            //        {
            //            BusinessPartner = billingData.BusinessPatner,
            //            Installation = billingData.Installation,
            //            GASAmount = gasAmount.ToString("#0.#####"),
            //            VATAmount = vatCharges.ToString("#0.#####"),
            //            DiscountAmount = "",
            //            MinimumAmount = "",
            //            CGST_on_MinimumCharge = "",
            //            SGST_on_MinimumCharge = "",
            //            AMCAmount = "",
            //            CGST_on_AMC_Amount = "",
            //            SGST_on_AMC_Amount = "",
            //            TotalAmount = "",
            //            Disconnection_Flag = "",
            //            GCV = "10085.968",
            //            MaterialNumber = "IS-G1.6PD",
            //            MeterNo = billingData.MeterNumber,
            //            ReadingDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
            //            CurrentMeterReading = billingData.TotalConsumption.ToString(),
            //            GASPrice = billingData.StandardCharge,
            //            VATPercentage = "15",
            //            Recharge_Balance = billingData.AccountBalance.ToString(),
            //            FromDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
            //            DueDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
            //            PrePaidSCM_Consumption = billingData.TotalConsumptionDifference.ToString(),
            //            PrePaid_Kcal = kkcal.ToString("#0.#####"),
            //            PrePaid_BTU = btu.ToString("#0.#####"), /*Convert.ToDecimal(Convert.ToDouble(billingData.TotalConsumptionDifference) * 35.314 * 1000 * 1.055).ToString("#0.#####"),*/
            //            PrePaid_MMBTU = mmbtu.ToString("#0.#####"),
            //            PrePaid_GasRate = billingData.StandardCharge
            //        });
            //    }
            //    json = JsonConvert.SerializeObject(new { BillingData = billingDataList }, Formatting.Indented);
            //    string Url = "https://aipoqdmz.adani.com:443/RESTAdapter/hesmdms/billingdata";
            //    string Username = "INTF_SMARTMETER";
            //    string Password = "Adani@123";

            //    using (HttpClient client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            //            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}")));
            //        var json1 = JsonConvert.SerializeObject(json);
            //        var content = new StringContent(json, Encoding.UTF8, "application/json");
            //        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

            //        var response = await client.PostAsync(Url, content);
            //        if (response.IsSuccessStatusCode)
            //        {
            //            // Handle success

            //        }
            //    }
            //}
            return View();
        }
        public async Task<JsonResult> GenerateBillAsync(string bpnumber, string fromdate, string todate)
        {
            var getCustID = clsMeters.tbl_Customers.Where(x => x.BusinessPatner == bpnumber).FirstOrDefault();
            var custID = getCustID.ID;
            var getPLD = clsMeters_Prod.tbl_AssignSmartMeter.Where(x => x.CustomerInstallationID == custID).FirstOrDefault();
            DateTime toDate = Convert.ToDateTime(todate);
            DateTime fromDate = Convert.ToDateTime(fromdate);
            TimeSpan difference = toDate - fromDate;
            int dateDiff = difference.Days;
            string pld = getPLD.pld;
            string json = "";

            var data = clsMeters_Prod.sp_ResponseSplited_Billing(pld, fromDate, toDate).ToList().OrderBy(x => x.Date);

            foreach (var billingData in data)
            {
                var billingDataList = new List<BillingData>();
                var gcv = "10085.968";
                var kkcal = Convert.ToDecimal(billingData.TotalConsumptionDifference) * Convert.ToDecimal(10085.968);
                var btu = Convert.ToDecimal(kkcal) * Convert.ToDecimal(3.968321);
                var mmbtu = Convert.ToDecimal(btu) / Convert.ToDecimal(Math.Pow(10, 6));
                var gasAmount = Convert.ToDecimal(billingData.StandardCharge) * mmbtu;
                var vatCharges = gasAmount * Convert.ToDecimal(0.15);
                var date1 = billingData.Date;
                if (date1 != toDate)
                {
                    billingDataList.Add(new BillingData
                    {
                        BusinessPartner = billingData.BusinessPatner,
                        Installation = billingData.Installation,
                        GASAmount = gasAmount.ToString("#0.#####"),
                        VATAmount = vatCharges.ToString("#0.#####"),
                        DiscountAmount = "",
                        MinimumAmount = "",
                        CGST_on_MinimumCharge = "",
                        SGST_on_MinimumCharge = "",
                        AMCAmount = "",
                        CGST_on_AMC_Amount = "",
                        SGST_on_AMC_Amount = "",
                        TotalAmount = "",
                        Disconnection_Flag = "",
                        GCV = "10085.968",
                        MaterialNumber = "",
                        MeterNo = "",
                        ReadingDate = "",
                        CurrentMeterReading = "",
                        GASPrice = billingData.StandardCharge,
                        VATPercentage = "15",
                        Recharge_Balance = billingData.AccountBalance.ToString(),
                        FromDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
                        DueDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
                        PrePaidSCM_Consumption = Convert.ToDecimal(billingData.TotalConsumptionDifference).ToString("#0.###"),
                        PrePaid_Kcal = kkcal.ToString("#0.#####"),
                        PrePaid_BTU = btu.ToString("#0.#####"), /*Convert.ToDecimal(Convert.ToDouble(billingData.TotalConsumptionDifference) * 35.314 * 1000 * 1.055).ToString("#0.#####"),*/
                        PrePaid_MMBTU = mmbtu.ToString("#0.#####"),
                        Prepaid_Rent_Amount = billingData.StandardCharge
                    });
                }
                else
                {
                    billingDataList.Add(new BillingData
                    {
                        BusinessPartner = billingData.BusinessPatner,
                        Installation = billingData.Installation,
                        GASAmount = gasAmount.ToString("#0.#####"),
                        VATAmount = vatCharges.ToString("#0.#####"),
                        DiscountAmount = "15",
                        MinimumAmount = "",
                        CGST_on_MinimumCharge = "",
                        SGST_on_MinimumCharge = "",
                        AMCAmount = "",
                        CGST_on_AMC_Amount = "",
                        SGST_on_AMC_Amount = "",
                        TotalAmount = "",
                        Disconnection_Flag = "",
                        GCV = "10085.968",
                        MaterialNumber = "IS-G1.6SMART",
                        MeterNo = billingData.MeterNumber,
                        ReadingDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
                        CurrentMeterReading = billingData.TotalConsumption.ToString(),
                        GASPrice = billingData.StandardCharge,
                        VATPercentage = "15",
                        Recharge_Balance = billingData.AccountBalance.ToString(),
                        FromDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
                        DueDate = billingData.Date.ToString().Replace("-", "").Split(' ')[0],
                        PrePaidSCM_Consumption = Convert.ToDecimal(billingData.TotalConsumptionDifference).ToString("#0.###"),
                        PrePaid_Kcal = kkcal.ToString("#0.#####"),
                        PrePaid_BTU = btu.ToString("#0.#####"), /*Convert.ToDecimal(Convert.ToDouble(billingData.TotalConsumptionDifference) * 35.314 * 1000 * 1.055).ToString("#0.#####"),*/
                        PrePaid_MMBTU = mmbtu.ToString("#0.#####"),
                        Prepaid_Rent_Amount = "",
                        Prepaid_Rent_CGST = "90",
                        Prepaid_Rent_SGST = "90",
                        Installment_Amount = getCustID.Installment_Amount
                    });
                }
                json = JsonConvert.SerializeObject(new { BillingData = billingDataList }, Formatting.Indented);
                string Url = "https://dbcixdp.adani.com:443/RESTAdapter/hesmdms/billingdata";
                string Username = "INTF_SMARTMETER";
                string Password = "Navis@2024";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}")));
                    var json1 = JsonConvert.SerializeObject(json);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    var response = await client.PostAsync(Url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Thread.Sleep(1000);
                        // Handle success

                    }
                }
            }
            return Json(bpnumber, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ElectricMeterView()
        {
            return View();
        }
        public class MeterReading
        {
            public string obisCode { get; set; }
            public string description { get; set; }
            public string value { get; set; }
        }
        string StringToHex(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.AppendFormat("{0:X2}", (int)c); // Convert each char to int, then to hex
            }
            return sb.ToString();
        }
        public async Task<ActionResult> electric()
        {
            var systemtile = "EEP" + 10000078;
            var hextitile = StringToHex(systemtile);
            var apiUrl = "https://electrifyapi.hesmdms.com/api/EnergyMeter/read-data";
            var requestBody = new
            {
                obisCodes = new List<string>
        {
           "1.0.11.7.0.255",
    "1.0.12.7.0.255",
    "0.0.1.0.0.255",
    "1.0.91.7.0.255",
    "1.0.13.7.0.255",
    "1.0.14.7.0.255",
    "1.0.9.7.0.255",
    "1.0.1.7.0.255",
    "1.0.1.8.0.255",
    "1.0.9.8.0.255",
    "1.0.1.6.0.255",
    "1.0.9.6.0.255",
    "0.0.94.91.14.255",
    "0.0.94.91.0.255",
    "0.0.0.1.0.255",
    "0.0.96.2.0.255",
    "1.0.2.8.0.255",
    "0.0.96.3.10.255",
    "0.0.17.0.0.255"
            // Continue adding other OBIS codes...
        },
                host = "2401:4900:9807:35b0:0:0:0:2",
                port = 4059,
                clientAddress = 48,
                serverAddress = 1,
                authentication = 2,
                password = "wwwwwwwwwwwwwwww",
                interfaceType = 1,
                systemTitle = "4545503030303738",
                security = 48,
                blockCipherKey = "62626262626262626262626262626262",
                authenticationKey = "62626262626262626262626262626262",
                invocationCounter = "0.0.43.1.3.255"
            };

            string responseData = await SendPostRequest(apiUrl, requestBody);
            if (responseData != "Error: InternalServerError")
            { 
            
            }
            var readings = JsonConvert.DeserializeObject<List<MeterReading>>(responseData);

            string connectionString = "Server=52.140.120.20,20349; Database=ElectricMeter; User Id=azureadmin; Password=Itouch@2015;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO tbl_MeterData (MeterID, PhaseCurrent, Voltage, Clock, Frequency,CreatedDate,NeutralCurrent,ApparentPower,ActivePower,ActiveEnergy,ApparentEnergy) VALUES (@Column1, @Column2, @Column3, @Column4, @Column5, @Column6, @Column7, @Column8, @Column9, @Column10, @Column11)";
                var phasecurrent = readings[0].value;
                var Voltage = readings[1].value;
                var Clock = readings[2].value;
                var NeutralCurrent = readings[3].value;
                var Frequency = readings[5].value;
                var ApparentPower = readings[6].value;
                var ActivePower = readings[7].value;
                var ActiveEnergy = readings[8].value;
                var ApparentEnergy = readings[9].value;


                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Column1", "1");
                    command.Parameters.AddWithValue("@Column2", phasecurrent);
                    command.Parameters.AddWithValue("@Column3", Voltage);
                    command.Parameters.AddWithValue("@Column4", Clock);
                    command.Parameters.AddWithValue("@Column5", Frequency);
                    command.Parameters.AddWithValue("@Column6", DateTime.Now);
                    command.Parameters.AddWithValue("@Column7", NeutralCurrent);
                    command.Parameters.AddWithValue("@Column8", ApparentPower);
                    command.Parameters.AddWithValue("@Column9", ActivePower);
                    command.Parameters.AddWithValue("@Column10", ActiveEnergy);
                    command.Parameters.AddWithValue("@Column11", ApparentEnergy);

                    command.ExecuteNonQuery();
                }

            }
            //string connectionString = "Server=52.140.120.20,20349; Database=ElectricMeter; User Id=azureadmin; Password=Itouch@2015;";
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    string sql = "INSERT INTO tbl_MeterData (MeterID, PhaseCurrent, Voltage, Frequency, CreatedDate) VALUES (@Column1, @Column2, @Column3, @Column4, @Column5)";

            //    using (SqlCommand command = new SqlCommand(sql, connection))
            //    {
            //        command.Parameters.AddWithValue("@Column1", "1");
            //        command.Parameters.AddWithValue("@Column2", responseData1.Current.ToString());
            //        command.Parameters.AddWithValue("@Column3", responseData1.Voltage.ToString());
            //        command.Parameters.AddWithValue("@Column4", "50");
            //        command.Parameters.AddWithValue("@Column5", DateTime.Now);

            //        command.ExecuteNonQuery();
            //    }
            //}
            return null;
        }
        private async Task<string> SendPostRequest(string url, object jsonData)
        {
            using (var httpClient = new HttpClient())
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {

                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Handle error or non-success response
                    return $"Error: {response.StatusCode}";
                }
            }
        }
        private class ResponseData
        {
            public double Voltage { get; set; }
            public double Current { get; set; }
        }
        public ActionResult EleMeterMaster()
        { 
            return View();
        }
        public async Task<JsonResult> ReConnectMeterAsync(string ipaddress)
        {
            var ip = clsElectric.tbl_MeterMasterRelay.Where(x=>x.SystemTitle==ipaddress).FirstOrDefault();
            var httpClient = new HttpClient();
            var apiUrl = "https://electrifyapi.hesmdms.com/api/EnergyMeter/reconnect";
            var jsonData = new
            {
                host = ip.IPAddress,
                port = 4059,
                clientAddress = 48,
                serverAddress = 1,
                authentication = 2,
                password = "wwwwwwwwwwwwwwww",
                interfaceType = 1,
                systemTitle = "4545503030303738",
                security = 48,
                blockCipherKey = "62626262626262626262626262626262",
                authenticationKey = "62626262626262626262626262626262",
                invocationCounter = "0.0.43.1.3.255"
            };
            string jsonContent = JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Send a POST request with the JSON data
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                // Check the response status
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent successfully!");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return Json("Device Turned On", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Console.WriteLine("Error Code:" + response.StatusCode + " Message: " + response.ReasonPhrase);
                    return Json("Error Try Again", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Exception caught: " + e.Message);
                return Json("Exception", JsonRequestBehavior.AllowGet);
            }
           
        }
        public async Task<JsonResult> DisConnectMeterAsync(string ipaddress)
        {
            var ip = clsElectric.tbl_MeterMasterRelay.Where(x => x.SystemTitle == ipaddress).FirstOrDefault();
            var systemtile = "EEP" + ip.SystemTitle.Substring(ip.SystemTitle.Length - 5);
            var hextitile = StringToHex(systemtile);
            var httpClient = new HttpClient();
            var apiUrl = "https://electrifyapi.hesmdms.com/api/EnergyMeter/disconnect";
            var jsonData = new
            {
                host = ip.IPAddress,
                port = 4059,
                clientAddress = 48,
                serverAddress = 1,
                authentication = 2,
                password = "wwwwwwwwwwwwwwww",
                interfaceType = 1,
                systemTitle = hextitile,
                security = 48,
                blockCipherKey = "62626262626262626262626262626262",
                authenticationKey = "62626262626262626262626262626262",
                invocationCounter = "0.0.43.1.3.255"
            };
            string jsonContent = JsonConvert.SerializeObject(jsonData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Send a POST request with the JSON data
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                // Check the response status
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data sent successfully!");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return Json("Device Turned Off", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Console.WriteLine("Error Code:" + response.StatusCode + " Message: " + response.ReasonPhrase);
                    return Json("Error Try Again", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: " + e.Message);
                return Json("Error Try Again", JsonRequestBehavior.AllowGet);
            }
          
        }
        public ActionResult SGMBLEConnection()
        {
            return View();
        }
    }
}