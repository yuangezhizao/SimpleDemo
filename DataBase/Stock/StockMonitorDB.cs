using System;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;
namespace DataBase.Stock
{
    public class StockMonitorDb : OrmLiteFactory
    {
        public static OrmLiteConnectionFactory DbFactory;
        public StockMonitorDb()
        {
            if (DbFactory == null)
                DbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = DbFactory.OpenDbConnection())
            {
                db.CreateTable<StockMonitor>();
            }
        }

        public void AddStockinfo(StockMonitor item)
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = DbFactory.OpenDbConnection())
                    {
                        db.Insert(item);
                    }
                    break;
                }
                catch (Exception ex1)
                {
                    error++;
                    Thread.Sleep(10000);
                    LogServer.WriteLog(ex1.Message, "DBError");
                }
            } while (error < 4);

        }

        public StockMonitor GetStockMonitor(string skNo)
        {
            try
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                    return db.Single<StockMonitor>(c => c.StockNo == skNo);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex.Message, "DBError");
                return null;
            }
        }
    }
}
