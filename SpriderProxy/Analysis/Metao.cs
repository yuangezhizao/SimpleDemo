using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.metao.com/
    /// 蜜淘
    /// </summary>
    public class Metao : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            return true;
        }
    }
}
