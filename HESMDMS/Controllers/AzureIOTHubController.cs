using System;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Client;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using HESMDMS.Models;

namespace HESMDMS.Controllers
{
    public class AzureIOTHubController : ApiController
    {
        private static DeviceClient s_deviceClient;
        private readonly static string s_connectionString01 = "HostName=smartmeters.azure-devices.net;DeviceId=DemoIOTDevice;SharedAccessKey=2yx83iBsYbLGlgdRTsNM5uRgmE79+183XmZmBGjc6IA=";

        [Route("AzureIOT")]
        [HttpPost]
        public async Task<HttpResponseMessage> DataReceptionwithCRC()
        {
            try
            {
                string result = await Request.Content.ReadAsStringAsync();
                Thread thr1 = new Thread(() => fn_insertDataCRC(result));
                thr1.Start();
                return Request.CreateErrorResponse(HttpStatusCode.OK, "OK");
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error Found");
            }
        }
        public async Task fn_insertDataCRC(string result)
        {
            await Task.Run(() =>
            {
                try
                {
                    s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString01, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                    SendDeviceToCloudMessagesAsync(s_deviceClient, result);
                }
                catch (Exception ex)
                {
                  
                }
            });
        }
        private static async void SendDeviceToCloudMessagesAsync(DeviceClient s_deviceClient,string result)
        {
            try
            {

                string[] resultArray = result.Split(new string[] { "}E" }, StringSplitOptions.None);
                string aa = resultArray[0] + "}";
                JObject json = JObject.Parse(aa);
                string json1 = json.ToString(Newtonsoft.Json.Formatting.None);
                var ss = JsonConvert.SerializeObject(json1);
                var Finalresult = JsonConvert.DeserializeObject(ss).ToString();
                var result2 = JsonConvert.DeserializeObject<AMRCRCModel>(Finalresult);
                // var ss = JsonConvert.SerializeObject(json1);
                // Create JSON message
                var telemetryDataPoint = new
                {

                    VAYUDUT_ID = result2.VAYUDUT_ID,
                    V_VOLTAGE = result2.V_VOLTAGE,
                    VAYUDUT_TEMPERATURE = result2.VAYUDUT_TEMPERATURE,
                    MODULE_ERROR = result2.MODULE_ERROR,
                    NW_ERROR = result2.NW_ERROR,
                    POST_ERROR = result2.POST_ERROR,
                    TX_ERRORS = result2.TX_ERRORS,
                    DATA_STRING = result2.DATA_STRING,
                };

                string messageString = "";



                    messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                    var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    // Add a custom application property to the message.
                    // An IoT hub can filter on these properties without access to the message body.
                    //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                    // Send the telemetry message
                    await s_deviceClient.SendEventAsync(message);
                    //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                    //await Task.Delay(1000 * 10);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
