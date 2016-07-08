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
    public class StockinfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public StockinfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<StockInfo>();
            }
        }

        public void AddStockinfo(List<StockInfo> list)
        {

            using (var db = _dbFactory.OpenDbConnection())
            {

                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    foreach (StockInfo info in list)
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

        public List<StockInfo> GetAllinfo()
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<StockInfo>().ToList();
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
