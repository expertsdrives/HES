using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class APIData
    {
        public string idType { get; set; }
        public string id { get; set; }
        public string transactionId { get; set; }
        public string retentionTime { get; set; }
        public string data { get; set; }
    }
    public class c2dProd
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
    public class c2dTokenProd
    {
        public string access_token { get; set; }
    }

}