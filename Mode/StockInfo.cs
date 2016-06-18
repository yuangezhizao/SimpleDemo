using System;
using ServiceStack.DataAnnotations;


namespace Mode
{
    public class StockInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string StockNo { get; set; }
        public string StockName { get; set; }
        public decimal CurrentPrice { get; set; }
        public float Amplitude { get; set; }
        public decimal Oldprice { get; set; }
        public decimal Startprice { get; set; }
        public decimal Maxprice { get; set; }
        public decimal Minprice { get; set; }
        public int Volume { get; set; }
        public int Turnover { get; set; }
        public decimal Huanshou { get; set; }
        public decimal Zhenfu { get; set; }
        public decimal Liangbi { get; set; }
    }
}
