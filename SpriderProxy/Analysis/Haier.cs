using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 海尔商城
    /// http://www.ehaier.com/
    /// </summary>
    public class Haier:BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 4 || catId.Length > 30)
                return false;
            return true;
        }
    }
}
