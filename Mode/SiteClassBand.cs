using System;
using MongoDB.Bson;

namespace Mode
{
    /// <summary>
    /// 商城分类品牌对应信息
    /// </summary>
    public class SiteClassBand
    {
        public BsonObjectId _id { get; set; }

        public int SiteId { get; set; }

        public string DisplayName { get; set; }

        public string CnName { get; set; }

        public string EnName { get; set; }

        public string SiteClassId { get; set; }

        public string SiteBandId { get; set; }

        public string ImgUrl { get; set; }

        public string UniqueKey { get; set; }
        /// <summary>
        /// 商城分类品牌地址
        /// </summary>
        public string Urlinfo { get; set; }

        /// <summary>
        /// 品牌 产品数量
        /// </summary>
        public int ProductCount { get; set; }
        /// <summary>
        /// 品牌 评论数
        /// </summary>
        public int CommentCount { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsHid { get; set; }


    }
}
