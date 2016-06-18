using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Mode.account;
using ServiceStack.OrmLite;

namespace DataBase.Account
{
    public class UserTransactionDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public UserTransactionDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<UserTransaction>();
            }
        }

        public void SaveTransaction(UserTransaction transaction)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(transaction);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }


            }
        }
        public UserTransaction GetById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.SingleById<UserTransaction>(id);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }
            }
        }

        public List<UserTransaction> GetTransactionByOrderid(int orderid)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.Select<UserTransaction>().Where(c => c.OrderId == orderid).ToList();
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }
            }
        }

        public void UpdateOrderInfo(UserTransaction transaction)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Update(transaction);
            }

        }
    }
}
