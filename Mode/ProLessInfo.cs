using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ProLessInfo
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string SkuId { get; set; }

        /// <summary>
        ///     商品名称
        /// </summary>
        public string ProName { get; set; }

        /// <summary>
        ///     商品价格
        /// </summary>
        public decimal ProPrice { get; set; }

        /// <summary>
        ///     商品地址
        /// </summary>
        public string ProUrl { get; set; }

        /// <summary>
        ///     商品图片
        /// </summary>
        public string ProImg { get; set; }

        /// <summary>
        ///     优惠信息
        /// </summary>
        public string Promotions { get; set; }

        /// <summary>
        ///     品牌id
        /// </summary>
        public int BrandID { get; set; }

        /// <summary>
        ///     品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        ///     历史折扣
        /// </summary>
        public decimal HisDiscounts { get; set; }

        /// <summary>
        ///     历史价格
        /// </summary>
        public decimal hisPrice { get; set; }

        /// <summary>
        ///     差额 历史价-当前价
        /// </summary>
        public decimal balance { get; set; }

        /// <summary>
        ///     历史最低价格
        /// </summary>
        public decimal floorPrice { get; set; }

        /// <summary>
        ///     0初始值 1自营 2非自营
        /// </summary>
        public int SellType { get; set; }

        /// <summary>
        ///     类别名称
        /// </summary>
        public string ClassId { get; set; }

        /// <summary>
        ///     商城编号
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        ///     商城类别编号
        /// </summary>
        public string SiteClassId { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}