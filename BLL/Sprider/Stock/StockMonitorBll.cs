using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase.Stock;
using Mode;
using Commons;

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

        public void VolidReportCount()
        {
            var count =new XqStockDayReportDB().GetXqStockDayCount();
            if (count > 3000)
                return;
            EmailServer.SendMail("更新异常，更新数量为" + count, "信息监控", new string[] { "195896636@qq.com" });
        }

        public void DelCurrentDatXqStock()
        {
            new XqStockDayReportDB().DelCurrentDatXqStock();
        }
    }
}
