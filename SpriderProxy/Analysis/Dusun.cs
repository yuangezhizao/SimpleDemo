using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.dusun.com.cn/
    /// 大尚国际
    /// </summary>
    public class Dusun : BaseSiteInfo
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
