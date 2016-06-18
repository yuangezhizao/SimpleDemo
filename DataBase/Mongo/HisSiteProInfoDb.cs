using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;

namespace DataBase.Mongo
{
   public class HisSiteProInfoDb : BaseDB
    {
        public HisSiteProInfoDb()
            : base("HisSiteProInfo")
        {
        }

       public void AddHissiteproinfo(SiteProInfo pro)
       {
           try
           {
               Collection.Insert(pro);
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex, "DBError");
           }
       }
    }
}
