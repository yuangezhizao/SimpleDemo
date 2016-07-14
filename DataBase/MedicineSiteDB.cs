using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
   public class MedicineSiteDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public MedicineSiteDB()
        {
            //_dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory=new OrmLiteConnectionFactory(ZnmDbConnectionString,SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<MedicineSiteInfo>();
            }
        }

        public  void AddMedicineSite(List<MedicineSiteInfo> list)
        {
            if (list == null)
            {
                LogServer.WriteLog("empty", "DBError");
                return;
            }

            using (var db = _dbFactory.OpenDbConnection())
            {
               
                try
                {
                    db.InsertAll(list);
                }
                catch (Exception ex)
                {
                    foreach (MedicineSiteInfo info in list)
                    {
                        try
                        {
                            db.Insert(info);
                        }
                        catch (Exception ex1)
                        {

                            LogServer.WriteLog(info.CertificateNo+"\t"+ex1.Message, "DBError");
                        }
                      
                    }
               
                    LogServer.WriteLog(ex, "DBError");
        
                }
            }
        }
    }
}
