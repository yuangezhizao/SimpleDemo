using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using Mode;
using Commons;

namespace DataBase
{
    public class SiteInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public SiteInfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteInfo>();
            }
        }

        public SiteInfo SiteById(int siteid)
        {
            try
            {
                //getSithInfo();
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Single<SiteInfo>(p => p.SiteId == siteid);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public void getSithInfo()
        {
            List<SiteInfo> lits = null;
            _dbFactory = new OrmLiteConnectionFactory(MmbDbConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                lits = db.SqlList<SiteInfo>("select id as SiteId,sitename as SiteName,sitelogo as SiteLogo,fk as Domain,smallLogo as smallLogo from JD_hzSite");
            }
            AddSiteInfo(lits);
        }

        public List<SiteInfo> getAllSite()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Select<SiteInfo>();
            }
        }

        public static void AddSiteInfo(IEnumerable<SiteInfo> siteList)
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
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

    }
}
