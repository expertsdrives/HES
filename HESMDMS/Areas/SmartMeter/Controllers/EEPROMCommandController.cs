using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HESMDMS.Areas.SmartMeter.Controllers
{
    public class EEPROMCommandController : Controller
    {
        // Best Practice: Use more descriptive names for context instances.
        // Dependency Injection is recommended for managing DbContext lifetime.
        private readonly SmartMeter_ProdEntities _dbContext = new SmartMeter_ProdEntities();
        private readonly SmartMeter_ProdEntities1 _dbContext1 = new SmartMeter_ProdEntities1();

        // GET: SmartMeter/EEPROMCommand
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets a list of meters for DevExtreme data grid.
        /// </summary>
        /// <param name="loadOptions">DevExtreme data loading options.</param>
        /// <returns>A JSON result containing the meter data.</returns>
        public async Task<ActionResult> GetMeters(DataSourceLoadOptions loadOptions)
        {
            try
            {
                // Performance Optimization:
                // The query is now an IQueryable. DataSourceLoader will apply filtering, sorting,
                // and paging directly at the database level, preventing the entire table from being loaded into memory.
                // Readability & Maintainability:
                // Projecting to a specific view model or an anonymous type with clear property names improves clarity.
                var meterQuery = _dbContext.tbl_SMeterMaster
                                           .Select(m => new { m.MeterSerialNumber });

                // Best Practice: Use async operations for I/O-bound tasks like database queries
                // to improve application scalability and responsiveness.
                var loadResult = await DataSourceLoader.LoadAsync(meterQuery, loadOptions);

                return Content(JsonConvert.SerializeObject(loadResult), "application/json");
            }
            catch (Exception)
            {
                // Error Handling:
                // Catch potential exceptions during data retrieval.
                // It's good practice to log the exception details for debugging purposes.
                // For example: log.Error("Failed to get meters", ex);

                // Return a clear error response to the client.
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpPost]
        public async Task<JsonResult> ReadCommand(string command, string meter)
        {
            // Data Validation: Ensure input parameters are not null or empty.
            if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(meter))
            {
                return Json(new { success = false, message = "Command and meter cannot be empty." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Asynchronous Query: Use async methods for database calls to improve performance.
                var commandDetails = await _dbContext1.tbl_EPROMCommand
                                                      .FirstOrDefaultAsync(x => x.CommandName == command);

                // Null Check: Verify that the command exists.
                if (commandDetails == null)
                {
                    return Json(new { success = false, message = $"Command '{command}' not found." }, JsonRequestBehavior.AllowGet);
                }

                var meterDetails = await _dbContext.tbl_SMeterMaster
                                                   .FirstOrDefaultAsync(x => x.MeterSerialNumber == meter);

                // Null Check: Verify that the meter exists.
                if (meterDetails == null)
                {
                    return Json(new { success = false, message = $"Meter '{meter}' not found." }, JsonRequestBehavior.AllowGet);
                }

                // Readability: Use clear variable names.
                var commandString = $"{commandDetails.CommandHeader},{commandDetails.Address},{commandDetails.Length},{commandDetails.CommandFotter}";

                // Object Initialization: Use object initializer for cleaner code.
                var commandLog = new tbl_CommandBackLog
                {
                    pld = meterDetails.PLD,
                    Data = commandString,
                    EventName = command,
                    Status = "Pending",
                    LogDate = DateTime.Now
                };

                _dbContext.tbl_CommandBackLog.Add(commandLog);
                await _dbContext.SaveChangesAsync(); // Asynchronous Save

                return Json(new { success = true, message = $"Read command '{command}' executed for meter '{meter}'." });
            }
            catch (Exception)
            {
                // Error Handling: Log the exception and return a generic error message.
                // For example: log.Error("Error in ReadCommand", ex);
                return Json(new { success = false, message = "An internal error occurred." }, JsonRequestBehavior.AllowGet);
            }
        }
        private static unsafe float FromHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            return *((float*)&i);
        }
        private static unsafe string ToHexString(float f)
        {
            var i = *((int*)&f);
            return i.ToString("X8");
        }
        [HttpPost]
        public async Task<JsonResult> WriteReadCommand(string command, string value, string meter)
        {
            // Data Validation: Ensure input parameters are not null or empty.
            if (string.IsNullOrWhiteSpace(command) || string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(meter))
            {
                return Json(new { success = false, message = "Command, value, and meter cannot be empty." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var commandDetails = await _dbContext1.tbl_EPROMCommand
                                                      .FirstOrDefaultAsync(x => x.CommandName == command);

                if (commandDetails == null)
                {
                    return Json(new { success = false, message = $"Command '{command}' not found." }, JsonRequestBehavior.AllowGet);
                }

                var meterDetails = await _dbContext.tbl_SMeterMaster
                                                   .FirstOrDefaultAsync(x => x.MeterSerialNumber == meter);

                if (meterDetails == null)
                {
                    return Json(new { success = false, message = $"Meter '{meter}' not found." }, JsonRequestBehavior.AllowGet);
                }

                // Step 1: Validate and parse the input 'value' as an integer.
                if (!int.TryParse(value, out int numericValue))
                {
                    return Json(new { success = false, message = "Invalid numeric value provided." }, JsonRequestBehavior.AllowGet);
                }
                string hexValue = "";
                if (command.Contains("Tariff"))
                {
                    float parsedBalance = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                    hexValue = ToHexString(parsedBalance);
                }
                else
                { 
                    hexValue=numericValue.ToString("X");
                }
                // Step 2: Convert the integer to a hexadecimal string.
                

                // Step 3: Pad the hex string to ensure it has an even number of characters.
                // This is standard for hex representations of data.
                if (hexValue.Length % 2 != 0)
                {
                    hexValue = "0" + hexValue;
                }

                // Step 4: Calculate the initial length in bytes.
                int byteLength = hexValue.Length / 2;

                // Step 5: Compare with expected length and pad with "00" bytes if necessary.
                if (int.TryParse(commandDetails.Length, out int expectedLength) && byteLength < expectedLength)
                {
                    int diff = expectedLength - byteLength;
                    string padding = string.Concat(Enumerable.Repeat("00", diff));
                    hexValue = padding + hexValue;
                }

                if (commandDetails.ArrayType?.ToLower() == "lsb")
                {
                    hexValue = ReverseHex(hexValue);
                }

 
                 // Step 6: Format the final hex string with commas.
                 hexValue = string.Join(",", Enumerable.Range(0, hexValue.Length / 2)
                                                      .Select(i => hexValue.Substring(i * 2, 2)));
 
                 // Step 7: Calculate the total length based on the new requirement.
                // Length = length of Address + length of Length field (1 byte) + length of data payload
                int addressLengthInBytes = (commandDetails.Address?.Length ?? 0) / 2;
                int dataPayloadLengthInBytes = hexValue.Split(',').Length;
                int totalLength = addressLengthInBytes + 1 + dataPayloadLengthInBytes;
                string hexLength = totalLength.ToString("X2");

                // Step 8: Construct the final command string.
                commandDetails.CommandHeader = commandDetails.CommandHeader.Replace("-", hexLength);
                commandDetails.CommandFotter =  hexValue;
                var commandString = $"{commandDetails.CommandHeader},{commandDetails.Address},{commandDetails.Length},{commandDetails.CommandFotter},03";

                var commandLog = new tbl_CommandBackLog
                {
                    pld = meterDetails.PLD,
                    Data = commandString,
                    EventName = command,
                    Status = "Pending",
                    LogDate = DateTime.Now
                };

                _dbContext.tbl_CommandBackLog.Add(commandLog);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true, message = $"Write-Read command '{command}' with value '{value}' executed for meter '{meter}'." });
            }
            catch (Exception)
            {
                // Error Handling: Log the exception for debugging.
                // For example: log.Error("Error in WriteReadCommand", ex);
                return Json(new { success = false, message = "An internal error occurred." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckPld(string meterSerialNumber)
        {
            var meter = _dbContext.tbl_SMeterMaster.FirstOrDefault(m => m.MeterSerialNumber == meterSerialNumber);
            if (meter != null && !string.IsNullOrEmpty(meter.PLD))
            {
                return Json(new { success = true, hasPld = true });
            }
            return Json(new { success = true, hasPld = false });
        }

        [HttpGet]
        public async Task<JsonResult> GetEpromCommands()
        {
            try
            {
                var commands = await _dbContext1.tbl_EPROMCommand
                                                .Select(c => new { c.CommandName })
                                                .ToListAsync();
                return Json(new { success = true, data = commands }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // log.Error("Failed to get EPROM commands", ex);
                return Json(new { success = false, message = "An error occurred while fetching commands." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetLatestResponse(string commandName, string meterSerialNumber)
        {
            if (string.IsNullOrWhiteSpace(commandName) || string.IsNullOrWhiteSpace(meterSerialNumber))
            {
                return Json(new { success = false, message = "Command name and meter serial number cannot be empty." });
            }

            try
            {
                var command = await _dbContext1.tbl_EPROMCommand.FirstOrDefaultAsync(c => c.CommandName == commandName);
                if (command == null || string.IsNullOrWhiteSpace(command.Address))
                {
                    return Json(new { success = false, message = "Command or address not found." });
                }

                var meter = await _dbContext.tbl_SMeterMaster.FirstOrDefaultAsync(m => m.MeterSerialNumber == meterSerialNumber);
                if (meter == null || string.IsNullOrWhiteSpace(meter.PLD))
                {
                    return Json(new { success = false, message = "Meter or PLD not found." });
                }

                var responseLog = await _dbContext.tbl_Response
                                            .Where(r => r.pld == meter.PLD && r.Data.Contains(command.Address) && r.Data.Contains("E9"))
                                            .OrderByDescending(r => r.LogDate)
                                            .FirstOrDefaultAsync();

                if (responseLog != null)
                {
                    var responseData = responseLog.Data.Replace(",", "");
                    var address = command.Address.Replace(",", "");
                    var addressIndex = responseData.IndexOf(address);
                    if (addressIndex != -1)
                    {
                        if (int.TryParse(command.Length, out int length))
                        {
                            var startIndex = addressIndex + address.Length + 2; // Skip 1 byte (2 hex characters)
                            if (responseData.Length >= startIndex + length * 2)
                            {
                                var extractedData = responseData.Substring(startIndex, length * 2);
                                if (command.ArrayType?.ToLower() == "lsb")
                                {
                                    extractedData = ReverseHex(extractedData);
                                }
                                var convertedData = ConvertHexTo(extractedData, command.DataType);
                                return Json(new { success = true, data = convertedData });
                            }
                        }
                    }
                    return Json(new { success = true, data = responseLog.Data });
                }
                else
                {
                    return Json(new { success = false, message = "No matching response found." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An internal error occurred." });
            }
        }

        private string ConvertHexTo(string hexValue, string dataType)
        {
            try
            {
                switch (dataType.ToLower())
                {
                    case "int":
                        return int.Parse(hexValue, NumberStyles.HexNumber).ToString();
                    case "float":
                        uint num = uint.Parse(hexValue, NumberStyles.HexNumber);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        return BitConverter.ToSingle(floatVals, 0).ToString();
                    case "string":
                        byte[] bytes = Enumerable.Range(0, hexValue.Length)
                                                 .Where(x => x % 2 == 0)
                                                 .Select(x => Convert.ToByte(hexValue.Substring(x, 2), 16))
                                                 .ToArray();
                        return System.Text.Encoding.ASCII.GetString(bytes);
                    case "decimal":
                        return long.Parse(hexValue, NumberStyles.HexNumber).ToString();
                    case "datetime":
                        long ticks = long.Parse(hexValue, NumberStyles.HexNumber);
                        return DateTimeOffset.FromUnixTimeSeconds(ticks).LocalDateTime.ToString();
                    default:
                        return hexValue;
                }
            }
            catch
            {
                return hexValue; // Return original hex if conversion fails
            }
        }

        private string ReverseHex(string hex)
        {
            return string.Concat(Enumerable.Range(0, hex.Length / 2).Select(i => hex.Substring(hex.Length - (i + 1) * 2, 2)));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
                _dbContext1.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}