//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HESMDMS
{
    using System;
    
    public partial class sp_FetchConsumptionData_Result
    {
        public Nullable<long> BusinessPartnerNo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> OpeningMeterReading { get; set; }
        public Nullable<long> DataCount { get; set; }
        public Nullable<decimal> OpeningAMRReading { get; set; }
        public Nullable<decimal> ReadingCount { get; set; }
        public string FullName { get; set; }
        public string MeterNumber { get; set; }
        public string ContractAcct { get; set; }
        public string ConnectionObject { get; set; }
        public string ChargeAreaNameZoneName { get; set; }
        public Nullable<int> TXID { get; set; }
        public Nullable<decimal> DailyConsumption { get; set; }
    }
}
