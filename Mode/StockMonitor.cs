using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack.DataAnnotations;

namespace Mode
{
    public class StockMonitor
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string StockNo { get; set; }
        public string StockName { get; set; }
        public decimal StratPrice { get; set; }
        public decimal SkCount { get; set; }
        public decimal SkAmount { get; set; }
        public decimal CtPrice { get; set; }
        public decimal CtAmount { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinRange { get; set; }
        public decimal MaxRange { get; set; }
        public string CurrentInfo { get; set; }

    }
}
