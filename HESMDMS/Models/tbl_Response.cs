namespace HESMDMS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Response
    {
        public int ID { get; set; }

        public DateTime? LogDate { get; set; }

        [StringLength(4000)]
        public string pld { get; set; }

        [StringLength(50)]
        public string aid { get; set; }

        [StringLength(50)]
        public string eid { get; set; }

        public string Data { get; set; }
    }
}
