using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 保税超市
    /// http://www.cnbuyers.cn/
    /// </summary>
    public class Cnbuyers : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int cid;
            return int.TryParse(catId, out cid);
        }
    }
}
