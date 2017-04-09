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
        /// <summary>
        /// 获取当天的更新量
        /// </summary>
        /// <returns></returns>
        public long GetXqStockDayCount()
        {
            try
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                   var result= db.Count<XqStockDayReport>(c => c.CreateDate > DateTime.Now.Date);
                    return result;

                }
              
            }
            catch (Exception ex1)
            {
                LogServer.WriteLog(ex1.Message, "DBError");
            }
            return 0;
        }
        /// <summary>
        /// 删除当天的更新记录
        /// </summary>
        public void DelCurrentDatXqStock()
        {
            try
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                    db.Delete<XqStockDayReport>(c => c.CreateDate > DateTime.Now.Date);
                }

            }
            catch (Exception ex1)
            {
                LogServer.WriteLog(ex1.Message, "DBError");
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
