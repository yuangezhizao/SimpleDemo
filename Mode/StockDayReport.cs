using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class StockDayReport
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string StockNo { get; set; }
        public string StockName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ZhenFu { get; set; }
        public float Range { get; set; }
        public float RangeRatio { get; set; }
        public decimal Maxprice { get; set; }
        public decimal Minprice { get; set; }
        public float MarketValue { get; set; }
        public float Amount { get; set; }
        public int Volume { get; set; }
        public float Exchange { get; set; }
        public decimal Pe { get; set; }
        public decimal Pb { get; set; }
        public decimal PElyr { get; set; }
        public bool IsStop{ get; set; }
        public float Zdf5 { get; set; }
        public float Zdf10 { get; set; }
        public float Zdf20 { get; set; }
        public float Zdf60 { get; set; }
        public float Zdf120 { get; set; }
        public float Zdf250 { get; set; }
        public string IndexNumber { get; set; }
        public string Industry { get; set; }
        public string Area { get; set; }
        public DateTime CreateDate { get; set; }


    }
}
