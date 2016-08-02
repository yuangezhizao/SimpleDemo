using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase.Stock
{
    public class XqStockDayReportDB : OrmLiteFactory
    {
        public static OrmLiteConnectionFactory DbFactory;
        public XqStockDayReportDB()
        {
            if (DbFactory == null)
                DbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = DbFactory.OpenDbConnection())
            {
                db.CreateTable<XqStockDayReport>();
            }
        }

        public void AddXqStockDayReport(XqStockDayReport item)
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

        public void AddXqStockDayReport(List<XqStockDayReport> list)
        {

            using (var db = DbFactory.OpenDbConnection())
            {

                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    foreach (XqStockDayReport info in list)
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

        public List<XqStockDayReport> GetXqAllinfo()
        {
            try
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                  return  db.Select<XqStockDayReport>().ToList();
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
