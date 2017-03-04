using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class StockDayReport
    {
        //{"stockcode":"600662","stockname":"\u5f3a\u751f\u63a7\u80a1","fieldcode":"1150","fieldname":"\u516c\u4ea4","fieldjp":"gj","xj":"11.21","zdf":"-3.69%","zde":"-0.43","cjl":"34.81 \u4e07\u624b","cje":"3.92 \u4ebf\u5143","kp":"11.46","zs":"11.64","zg":"11.49","zd":"11.00","hs":"3.31%","syl":"81.26","np":189715,"wp":150836,"jj":"11.27","zf":"4.21%","zt":"12.80","dt":"10.48","field":"0.00","wb":"7.86","wc":132,"buy1":"11.20","buy1data":20,"buy2":"11.19","buy2data":185,"buy3":"11.18","buy3data":143,"buy4":"11.17","buy4data":230,"buy5":"11.16","buy5data":328,"sell1":"11.21","sell1data":111,"sell2":"11.22","sell2data":185,"sell3":"11.23","sell3data":196,"sell4":"11.24","sell4data":81,"sell5":"11.25","sell5data":201}
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
        public bool IsStop { get; set; }
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