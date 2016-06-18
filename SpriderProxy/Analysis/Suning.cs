using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.suning.com
    /// 苏宁易购
    /// </summary>
    public class Suning : BaseSiteInfo
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
            if (catId.Length < 5 || catId.Length > 10)
                return false;
            return true;
        }
        private const string commentUrl = "http://zone.suning.com/review/json/product_reviews/{0}--total-g-278600---10-{1}-getItem.html?callback=getItem";
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
