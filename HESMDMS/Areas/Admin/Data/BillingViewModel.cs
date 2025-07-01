using System;

namespace HESMDMS.Areas.Admin.Data
{
    public class BillingViewModel
    {
        public int ID { get; set; }
        public string BusinessPartner { get; set; }
        public string Installation { get; set; }
        public DateTime? Date { get; set; }
        public string Material { get; set; }
        public string Meter_SerialNo { get; set; }
        public string MessageType { get; set; }
        public string MessageID { get; set; }
        public string MessageDescription { get; set; }
        public string CustomerID { get; set; }
        public string MeterSerialNumber { get; set; }
    }
}