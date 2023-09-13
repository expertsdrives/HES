using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class ModelParameter
    {

        public int ID { get; set; }
        public string InstrumentID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Record { get; set; }
        
        public string MeasurementValue { get; set; }
        public string TotalConsumption { get; set; }
        public decimal BatteryVoltage { get; set; }
        public string TamperEvents { get; set; }
        public string AccountBalance { get; set; }
        public string eCreditBalance { get; set; }
        public string StandardCharge { get; set; }
        public string StandardChargeUnit { get; set; }
        public string eCreditonoff { get; set; }
        public string ValvePosition { get; set; }
        public string SystemHealth { get; set; }
        public string TransmissionPacket { get; set; }
        public string Temperature { get; set; }
        public string TarrifName { get; set; }
        public string GasCalorific { get; set; }
        public string Checksum { get; set; }
        public string EndOfFrame { get; set; }
        public string MagnetTamper { get; set; }
        public string CaseTamper { get; set; }
        public string BatteryRemovalCount { get; set; }
        public string ExcessiveGasFlow { get; set; }
        public string ExcessivePushKey { get; set; }
        public string SOVTamper { get; set; }
        public string TiltTamper { get; set; }
        public string InvalidUserLoginTamper { get; set; }
        public string NBIoTModuleError { get; set; }
        public string NBIoTRSSI { get; set; }
        public string ContinuousConsumption { get; set; }
        public string MWT { get; set; }
        public string DateRx { get; set; }
        public string TimeRx { get; set; }
    }
}