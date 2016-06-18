using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    
    /// <summary>
    /// http://www.111.com.cn/
    /// 1药网
    /// </summary>
    public class Yiyao : BaseSiteInfo
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
