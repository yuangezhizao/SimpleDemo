using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 格家美居
    /// http://www.gj5s.com/product/search/qgyys.html
    /// </summary>
    public class Gjmj:BaseSiteInfo
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
