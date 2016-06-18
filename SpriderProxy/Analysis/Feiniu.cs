using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.feiniu.com/
    /// 飞牛网
    /// </summary>
   public class Feiniu : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 4 || catId.Length > 10)
                return false;
            return true;
        }
    }
}
