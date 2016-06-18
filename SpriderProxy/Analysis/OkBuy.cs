using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.okbuy.com/
    /// 好乐买
    /// </summary>
    public class OkBuy:BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length > 20)
                return false;
            return true;
        }
    }
}
