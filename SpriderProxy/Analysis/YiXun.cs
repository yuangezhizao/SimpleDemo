using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.yixun.com/
    /// 易迅网
    /// </summary>
    public class YiXun : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 6 || catId.Length > 15)
                return false;
            return true;
        }
    }
}
