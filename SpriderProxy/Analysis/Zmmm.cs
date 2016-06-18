using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.zuimeimami.com/
    /// 最美妈咪
    /// </summary>
    public class Zmmm : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int result;
            return int.TryParse(catId, out result);
         
        }
    }
}
