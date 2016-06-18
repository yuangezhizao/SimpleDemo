using DataBase.Mongo;
using Mode;

namespace BLL
{
   public class HisSiteProInfoBll
    {
       public void AddHissiteproinfo(SiteProInfo pro)
       {
           new HisSiteProInfoDb().AddHissiteproinfo(pro);
       }

     
    }
}
