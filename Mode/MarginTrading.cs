using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class MarginTrading
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string StockNo { get; set; }
        public string StockName { get; set; }

        public decimal Rqrz { get; set; }
        /// <summary>
        /// 融券偿还量
        /// </summary>
        public decimal Rqchl { get; set; }
        /// <summary>
        /// 融券卖出量
        /// </summary>
        public decimal Rqmcl { get; set; }
        /// <summary>
        /// 融券余额
        /// </summary>
        public decimal Rqye { get; set; }
        /// <summary>
        /// 融券余量
        /// </summary>
        public decimal Rqyl { get; set; }
        /// <summary>
        /// 融资偿还额
        /// </summary>
        public decimal Rzchl { get; set; }
        /// <summary>
        /// 融资买入额
        /// </summary>
        public decimal Rzmre { get; set; }
        /// <summary>
        /// 融资融券余额
        /// </summary>
        public decimal Rzrqye { get; set; }
        /// <summary>
        /// 融资余额
        /// </summary>
        public decimal Rzye { get; set; }
        /// <summary>
        /// 融资净买额
        /// </summary>
        public decimal Rzjme { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public  DateTime ReportDate { get; set; }



    }
}
