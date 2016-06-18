using System;
using Commons;
using Mode.account;
using ServiceStack.OrmLite;

namespace DataBase.Account
{
    public class UserAccountDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public UserAccountDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<UserAccount>();
            }
        }
        public void SaveAccount(UserAccount account)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(account);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }


            }
        }
        public UserAccount GetById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.SingleById<UserAccount>(id);
            }
        }
        public void UpdateAccount(UserAccount account)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Update(account);
            }

        }
    }
}
