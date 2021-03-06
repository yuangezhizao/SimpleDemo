﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
    public class StockinfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public StockinfoDB()
        {
            // _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
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
                    LogServer.WriteLog("新增"+list.Count, "StockDetial");
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
            int error = 0;
            do
            {
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {

                        // return db.SqlList<StockInfo>("select * from dbo.StockInfo with(nolock) where stockno not in(select Code from dbo.XqStockDayReport with(nolock) )");
                        return db.Select<StockInfo>().ToList();
                    }
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                    Thread.Sleep(10000);
                    error++;
                }
            } while (error<5);
            return null;

        }

        public void UpdateStockinfo(StockInfo stock)
        {
            try
            {

                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Update(stock);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }

        }
    }
}
