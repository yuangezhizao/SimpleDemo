using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class SmsHistoryDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public SmsHistoryDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SmsHistory>();
            }
        }
        public void AddSmsHistory(List<SmsHistory> list)
        {
            try
            {

                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.InsertAll(list);
                    foreach (SmsHistory info in list)
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
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        public void AddSmsHistory(SmsHistory sms)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(sms);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex, "DBError");
            }
        }

        public List<SmsHistory> GetAllinfo()
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<SmsHistory>().ToList();
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
