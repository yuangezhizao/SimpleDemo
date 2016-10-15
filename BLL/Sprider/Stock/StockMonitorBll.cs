using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase.Stock;
using Mode;

namespace BLL.Sprider.Stock
{
    public class StockMonitorBll
    {
        public void AddStockinfo(StockMonitor item)
        {
            new StockMonitorDb().AddStockinfo(item);
        }
        public StockMonitor GetStockMonitor(string skNo)
        {
            return new StockMonitorDb().GetStockMonitor(skNo);
        }
        public List<StockMonitor> GetALlStockMonitor()
        {
            return new StockMonitorDb().GetALlStockMonitor();
        }
    }
}
