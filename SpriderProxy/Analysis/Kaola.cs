using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis
{
    public class Kaola : BaseSiteInfo
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
