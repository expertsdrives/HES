using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class BillingData
    {
        public string BusinessPartner { get; set; }
        public string Installation { get; set; }
        public string GASAmount { get; set; }
        public string VATAmount { get; set; }
        public string DiscountAmount { get; set; }
        public string MinimumAmount { get; set; }
        public string CGST_on_MinimumCharge { get; set; }
        public string SGST_on_MinimumCharge { get; set; }
        public string AMCAmount { get; set; }
        public string CGST_on_AMC_Amount { get; set; }
        public string SGST_on_AMC_Amount { get; set; }
        public string TotalAmount { get; set; }
        public string Disconnection_Flag { get; set; }
        public string GCV { get; set; }
        public string MaterialNumber { get; set; }
        public string MeterNo { get; set; }
        public string ReadingDate { get; set; }
        public string CurrentMeterReading { get; set; }
        public string GASPrice { get; set; }
        public string VATPercentage { get; set; }
        public string Recharge_Balance { get; set; }
        public string FromDate { get; set; }
        public string DueDate { get; set; }
        public string PrePaidSCM_Consumption { get; set; }
        public string PrePaid_Kcal { get; set; }
        public string PrePaid_BTU { get; set; }
        public string PrePaid_MMBTU { get; set; }
        public string Prepaid_Rent_Amount { get; set; }
        public string Prepaid_Rent_CGST { get; set; }
        public string Prepaid_Rent_SGST { get; set; }
        public string Installment_Amount { get; set; }
    }
}