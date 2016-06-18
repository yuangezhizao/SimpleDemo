namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 中国亚马逊
    /// http://www.amazon.cn
    /// </summary>
    public class Amazon : BaseSiteInfo
    {
        /// <summary>
        /// 商城分类id 验证
        /// </summary>
        /// <param name="catId">分类id</param>
        /// <returns></returns>
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 8 || catId.Length > 15)
                return false;
            return true;
        }

        private const string commentUrl = "http://www.amazon.cn/product-reviews/{0}/ref=cm_cr_pr_top_link_{1}?ie=UTF8&pageNumber={1}&showViewpoints=0&sortBy=bySubmissionDateDescending";
        /// <summary>
        /// 获取评论地址
        /// </summary>
        /// <param name="catid">分类Id</param>
        /// <param name="pageid">页号</param>
        /// <returns></returns>
        public string getCommentUrl(string catid, int pageid)
        {
            return string.Format(commentUrl, catid, pageid);
        }
      
    }
}
