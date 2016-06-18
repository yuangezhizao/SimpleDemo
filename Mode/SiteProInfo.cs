using System;
using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;


namespace Mode
{
    /// <summary>
    /// 商城商品信息
    /// </summary>
    public class SiteProInfo
    {
        [AutoIncrement]
        public int Id { get; set; }
        //public BsonObjectId _id { get; set; }
        public int ClassId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string SpName { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal SpPrice { get; set; }
        /// <summary>
        /// 产品地址
        /// </summary>
        public string ProUrl { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string smallPic { get; set; }
        /// <summary>
        /// 大图
        /// </summary>
        public string BigPic { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string spBrand { get; set; }

        public int BrandId { get; set; }
        /// <summary>
        /// 其他图片
        /// </summary>
        public string Otherpic { get; set; }
        
        public int CommenCount { get; set; }

        public string CommentUrl { get; set; }

        /// <summary>
        /// 促销信息
        /// </summary>
        public string Promotions { get; set; }
        /// <summary>
        /// 历史最低价
        /// </summary>
        public decimal FloorPrice { get; set; }

        public string SingleDesc { get; set; }
        
        /// <summary>
        /// 1自营 2非自营
        /// </summary>
        public int SellType { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string ShopName { get; set; }
        /// <summary>
        /// 是否在售
        /// </summary>
        public bool IsSell { get; set; }

        /// <summary>
        /// 商城id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 商城类别id
        /// </summary>
        public string SiteCat { get; set; }
        //[Required]
        [Index(Unique = true)]
        //[BsonId]
        public string SiteSkuId { get; set; }

        public string spSkuDes { get; set; }
        /// <summary>
        /// 商城proid
        /// </summary>
        public string SiteProId { get; set; }

        //public string sitePageid { get; set; }

        //public string sitePageurl { get; set; }

        /// <summary>
        /// 分类搜索 类型
        /// </summary>
        public string SearchSkuJson { get; set; }
       
        public string SearchFiles { get; set; }

        public string AreaPriceDetial { get; set; }

        public int QzSort { get; set; }

    
        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }

    }

    public class TempSiteProInfo 
    {
        [AutoIncrement]
        public int Id { get; set; }
        //public BsonObjectId _id { get; set; }
        public int ClassId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string SpName { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal SpPrice { get; set; }
        /// <summary>
        /// 产品地址
        /// </summary>
        public string ProUrl { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string smallPic { get; set; }
        /// <summary>
        /// 大图
        /// </summary>
        public string BigPic { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string spBrand { get; set; }

        public int BrandId { get; set; }
        /// <summary>
        /// 其他图片
        /// </summary>
        public string Otherpic { get; set; }

        public int CommenCount { get; set; }

        public string CommentUrl { get; set; }

        /// <summary>
        /// 促销信息
        /// </summary>
        public string Promotions { get; set; }
        /// <summary>
        /// 历史最低价
        /// </summary>
        public decimal FloorPrice { get; set; }

        public string SingleDesc { get; set; }

        /// <summary>
        /// 1自营 2非自营
        /// </summary>
        public int SellType { get; set; }

        public string ShopName { get; set; }

        public bool IsSell { get; set; }


        /// <summary>
        /// 商城id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 商城类别id
        /// </summary>
        public string SiteCat { get; set; }
        //[Required]
        [Index(Unique = true)]
        //[BsonId]
        public string SiteSkuId { get; set; }

        public string spSkuDes { get; set; }
        /// <summary>
        /// 商城proid
        /// </summary>
        public string SiteProId { get; set; }

        //public string sitePageid { get; set; }

        //public string sitePageurl { get; set; }



        /// <summary>
        /// 分类搜索 类型
        /// </summary>
        public string SearchSkuJson { get; set; }

        public string SearchFiles { get; set; }

        public string AreaPriceDetial { get; set; }

        public int QzSort { get; set; }


        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }
    }

}
