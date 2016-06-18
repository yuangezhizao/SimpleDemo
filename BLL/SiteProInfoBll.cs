using System.Collections.Generic;
using DataBase;
using Mode;

namespace BLL
{
    public class SiteProInfoBll
    {
        public void addSiteProinfo(List<SiteProInfo> list)
        {
            new SiteProductsDb().AddSiteProInfo(list);
            new TempAddSitePro().AddSiteProInfo(list);
            
        }

        public SiteProInfo FindOne(string SiteSkuId)
        {
            return new SiteProductsDb().FindOne(SiteSkuId);
        }

        public void UpdateProinfo(SiteProInfo pro)
        {
            new TempUpdateSitePro().AddSiteProInfo(pro);
            new SiteProductsDb().UpdateProinfo(pro);
        }

        public void SaveProinfo(SiteProInfo pro)
        {
            new TempAddSitePro().AddSiteProInfo(pro);
            new SiteProductsDb().AddSiteProInfo(pro);
        }
    }
}
