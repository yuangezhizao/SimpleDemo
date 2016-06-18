using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    ///国家食品药品监督管理总局  
    ///http://www.sfda.gov.cn
    /// </summary>
    public class Sfda : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 20)
                return false;
            return true;
        }
    }
}
