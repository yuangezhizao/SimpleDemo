using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
   public class BondInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public BondInfoDB()
        {
             _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
           // _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<BondInfo>();
            }
        }
        public void AddBondInfo(List<BondInfo> list)
        {

            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {

                    foreach (BondInfo info in list)
                    {
                        try
                        {
                            using (var db1 = _dbFactory.OpenDbConnection())
                            {
                                db1.Insert(info);
                            }
                        }
                        catch (Exception ex1)
                        {

                            LogServer.WriteLog(ex1.Message, "DBError");
                        }

                    }

                    LogServer.WriteLog(ex, "DBError");

                }
            }


        }

        public List<BondInfo> GetAllinfo()
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        return db.Select<BondInfo>().ToList();
                    }
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                    Thread.Sleep(10000);
                    error++;
                }
            } while (error < 5);
            return null;

        }
    }
}
