using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class ShopInfoDb : OrmLiteFactory
    {
         private static OrmLiteConnectionFactory _dbFactory;

        public ShopInfoDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString,SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<ShopInfo>();
            }
        }

        public List<ShopInfo> getAllSite()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                // return db.Dictionary<int, string>(db.From<ShopInfo>().Select(x => new { x.Id, x.UserName,x.UserPhone }).Where(x=>x.Id==21));
               return db.Select<ShopInfo>();
            }
        }

        public void AddSiteInfo(List<ShopInfo> siteList)
        {
            if (siteList == null) throw new ArgumentNullException("siteList");
     
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(siteList);
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < siteList.Count; i++)
                    {
                        AddSiteInfo(siteList[i]);
                    }
                    
                    LogServer.WriteLog(ex, "DBError");
              
                }

            }
        }

        public void AddSiteInfo(ShopInfo info)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(info);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

    }
}
