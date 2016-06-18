using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
   public class PromotionsBll
    {
       public List<PromotionsInfo> LoadPromotions()
       {
           var list = new PromotionsDB().LoadPromotionsInfo();
          
            return list.Where(p => p.PromoStopTime > DateTime.Now && p.PromoStartTime < DateTime.Now).OrderBy(p => p.PromoStopTime).ToList();
        }
    }
}
