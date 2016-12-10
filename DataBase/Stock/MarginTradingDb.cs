using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
    public class MarginTradingDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public MarginTradingDb()
        {
            // _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<MarginTrading>();
            }
        }
        public void AddMarginTrading(List<MarginTrading> list)
        {

            using (var db = _dbFactory.OpenDbConnection())
            {

                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    foreach (MarginTrading info in list)
                    {
                        try
                        {
                            db.Insert(info);
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
        public List<MarginTrading> GetAllinfo()
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {

                        // return db.SqlList<StockInfo>("select * from dbo.StockInfo with(nolock) where stockno not in(select Code from dbo.XqStockDayReport with(nolock) )");
                        return db.Select<MarginTrading>().ToList();
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

        public MarginTrading GetNewestTrading(string code)
        {
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        return db.Select<MarginTrading>(c => c.StockNo == code).OrderByDescending(c=>c.ReportDate).FirstOrDefault();
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
