using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 金象网
    /// http://www.jxdyf.com/
    /// </summary>
    public class Jxdyf : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int id;
            return int.TryParse(catId, out id);
        }
    }
}
