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
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public decimal Amplitude { get; set; }

        public decimal AmplitudePercent { get; set; }
        public decimal Oldprice { get; set; }
        public decimal Startprice { get; set; }
        public decimal Maxprice { get; set; }
        public decimal Minprice { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public int Turnover { get; set; }
        public decimal Huanshou { get; set; }
        public decimal Zhenfu { get; set; }
        public decimal Liangbi { get; set; }
        public string StockType { get; set; }
        public string StockTypeAdd { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
