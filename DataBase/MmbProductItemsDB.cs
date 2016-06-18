using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
   public class MmbProductItemsDB : OrmLiteFactory
    {
       private  OrmLiteConnectionFactory _dbFactory;

       public MmbProductItemsDB()
       {
           _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
       }

       public List<MmbProductItems> GetItem(int minProid, int maxProid,string siteid)
       {
           int error = 0;
           do
           {
               try
               {
                   using (var db = _dbFactory.OpenDbConnection())
                   {
                       siteid = string.IsNullOrEmpty(siteid) ? "siteid!=5 and siteid!=10" : string.Format(siteid.Contains(',') ? "siteid in ({0})" : "siteid = {0}", siteid);


                       string sql = string.Format(
                               "select id, classid,siteid,spurl,spid,ppid,shopID,spname,sppic,pingpai  from Check_tempNewPro with(nolock) where  spid>={0} and spid<{1}  and DATEDIFF(dd,updatetime,GETDATE())<8 and {2} ",
                               minProid, maxProid, siteid);
                       return db.SelectFmt<MmbProductItems>(sql);
                   }
               }
               catch (Exception ex)
               {       
                   error++;
                   LogServer.WriteLog(ex, "DBError");
                  
               }
           } while (error <3);


           return null;
         
       }

       public int GetItemMaxId()
       {
           _dbFactory = new OrmLiteConnectionFactory(mmbDBConnectionString, SqlServerDialect.Provider);
           int error = 0;
           do
           {
               try
               {
                   using (var db = _dbFactory.OpenDbConnection())
                   {
                       return db.Single<int>("select MAX(id) From JD_Product with(nolock)");
                   }
               }
               catch (Exception ex)
               {
                   LogServer.WriteLog(ex, "DBError");
                   error++;
               }
           } while (error < 3);
           return 0;
       }


    }
}
