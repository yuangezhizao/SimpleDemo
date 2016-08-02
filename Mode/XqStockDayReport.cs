using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class XqStockDayReport
    {
       
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string StockNo { get; set; }
        public string Exchange { get; set; }
        public string Code { get; set; }
        public string StockName { get; set; }
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 跌幅百分比
        /// </summary>
        public float Range { get; set; }
        /// <summary>
        /// （下跌或者上涨）价格
        /// </summary>
        public float RangeRatio { get; set; }

        public decimal OpenPrice { get; set; }
        public decimal Maxprice { get; set; }
        public decimal Minprice { get; set; }
        public decimal Closeprice { get; set; }
        /// <summary>
        /// 昨天收盘价
        /// </summary>
        public decimal LastCloseprice { get; set; }
        /// <summary>
        /// high52week 52周最高
        /// </summary>
        public float High52Week { get; set; }
        /// <summary>
        /// high52week 52周最低
        /// </summary>
        public float Low52Week { get; set; }

        //volume成交（手 1/100股）
        public float Volume { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public float VolumeAverage { get; set; }
        public float MarketCapital { get; set; }
        public float Eps { get; set; }
        public float Pettm { get; set; }
        public float PElyr { get; set; }

        public float Beta { get; set; }
        public float TotalShares { get; set; }
        public float AfterHours { get; set; }
        public float AfterHoursPct { get; set; }
        public float AfterHoursChg { get; set; }
        public float UpdateAt { get; set; }
        public float Dividend { get; set; }
        public float Yield { get; set; }
        public float Turnoverrate { get; set; }
        public float InstOwn { get; set; }
        public float Risestop { get; set; }
        public float FallStop { get; set; }
        public string CurrencyUnit { get; set; }
        public string Amount { get; set; }
        public float NetAssets { get; set; }
        public string Hasexist { get; set; }
        public string HasWarrant { get; set; }
        public float Type { get; set; }
        public int Flag { get; set; }
        public string RestDay { get; set; }
        public float Amplitude { get; set; }
        public float LotSize { get; set; }
        public float MinOrderQuantity { get; set; }
        public float MaxOrderQuantity { get; set; }
        public float TickSize { get; set; }
        public string KzzStockSymbol { get; set; }
        public string KzzStockName { get; set; }
        public float KzzStockCurrent { get; set; }
        public float KzzConvertPrice { get; set; }
        public float kzzcovertValue { get; set; }
        public float KzzCpr { get; set; }
        public float KzzPutbackPrice { get; set; }
        public string KzzConvertTime { get; set; }
        public float KzzRedemptPrice { get; set; }
        public float KzzStraightPrice { get; set; }
        public string KzzStockPercent { get; set; }
        public float Pb  { get; set; }
        public float BenefitBeforeTax { get; set; }
        public float BenefitAfterTax { get; set; }
        public string ConvertBondRatio { get; set; }
        public string Totalissuescale { get; set; }
        public string Outstandingamt { get; set; }
        public string Maturitydate { get; set; }
        public string RemainYear { get; set; }

        public string Interestrtmemo { get; set; }
        public string ReleaseDate { get; set; }
        public float Circulation { get; set; }
        public float ParValue { get; set; }
        public float DueTime { get; set; }
        public string ValueDate { get; set; }
        public string DueDate { get; set; }
        public string Publisher { get; set; }
        public string RedeemType { get; set; }
        public int IssueType { get; set; }
        public string BondType { get; set; }
        public string Warrant { get; set; }
        public string SaleRrg { get; set; }
        public string Rate { get; set; }
        public int AfterHourVol { get; set; }
        public float FloatShares { get; set; }
        public string FloatMarketCapital { get; set; }
        public string DisnextPayDate { get; set; }
        public float ConvertRate { get; set; }
        public float Psr { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
