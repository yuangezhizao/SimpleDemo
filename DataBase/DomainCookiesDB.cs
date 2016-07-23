using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class DomainCookiesDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public DomainCookiesDb()
        {
            //_dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteCookies>();
            }
        }

        public void SaveCookies(SiteCookies cookies)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    var item = db.Single<SiteCookies>(c => c.Domain == cookies.Domain);
                    if (item == null || item.Id == 0)
                    {
                        cookies.CreateDate = DateTime.Now;
                        cookies.UpdateTime = DateTime.Now;
                        db.Insert(cookies);
                    }
                    else
                    {
                        item.Domain = cookies.Domain;
                        item.Url = cookies.Url;
                        item.Cookies = cookies.Cookies;
                        item.UserAgent = cookies.UserAgent;
                        item.UpdateTime=DateTime.Now;
                        db.Save(item);
                    }
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex);
            }
        }

        public SiteCookies GetOneByDomain(string domain)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    var item = db.Single<SiteCookies>(c => c.Domain == domain);
                    if (item == null || item.Id == 0)
                    {

                        return null;
                    }
                    return item;
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex);
                return null;
            }
        }
    }
}
