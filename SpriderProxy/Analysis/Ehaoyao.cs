using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.ehaoyao.com/
    /// 好药师
    /// </summary>
    public class Ehaoyao : BaseSiteInfo
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
