using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class PromotionsInfo
    {

        [AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public string PromoTitle { get; set; }
        /// <summary>
        /// 活动价格
        /// </summary>
        public decimal PromoPrice { get; set; }
        /// <summary>
        /// 活动网址
        /// </summary>
        public string PromoUrl { get; set; }
        /// <summary>
        /// 活动手机网址
        /// </summary>
        public string PromoWapUrl { get; set; }

        public string PromoWeibo { get; set; }

        /// <summary>
        /// 活动详细信息
        /// </summary>
        public string PromoDesc { get; set; }
        /// <summary>
        /// 活动图片
        /// </summary>
        public string PromoPic { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime PromoStartTime { get; set; }
        /// <summary>
        /// 活动截至时间
        /// </summary>
        public DateTime PromoStopTime { get; set; }
        /// <summary>
        ///  活动类型
        /// </summary>
        public string PromoType { get; set; }
        /// <summary>
        /// 商城id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 商城名称
        /// </summary>
        public string SiteName { get; set; }

        public string SkuId { get; set; }

        public string SkuName { get; set; }

        public string SiteProId { get; set; }
        /// <summary>
        /// 商城分类
        /// </summary>
        public string SiteCatID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
