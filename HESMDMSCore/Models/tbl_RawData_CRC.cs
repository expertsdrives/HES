//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HESMDMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_RawData_CRC
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string VAYUDUT_ID { get; set; }
        public string V_VOLTAGE { get; set; }
        public string VAYUDUT_TEMPERATURE { get; set; }
        public string MODULE_ERROR { get; set; }
        public string NW_ERROR { get; set; }
        public string POST_ERROR { get; set; }
        public string TX_ERRORS { get; set; }
        public string AMR_ID { get; set; }
        public string AMR_VERSION_NO { get; set; }
        public string AMR_LOW_WEIGHTAGE_COUNT { get; set; }
        public string AMR_HIGH_WEIGHTAGE_COUNT { get; set; }
        public string AMR_BATTERY_VOLTAGE { get; set; }
        public string AMR_TEMPERATURE { get; set; }
        public string AMR_MAGNET_TEMPER_LOGGER { get; set; }
        public string AMR_CASE_TEMPER_LOGGER { get; set; }
        public string AMR_CRC8 { get; set; }
        public string AMR_CRC_STATUS { get; set; }
    }
}
