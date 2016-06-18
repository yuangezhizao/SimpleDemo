using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.taoxie.com/
    /// 淘鞋网
    /// </summary>
    public class Taoxie : BaseSiteInfo
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
            int cid;
            if (!int.TryParse(catId, out cid))
                return false;
            return true;
        }
    }
}
