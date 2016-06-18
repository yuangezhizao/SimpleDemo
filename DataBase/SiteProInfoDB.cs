using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class SiteProInfoDB: OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public SiteProInfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteProInfo>();
            }
        }

        public void AddSitePro(IEnumerable<SiteProInfo> siteList)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {

                foreach (SiteProInfo item in siteList)
                {
                    try
                    {
                        var olditem = db.Single<SiteProInfo>(new {item.SiteSkuId});
                        if (olditem != null)
                        {
                            //item._id = olditem._id;
                            item.Id = olditem.Id;
                            item.CreateDate = olditem.CreateDate;
                            item.UpdateTime = DateTime.Now;
                            db.Update(item);
                        }
                        else
                            db.Insert(item);
                    }
                    catch (Exception ex)
                    {
                        LogServer.WriteLog(ex, "DBError");
                      
                    }
                }
             
               
            }
        }

        public void AddSiteProInfo(IEnumerable<SiteProInfo> siteList)
        {
            if (siteList == null) throw new ArgumentNullException("siteList");
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(siteList);
                }
                catch (Exception ex)
                {
                    AddSitePro(siteList);
                    LogServer.WriteLog(ex, "DBError");
        
                }

            }
            //var mmbdbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);

            //using (var db = mmbdbFactory.OpenDbConnection())
            //{
            //    try
            //    {
            //        db.CreateTable<SiteProInfo>();
            //        db.SaveAll(siteList);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogServer.WriteLog(ex, "DBError");
            //        throw;
            //    }

            //}
        }


    }
}
