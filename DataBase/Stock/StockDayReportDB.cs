using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
    public class StockDayReportDb : OrmLiteFactory
    {
        public static OrmLiteConnectionFactory DbFactory;
        public StockDayReportDb()
        {
            if (DbFactory == null)
                DbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = DbFactory.OpenDbConnection())
            {
                db.CreateTable<StockDayReport>();
            }
        }

        public void AddStockinfo(StockDayReport item)
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
            } while (error<4);
       
        }

        public void AddStockinfo(List<StockDayReport> list)
        {

            using (var db = DbFactory.OpenDbConnection())
            {

                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    foreach (StockDayReport info in list)
                    {
                        try
                        {
                            db.Insert(info);
                        }
                        catch (Exception ex1)
                        {

                            LogServer.WriteLog( ex1.Message, "DBError");
                        }

                    }

                    LogServer.WriteLog(ex, "DBError");

                }
            }
         

        }

        public List<StockDayReport> GetAllinfo()
        {
            try
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                  return  db.Select<StockDayReport>().ToList();
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
