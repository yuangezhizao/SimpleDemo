using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class SiteInfoBll
    {
        public List<SiteInfo> GetAllSite()
        {
            return new SiteInfoDB().getAllSite();
        }
    }
}
