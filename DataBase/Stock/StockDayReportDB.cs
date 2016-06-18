using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using Mode.account;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
    public class StockDayReportDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public StockDayReportDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString,SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<StockDayReport>();
            }
        }

        public void AddStockinfo(List<StockDayReport> list)
        {

            using (var db = _dbFactory.OpenDbConnection())
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
                using (var db = _dbFactory.OpenDbConnection())
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
