using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class ShopInfoBll
    {
        public List<ShopInfo> getAllSite()
        {
            return new ShopInfoDb().getAllSite();
        }

        public void AddSiteInfo(List<ShopInfo> shopList)
        {
            new ShopInfoDb().AddSiteInfo(shopList);
        }
    }
}
