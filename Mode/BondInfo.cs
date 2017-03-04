using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    /// <summary>
    ///     bond.money.hexun.com/all_bond/1589363.shtml
    /// </summary>
    public class BondInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Companyname { get; set; }
        public string ShortName { get; set; }

        /// <summary>
        ///     债券代码
        /// </summary>
        public string BondNo { get; set; }

        /// <summary>
        ///     发布时间
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        ///     上市时间
        /// </summary>
        public DateTime MarketDate { get; set; }

        /// <summary>
        ///     发行额(亿元)
        /// </summary>
        public float IssuedAmount { get; set; }

        /// <summary>
        ///     面额
        /// </summary>
        public string Denomination { get; set; }

        /// <summary>
        ///     发行价
        /// </summary>
        public string IssuedPrice { get; set; }

        /// <summary>
        ///     发行开始时间
        /// </summary>
        public DateTime IssuedBeginDate { get; set; }

        /// <summary>
        ///     发行结束时间
        /// </summary>
        public DateTime IssuedEndDate { get; set; }

        /// <summary>
        ///     发行方式
        /// </summary>
        public string IssuedType { get; set; }

        /// <summary>
        ///     发行对象
        /// </summary>
        public string IssuedObject { get; set; }

        /// <summary>
        ///     期限（年）
        /// </summary>
        public string Deadline { get; set; }

        /// <summary>
        ///     年利率
        /// </summary>
        public float AnnualInterestRate { get; set; }

        /// <summary>
        ///     调整后年利率
        /// </summary>
        public float AnnualInterestRateNow { get; set; }

        /// <summary>
        ///     计息日
        /// </summary>
        public DateTime JixiDay { get; set; }

        /// <summary>
        ///     到期时间
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        ///     兑付价
        /// </summary>
        public float RedemptionPrice { get; set; }

        /// <summary>
        ///     认购对象
        /// </summary>
        public string SubscriptionObject { get; set; }

        /// <summary>
        ///     债券价值
        /// </summary>
        public float Bondvalue { get; set; }

        /// <summary>
        ///     税收状况
        /// </summary>
        public string TaxStatus { get; set; }

        /// <summary>
        ///     信用级别
        /// </summary>
        public int CreditRating { get; set; }

        /// <summary>
        ///     发行单位
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        ///     还本付息方式
        /// </summary>
        public string InterestType { get; set; }

        /// <summary>
        ///     发行担保人
        /// </summary>
        public string Guarantor { get; set; }

        /// <summary>
        ///     主承销机构
        /// </summary>
        public string UnderwritingAgency { get; set; }

        /// <summary>
        ///     债券类型
        /// </summary>
        public string Bondtype { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Remork { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}