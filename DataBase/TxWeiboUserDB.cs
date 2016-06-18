
using System;
using Commons;
using Mode;
using ServiceStack.OrmLite;
using System.Collections.Generic;

namespace DataBase
{
    public class TxWeiboUserDB : OrmLiteFactory
    {
        private static  OrmLiteConnectionFactory _dbFactory;
        public TxWeiboUserDB()
        {
             _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
             using (var db = _dbFactory.OpenDbConnection())
             {
                 db.CreateTable<WbUser>();
             }
        }

        public static void AddUserInfo(IEnumerable<WbUser> userList)
        {
    
            if (userList == null) throw new ArgumentNullException("userList");
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(userList);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }
           
            }
        }

        public List<WbUser> LoadAllUser()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.Select<WbUser>(q => q.IsUsed);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public void SaveUserInfo(WbUser user)
        {

            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    var wb = db.Single<WbUser>(x => x.IdTwb == user.IdTwb);
                    if (wb == null)
                        db.Insert(user);
                    else
                    {
                        user.CreateDate = wb.CreateDate;
                        db.Update(user);
                    }
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
