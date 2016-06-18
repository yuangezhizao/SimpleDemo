using System.Collections.Generic;
using DataBase;
using Mode;

namespace BLL
{
   public class MmbProductItemsBll
    {
       public List<MmbProductItems> GetItem(int minProid, int maxProid,string siteid)
       {
          return new MmbProductItemsDB().GetItem(minProid, maxProid,siteid);
       }

       public int GetProductMaxId()
       {
           return new MmbProductItemsDB().GetItemMaxId();
       }
    }
}
