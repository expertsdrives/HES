using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Security.Cryptography;
using System.Drawing;
using Syncfusion.Lic.util.encoders;

namespace HESMDMS.Models
{
    public static class c2d
    {
        static SmartMeterEntities clsMeters = new SmartMeterEntities();
        static SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        public static async Task<string> SendData(string aid, string pld, string eid, string eventname, string balanceInput, string modetype)
        {

            var actualCount = clsMetersProd.sp_MeterResponse().Where(x => x.pld == pld && x.LogDate == DateTime.UtcNow.Date).Count();
            var command = clsMeters.tbl_OTACommands.Where(x => x.Name == eventname).FirstOrDefault();
            var data = "";
            if (modetype == "api")
            {
                data = balanceInput;
            }
            else
            {
                data = command.Command;
            }
            float intValue = 0;
            Random rnd = new Random();
            double rndNumber = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
            int size = rndNumber.ToString().Length;
            if (size != 12)
            {
                rndNumber = Convert.ToDouble(string.Format("{0}{1}", rndNumber, 0));
            }
            if (modetype != "api")
            {
                if (eventname == "Add Balance" || eventname == "Set Vat")
                {
                    if (eventname.Contains("Set Vat"))
                        intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat) / 100;
                    else
                        intValue = float.Parse(balanceInput, CultureInfo.InvariantCulture.NumberFormat);
                    string balanceString = ToHexString(intValue);
                    var balanceutput = string.Join(",", SplitIntoChunks(balanceString, 2));
                    string modifiedString = data.Replace("-", balanceutput);
                    data = modifiedString;

                }
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
            var logdate = DateTime.Now;
            if (modetype != "auto")
            {
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
                    Thread.Sleep(18000);
                    var responJsonText1 = respon1.Content.ReadAsStringAsync();
                    var st = respon1.StatusCode;

                }
            }
            int start = Convert.ToInt32(command.StartingPostion);
            int end = Convert.ToInt32(command.EndingPostion);
            var hexOutput = getData(pld, command.Code, start, end, logdate, data, eventname, modetype, balanceInput, actualCount);
            //var balanceString = 0.0;
            if (hexOutput != null)
            {
                return hexOutput;
            }
            else
            {
                return "error1";
            }

            return "error2";
        }
        unsafe static string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }
        unsafe static float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
        }
        public static IEnumerable<string> SplitIntoChunks(string input, int chunkSize)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }
        public static string getData(string pld, string code, int startPostion, int endPostion, DateTime logdate, string data, string eventname, string modetype, string balanceInput, int actualCount)
        {

            DateTime curr = DateTime.UtcNow.Date;
            var res = clsMetersProd.sp_MeterResponse().Where(x => x.pld == pld && x.LogDate == DateTime.UtcNow.Date).OrderByDescending(x => x.ID).ToList();
            FetchJioLogs fetchLogs = new FetchJioLogs();
            var LogsData = "";
            foreach (var fData in res)
            {
                try
                {
                    //if (fData.Data.Contains(code))
                    //{
                        //    DateTime resDate = Convert.ToDateTime(fData.Data.Split(',')[1] + " " + fData.Data.Split(',')[2]);
                        //    resDate = Convert.ToDateTime(fData.LogDate);
                        //    if (logdate.Kind != DateTimeKind.Utc)
                        //        logdate = logdate.AddHours(-5).AddMinutes(-30);

                        //    if (resDate >= logdate)
                        //    {
                        
                            if (res.Count > actualCount)
                            {
                                LogsData = fData.Data;
                                break;
                            }
                        
                    
                        //    }
                    //}
                }
                catch (Exception ex) { }
            }
            string hexOutput = "";
            if (LogsData != "")
            {
                string[] splitoutput = Convert.ToString(LogsData).Split(',');
                for (int i = startPostion + 3; i <= endPostion + 3; i++)
                {
                    hexOutput += splitoutput[i];
                }
                var Smeter = clsMeters.tbl_SMeterMaster.Where(x => x.PLD == pld).FirstOrDefault();
                bool auditLog = SGMAuditLogs(pld, data, Smeter.MeterID, eventname);
            }
            else
            {
                hexOutput = "error3";
                if (modetype == "auto")
                {
                    var cData = clsMetersProd.tbl_CommandBackLog;
                    tbl_CommandBackLog clsCmd = new tbl_CommandBackLog();
                    clsCmd.Data = data;
                    clsCmd.EventName = eventname;
                    clsCmd.LogDate = DateTime.UtcNow;
                    clsCmd.Status = "Pending";
                    clsCmd.pld = pld;
                    clsMetersProd.tbl_CommandBackLog.Add(clsCmd);
                    clsMetersProd.SaveChanges();
                }
            }
            return hexOutput;
        }
        public static async Task<bool> checkManualMode(string data, string aid, string pld, string eid)
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
                var logdate = DateTime.Now;
                var objClint1 = new System.Net.Http.HttpClient();
                objClint1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objClint1.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken.access_token);
                objClint1.DefaultRequestHeaders.Add("eid", eid);
                System.Net.Http.HttpResponseMessage respon1 = await objClint1.PostAsync(requestUri1, new StringContent(content, System.Text.Encoding.UTF8, "application/json"));
                var responJsonText1 = respon1.Content.ReadAsStringAsync();
                var res = responJsonText1.Result.ToString();
                if (res.Contains("MESSAGE_DELIVERED_PREFERED_CHANNEL"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool SGMAuditLogs(string pld, string data, double? meterID, string eventname)
        {
            tbl_SGMAuditLogs SGMAuditLogs = new tbl_SGMAuditLogs();
            SGMAuditLogs.pld = pld;
            SGMAuditLogs.Data = data;
            SGMAuditLogs.MeterID = Convert.ToString(meterID);
            SGMAuditLogs.EventName = eventname;
            SGMAuditLogs.LogDate = DateTime.UtcNow;
            clsMetersProd.tbl_SGMAuditLogs.Add(SGMAuditLogs);
            clsMetersProd.SaveChanges();
            return true;
        }
        public static string getvalue(string data)
        {
            string hexOutPut = "";
            string[] splitoutput = Convert.ToString(data).Split(',');
            for (int i = 5; i <= 8; i++)
            {
                hexOutPut += splitoutput[i].Trim();
            }
            uint num = uint.Parse(hexOutPut, System.Globalization.NumberStyles.AllowHexSpecifier);

            byte[] floatBytes = BitConverter.GetBytes(num);
            float floatNum = BitConverter.ToSingle(floatBytes, 0);
            return "Rs. " + floatNum.ToString("F2");
        }


    }

}