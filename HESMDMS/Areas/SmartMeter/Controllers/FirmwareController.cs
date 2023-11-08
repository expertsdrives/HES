using HESMDMS.Models;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Data.Entity;
using Microsoft.Ajax.Utilities;
using DotNetty.Codecs.Mqtt.Packets;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class FirmwareController : Controller
    {
        SmartMeterEntities clsMeters = new SmartMeterEntities();
        SmartMeter_ProdEntities clsMetersProd = new SmartMeter_ProdEntities();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        // GET: SmartMeter/Firware
        public ActionResult Index()
        {
            var data = clsMetersProd.tbl_SMeterMaster.ToList();
            ViewBag.data = data;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> IndexData(HttpPostedFileBase file, string MeterID, string Packetsize)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            var backgroundWorker = new BackgroundWorker();
            string fileContent = "";
            int packetSize = Convert.ToInt32(Request.Form["packetSize"]);
            var meterID = MeterID;
            var pld = clsMetersProd.tbl_SMeterMaster.Where(x => x.TempMeterID == meterID).FirstOrDefault();
            int TotalPacket = 0;
            if (fileExtension == ".hex")
            {
                if (file != null && file.ContentLength > 0)
                {

                    var inputStream = file.InputStream;
                    using (var reader = new StreamReader(inputStream))
                    {
                        // Read the content of the file
                        fileContent = reader.ReadToEnd().Replace("\r\n", "");
                        int chunkSize = packetSize;
                        for (int i = chunkSize; i < fileContent.Length; i += chunkSize + 2) // +2 to account for inserted characters
                        {
                            fileContent = fileContent.Insert(i, "\r\n");
                        }

                    }
                    int counti = 1;
                    Task backgroundTask = Task.Run(async () => await insertData(fileContent, "52", pld.PLD, counti, TotalPacket, pld.AID, pld.EID.ToString(), MeterID));
                 

                }
            }
            if (fileExtension == ".bin")
            {
                string hexString = ConvertBinaryFileToHexString(file, packetSize);

              
                int counti = 1;
                Task backgroundTask = Task.Run(async () => await insertData(hexString, "52", pld.PLD, counti, TotalPacket, pld.AID, pld.EID.ToString(), MeterID));
                
                // Now, 'fileData' contains the binary data from the uploaded file
                // You can work with 'fileData' as needed

            }
            return Json(new { status = "completed" });
        }
        //[HttpPost]
        //public async Task<ActionResult> FirmwareFileComparision(HttpPostedFileBase file1, HttpPostedFileBase file2, string MeterID, string Packetsize)
        //{
        //    int packetSize = Convert.ToInt32(Request.Form["packetSize"]);
        //    double? meterID = Convert.ToDouble(MeterID);
        //    var pld = clsMeters.tbl_SMeterMaster.Where(x => x.MeterID == meterID).FirstOrDefault();
        //    int TotalPacket = 0;
        //    string fileExtension = Path.GetExtension(file1.FileName);
        //    string uploadDirectory = Server.MapPath("~/Uploads");
        //    string fileName1 = Guid.NewGuid().ToString() + Path.GetExtension(file1.FileName);
        //    string fileName2 = Guid.NewGuid().ToString() + Path.GetExtension(file2.FileName);
        //    // Ensure the directory exists, create it if not
        //    if (!Directory.Exists(uploadDirectory))
        //    {
        //        Directory.CreateDirectory(uploadDirectory);
        //    }
        //    string filePath1 = Path.Combine(uploadDirectory, fileName1);
        //    string filePath2 = Path.Combine(uploadDirectory, fileName2);

        //    // Save the file
        //    file1.SaveAs(filePath1);
        //    file2.SaveAs(filePath2);
        //    string file1Content = System.IO.File.ReadAllText(filePath1);
        //    string file2Content = System.IO.File.ReadAllText(filePath2);
        //    StringBuilder updatedData = new StringBuilder();
        //    // Step 2: Compare Files
        //    if (file1Content.Equals(file2Content))
        //    {
        //        Console.WriteLine("Files are identical.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Files are different.");

        //        // Step 3: Identify Differences
        //        // This simple example assumes that the files are text-based, and it prints the differing lines.
        //        string[] lines1 = file1Content.Split('\n');
        //        string[] lines2 = file2Content.Split('\n');

        //        for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
        //        {
        //            if (lines1[i] != lines2[i])
        //            {
        //                updatedData.AppendLine(lines2[i]);
        //                updatedData.AppendLine("");
        //            }
        //        }
        //        string updatedDataString = updatedData.ToString();
        //        Task backgroundTask = Task.Run(async () => await insertData(packetSize, updatedDataString, TotalPacket, pld.PLD, pld.AID, pld.EID.ToString(), MeterID.ToString(), cancellationTokenSource.Token, "51"));
        //        // Additional logic can be added based on your specific needs.
        //    }

        //    return Json(new { status = "completed" });
        //}
        public static ushort CalculateCRC16(string input)
        {
            ushort crc = 0xFFFF;

            foreach (char c in input)
            {
                crc ^= (ushort)(c << 8);

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }

            return crc;
        }
        public string ConvertBinaryFileToHexString(HttpPostedFileBase file, int PacketSize)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (Stream stream = file.InputStream)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);

                            // Convert the byte array to a hex string
                            string hexString = BitConverter.ToString(memoryStream.ToArray()).Replace("-", "");
                            int chunkSize = PacketSize;
                            for (int i = chunkSize; i < hexString.Length; i += chunkSize + 2) // +2 to account for inserted characters
                            {
                                hexString = hexString.Insert(i, "\r\n");
                            }

                            return hexString;
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid file or file is empty.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public JsonResult CheckStatus(string pld)
        {
            var status = clsMetersProd.tbl_CommandBackLog.Where(x => x.pld == pld && x.EventName == "Firmware Upgrade").OrderByDescending(x => x.ID).FirstOrDefault();
            return Json(status.Status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetData(string pld)
        {
            var data = clsMetersProd.tbl_FirmwareHistoty.Where(x => x.pld == pld).OrderByDescending(x => x.ID).ToList(); // Replace with your entity name

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task insertData(string data1, string cmd, string pld, int counti, int TotalPacket, string AID, string EID, string MeterID)
        {
            try
            {

                string[] splitsize = data1.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
                TotalPacket = splitsize.Length;
                tbl_FirmwareStatus fir = new tbl_FirmwareStatus();
                fir.pld = pld;
                fir.Status = "Started";
                fir.LogDate = DateTime.UtcNow;
                clsMetersProd.tbl_FirmwareStatus.Add(fir);
                clsMetersProd.SaveChanges();
                int counter = 0;
                foreach (var data in splitsize)
                {
                    counter++;
                  
                    var chcStatus = clsMetersProd.tbl_FirmwareStatus.Where(x => x.Status == "Aborted" && x.pld == pld).OrderByDescending(x => x.ID).Count();
                    if (chcStatus == 0)
                    {
                        string PacketChunks = data.ToString().Replace("\r\n", "");
                        ushort checksum = CalculateCRC16(PacketChunks);
                        Random rnd1 = new Random();
                        double rndNumber1 = Convert.ToDouble(DateTime.Now.ToString("ddMMyyHHmmsss"));
                        int size1 = rndNumber1.ToString().Length;
                        if (size1 != 12)
                        {
                            rndNumber1 = Convert.ToDouble(string.Format("{0}{1}", rndNumber1, 0));
                        }
                        List<string> substrings = new List<string>();

                        for (int i = 0; i < rndNumber1.ToString().Length; i += 2)
                        {
                            int length = Math.Min(2, rndNumber1.ToString().Length - i);
                            substrings.Add(rndNumber1.ToString().Substring(i, length));
                        }
                        var tid = string.Join("", substrings);
                        byte[] byteArray = BitConverter.GetBytes(PacketChunks.Length);
                        byte lsb = byteArray[0]; // LSB is the first byte (index 0).
                        byte msb = byteArray[1]; // MSB is the second byte (index 1).
                        string hexString = $"{msb:X2},{lsb:X2}";

                        byte[] byteArrayCount = BitConverter.GetBytes(counti);
                        byte lsbCount = byteArrayCount[0]; // LSB is the first byte (index 0).
                        byte msbCount = byteArrayCount[1]; // MSB is the second byte (index 1).

                        byte[] byteArrayPacket = BitConverter.GetBytes(TotalPacket);
                        byte lsbPacket = byteArrayPacket[0]; // LSB is the first byte (index 0).
                        byte msbPacket = byteArrayPacket[1]; // MSB is the second byte (index 1).

                        byte[] byteArrayCheckSum = BitConverter.GetBytes(checksum);
                        byte lsbCheckSum = byteArrayCheckSum[0]; // LSB is the first byte (index 0).
                        byte msbCheckSum = byteArrayCheckSum[1]; // MSB is the second byte (index 1).

                        string command = "02,03," + hexString + $",{cmd},{msbCount:X2}{lsbCount:X2},{msbPacket:X2}{lsbPacket:X2}," + PacketChunks + $",{msbCheckSum:X2}{lsbCheckSum:X2},03";
                        command = "?" + command + "##," + tid.ToString() + "!";
                        var bob1 = new
                        {
                            idType = "PLD",
                            id = pld,
                            transactionId = rndNumber1.ToString(),
                            retentionTime = DateTime.Now.ToString(),
                            data = new[] {
                 new  { aid =AID, dataformat = "cp",dataType="JSON",
                 ext="{'data': '"+command+"'}"
                 },

            }
                        };
                        var content1 = JsonConvert.SerializeObject(bob1);
                        var objClint2 = new System.Net.Http.HttpClient();
                        Uri requestUri1 = new Uri("https://com.api.cats.jvts.net:8082/auth-engine/v2.2/login"); //replace your Url  
                        var converter1 = new ExpandoObjectConverter();
                        c2dProd users1 = new c2dProd();
                        users1.grant_type = "password";
                        users1.username = "2025000_2890001@iot.jio.com";
                        users1.password = "a737b902951ec15cff735357a850b09cd941818095527a1925760b5a4e471464";
                        users1.client_id = "db2f04a5e72547cbb68331f406946494";
                        users1.client_secret = "d95578aa9b1eb30e";
                        string json1 = "";
                        json1 = Newtonsoft.Json.JsonConvert.SerializeObject(users1);
                        var objClint3 = new System.Net.Http.HttpClient();
                        System.Net.Http.HttpResponseMessage respon3 = await objClint3.PostAsync(requestUri1, new StringContent(json1, System.Text.Encoding.UTF8, "application/json"));
                        string responJsonText1 = await respon3.Content.ReadAsStringAsync();
                        var bearerToken1 = JsonConvert.DeserializeObject<c2dTokenProd>(responJsonText1);
                        var st = "";
                        Console.WriteLine(bearerToken1);
                        if (Convert.ToString(bearerToken1.access_token) != null)
                        {
                            Uri requestUri2 = new Uri("https://com.api.cats.jvts.net:8080/c2d-services/iot/jioutils/v2/c2d-message/unified"); //replace your Url  
                            objClint2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            objClint2.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken1.access_token);
                            objClint2.DefaultRequestHeaders.Add("eid", EID.ToString());
                            System.Net.Http.HttpResponseMessage respon1 = await objClint2.PostAsync(requestUri2, new StringContent(content1, System.Text.Encoding.UTF8, "application/json"));
                            //Thread.Sleep(10000);
                            var responJsonText2 = respon1.Content.ReadAsStringAsync();
                            st = respon1.StatusCode.ToString();

                        }
                        tbl_FirmwareHistoty cmdBack1 = new tbl_FirmwareHistoty();
                        cmdBack1.MeterID = MeterID.ToString();
                        cmdBack1.pld = pld;
                        cmdBack1.PacketNumber = counti.ToString();
                        cmdBack1.PacketData = command;
                        cmdBack1.pld = pld;
                        cmdBack1.CreatedDate = DateTime.Now;
                        cmdBack1.JioResponse = st;
                        clsMetersProd.tbl_FirmwareHistoty.Add(cmdBack1);
                        clsMetersProd.SaveChanges();
                        counti++;
                        Thread.Sleep(1000);

                    }
                    else
                    {
                        var existingEntity1 = clsMetersProd.tbl_FirmwareStatus.Where(x => x.pld == pld && x.Status == "Aborted").FirstOrDefault();

                        if (existingEntity1 != null)
                        {
                            // Update the properties of the existing entity.
                            existingEntity1.Status = "Completed";

                            // Update the entity in the database.
                            clsMetersProd.Entry(existingEntity1).State = EntityState.Modified;
                            clsMetersProd.SaveChanges();
                        }
                        break;
                    }

                }

               



            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Background: Foreach loop was canceled.");
            }
        }
        public JsonResult StopBackgroundFunction(string pld)
        {
            var existingEntity1 = clsMetersProd.tbl_FirmwareStatus.Where(x => x.pld == pld && x.Status == "Started").FirstOrDefault();

            if (existingEntity1 != null)
            {
                // Update the properties of the existing entity.
                existingEntity1.Status = "Aborted";

                // Update the entity in the database.
                clsMetersProd.Entry(existingEntity1).State = EntityState.Modified;
                clsMetersProd.SaveChanges();
            }
            return Json(new { status = "completed" });
        }
    }
}