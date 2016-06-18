using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 康复之家(网站已打不开 停用)
    /// http://www.kfzj.com/
    /// </summary>
   public class Kfzj:BaseSiteInfo
    {

        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            return true;
        }
    }
}
