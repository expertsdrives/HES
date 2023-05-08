using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HESMDMS.Models
{
    public class LiveUpdateSignalRHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public LiveUpdateSignalRHub()
        {
            _stockTicker = StockTicker.Instance;
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }
    }
}