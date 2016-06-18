using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.7lk.com/
    /// 七乐康
    /// </summary>
   public class Qlk : BaseSiteInfo
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
