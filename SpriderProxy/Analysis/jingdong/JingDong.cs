using System;
using System.Collections.Generic;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 京东商城
    /// </summary>
   public class JingDong : BaseSiteInfo
    {
        private const string RegClassid =
            "http://list.jd.com/(?<x>\\d*-\\d*(-\\d*)?).html|http://list.jd.com/list.html\\?cat=(?<x>\\d*,\\d*(,\\d*)?)";

        public string GetSiteClassIdByUrl(string url)
        {
            string classid= RegGroupsX<string>(url,RegClassid);
            if (string.IsNullOrEmpty(classid))
                return classid;
            return classid.Replace('-',',');
        }

        private const string CommentUrl = "";
        /// <summary>
        /// 获取评论地址
        /// </summary>
        /// <param name="catid">分类Id</param>
        /// <param name="pageid">页号</param>
        /// <returns></returns>
        public string getCommentUrl(string catid, int pageid)
        {
            return string.Format(CommentUrl, catid, pageid);
        }

        /// <summary>
        /// 商城分类id 验证
        /// </summary>
        /// <param name="catId">分类id</param>
        /// <returns></returns>
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 7 || catId.Length > 20)
                return false;
            return true;
        }

    }
}
