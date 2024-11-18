using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string PaymentLink { get; set; }
    }
}