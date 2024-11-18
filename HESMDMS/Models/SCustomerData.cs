using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class SCustomerData
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public Nullable<System.TimeSpan> LogTime { get; set; }
        public string pld { get; set; }
        public string aid { get; set; }
        public string eid { get; set; }
        public string StartingFrame { get; set; }
        public string InstrumentID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Time { get; set; }
        public string ActivationStatus { get; set; }
        public string GasCount { get; set; }
        public string MeasurementValue { get; set; }
        public Nullable<decimal> TotalConsumption { get; set; }
        public string UnitofMeasurement { get; set; }
        public string BatteryVoltage { get; set; }
        public string TamperEvents { get; set; }
        public Nullable<decimal> AccountBalance { get; set; }
        public string eCreditBalance { get; set; }
        public string StandardCharge { get; set; }
        public string StandardChargeUnit { get; set; }
        public string eCreditonoff { get; set; }
        public string ValvePosition { get; set; }
        public string SystemHealth { get; set; }
        public string TransmissionPacket { get; set; }
        public string GasCalorific { get; set; }
        public string Temperature { get; set; }
        public string TarrifName { get; set; }
        public string Checksum { get; set; }
        public string EndOfFrame { get; set; }
        public decimal previous_value { get; set; }
        public decimal ActualConsumption { get; set; }
        public string FullName { get; set; }
    }
}