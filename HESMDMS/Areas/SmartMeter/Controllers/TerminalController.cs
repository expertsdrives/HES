using HESMDMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class TerminalController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        private readonly IHttpClientFactory _clientFactory;
        // GET: SmartMeter/Terminal
        public ActionResult Index()
        {

            var data = clsMeters.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();
        }
        public async Task<ActionResult> SendToAPIAsync(string aid, string pld, string data, string eid)
        {
            Random rnd = new Random();
            double rndNumber = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
            int size = rndNumber.ToString().Length;
            if (size != 12)
            {
                rndNumber = Convert.ToDouble(string.Format("{0}{1}", rndNumber, 0));
            }
            var bob = new
            {
                idType = "PLD",
                id = pld,
                transactionId = rndNumber.ToString(),
                retentionTime = DateTime.Now.ToString(),
                data = new[] {
                 new  { aid =aid, dataformat = "cp",dataType="JSON",
                 ext="{'data': '"+data+"'}"
                 },

            }
            };
            //APIData apiData = new APIData()
            //{
            //    idType = "PLD",
            //    id = pld,
            //    transactionId = rndNumber.ToString(),
            //    retentionTime = DateTime.Now.ToString(),
            //    data = ""
            //};
            string stringjson = JsonConvert.SerializeObject(bob);
            Uri requestUri = new Uri("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login"); //replace your Url  
            var converter = new ExpandoObjectConverter();
            c2dProd users = new c2dProd();
            users.grant_type = "password";
            users.username = "2025000_2890001@iot.jio.com";
            users.password = "a737b902951ec15cff735357a850b09cd941818095527a1925760b5a4e471464";
            users.client_id = "db2f04a5e72547cbb68331f406946494";
            users.client_secret = "d95578aa9b1eb30e";
            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            var objClint = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage respon = await objClint.PostAsync(requestUri, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            string responJsonText = await respon.Content.ReadAsStringAsync();
            var bearerToken = JsonConvert.DeserializeObject<c2dTokenProd>(responJsonText);

            Console.WriteLine(bearerToken);
            if (Convert.ToString(bearerToken.access_token) != null)
            {
                Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified"); //replace your Url  

                var content = JsonConvert.SerializeObject(bob);

                var objClint1 = new System.Net.Http.HttpClient();
                objClint1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objClint1.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.access_token);
                objClint1.DefaultRequestHeaders.Add("eid", eid);
                System.Net.Http.HttpResponseMessage respon1 = await objClint1.PostAsync(requestUri1, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                var responJsonText1 = respon1.Content.ReadAsStringAsync();
                MyHub.SendMessages();
                return Json(responJsonText1.Result.ToString(), JsonRequestBehavior.AllowGet);

            }
            return Json(responJsonText, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMessages(string pld)
        {
            var PreviousData = "";
            var res = clsMetersProd.tbl_JioLogs.OrderByDescending(x => x.ID).Select(x => x.SMTPLResponse).ToList();

            FetchJioLogs fetchLogs = new FetchJioLogs();
            var LogsData = "";

            foreach (var fData in res)
            {
                if (IsValidJson(fData.ToString()))
                {
                    FetchJioLogs deserializedProduct = JsonConvert.DeserializeObject<FetchJioLogs>(fData);

                    List<ModelParameter> model = new List<ModelParameter>();
                    if (deserializedProduct.pld == pld)
                    {
                        if (PreviousData != deserializedProduct.Data)
                        {
                            PreviousData = deserializedProduct.Data;
                        }

                        LogsData = deserializedProduct.Data;
                        break;
                    }
                }
            }

            return Json(LogsData, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Demo()
        {
            return View();
        }
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
    }

    public class Response
    {
        public string Result { get; set; }
    }

}