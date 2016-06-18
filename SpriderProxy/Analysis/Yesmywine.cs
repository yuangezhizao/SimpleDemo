using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 也买酒
    /// http://www.yesmywine.com/
    /// </summary>
    public class Yesmywine : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length > 10)
                return false;
            return true;
        }
    }
}
