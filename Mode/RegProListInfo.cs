using ServiceStack.DataAnnotations;

namespace Mode
{
    /// <summary>
    /// 商城 产品列表页 正则信息
    /// </summary>
    public class RegProListInfo
    {
        [AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// 商城id
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 列表页URL地址
        /// </summary>
        public string ProListUrl { get; set; }
        /// <summary>
        /// 列表页 产品区域正则
        /// </summary>
        public string ListsReg { get; set; }
        /// <summary>
        /// 单个产品区块正则
        /// </summary>
        public string SingleReg { get; set; }
        /// <summary>
        /// 商品URL正则
        /// </summary>
        public string UrlReg { get; set; }

        public string PriceReg { get; set; }

        public string TitleReg { get; set; }
        /// <summary>
        /// 列表页产品图片
        /// </summary>
        public string PicReg { get; set; }

        public string IsSellReg { get; set; }

        public string CommentCountReg { get; set; }

        public string CommentUrlReg { get; set; }
        /// <summary>
        /// 产品id 
        /// </summary>
        public string SkuReg { get; set; }

        /// <summary>
        /// 第三方卖家 正则
        /// </summary>
        public string shopidReg { get; set; }

        public string MaxpageReg { get; set; }

         public int PageStart { get; set; }

         public int PageStep { get; set; }

         public string Remark { get; set; }

 

        public System.DateTime UpdateTime { get; set; }
        
    }
}
