using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class AMRCRCModel
    {
        public string VAYUDUT_ID { get; set; }
        public string V_VOLTAGE { get; set; }
        public string VAYUDUT_TEMPERATURE { get; set; }
        public string MODULE_ERROR { get; set; }
        public string NW_ERROR { get; set; }
        public string POST_ERROR { get; set; }
        public string TX_ERRORS { get; set; }
        public string DATA_STRING { get; set; }
    }
}