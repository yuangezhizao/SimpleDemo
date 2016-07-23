using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class SiteUserInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public SiteUserInfoDB()
        {
            //  _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteUserInfo>();
            }
        }

        public void addUser(SiteUserInfo user)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(user);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex);
            }
        }

        public void addUser(List<SiteUserInfo> users)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.InsertAll(users);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex);
            }
        }

        public List<SiteUserInfo> GetAllUser()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Select<SiteUserInfo>();
            }
        }

    }
}
