namespace Mode
{
    public class SpiderError
    {
        public int Id { get; set; }
        /// <summary>
        /// 商城编号
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// 商城名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 错误地址
        /// </summary>
        public string UrlPath { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int SpId { get; set; }
        /// <summary>
        /// 错误类型
        /// </summary>
        public string ErrType { get; set; }
        /// <summary>
        /// 错误等级
        /// </summary>
        public int Lvevl { get; set; }
        /// <summary>
        /// 错误详细信息
        /// </summary>
        public string ErrDetial { get; set; }
        /// <summary>
        /// 错误备注信息
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 页面html，整页
        /// </summary>
        public string PageHtml { get; set; }
        /// <summary>
        /// 商品列表匹配数据，如果列表正则出问题则该数据会取不到，通过PageHtml可以分析当时的html情况
        /// </summary>
        public string ListHtml { get; set; }
        /// <summary>
        /// 商品商品html，通过该html可以判断标题、连接匹配不到时的情况，如果single未匹配到数据可以通过ListHtml来检查当时的html情况
        /// </summary>
        public string SingleHtml { get; set; }
    }
}
