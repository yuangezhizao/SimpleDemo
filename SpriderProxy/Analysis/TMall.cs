using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// https://www.tmall.com/
    /// 天猫商城
    /// </summary>
    public class TMall : BaseSiteInfo
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
            if (catId.Length !=8)
                return false;
            return true;
        }
    }
}
