using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
   public class ApiRequstHistroyDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public ApiRequstHistroyDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString,SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<APIRequstHistroy>();
            }
        }
        public void AddApiHistory(List<APIRequstHistroy> list)
        {
            try
            {

                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.InsertAll(list);
                    foreach (APIRequstHistroy info in list)
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

        public void AddApiHistory(APIRequstHistroy sms)
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

    }
}
