using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 当当网
    /// http://www.dangdang.com/
    /// </summary>
    public class DangDang : BaseSiteInfo
    {
        public override bool ValidCatId(string catid)
        {
            if (string.IsNullOrEmpty(catid))
                return false;
            if (catid.Length !=7)
                return false;
            return true;
        }
        private const string commentUrl = "http://product.dangdang.com/comment/comment.php?product_id={0}&datatype=1&page={1}&filtertype=1&sysfilter=1&sorttype=1";
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
