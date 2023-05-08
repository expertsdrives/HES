using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class ModelParameter
    {
        public int ID { get; set; }
        public string StartingFrame { get; set; }
        public string InstrumentID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Record { get; set; }
        public string ActivationStatus { get; set; }
        public string GasCount { get; set; }
        public string MeasurementValue { get; set; }
        public string TotalConsumption { get; set; }
        public string UnitofMeasurement { get; set; }
        public string BatteryVoltage { get; set; }
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
    }
}