using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 老百姓网
    /// http://www.lbxcn.com/
    /// </summary>
    public class Lbxcn : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            return true;
        }
    }
}
