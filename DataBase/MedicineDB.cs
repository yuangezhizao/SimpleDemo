using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
   public class MedicineDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public MedicineDB()
        {
            //_dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory=new OrmLiteConnectionFactory(ZnmDBConnectionString,SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<MedicineInfo>();
            }
        }

        public  void AddMedicineInfo(List<MedicineInfo> list)
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
                    foreach (MedicineInfo info in list)
                    {
                        try
                        {
                            db.Insert(info);
                        }
                        catch (Exception ex1)
                        {

                            LogServer.WriteLog(info.ApprovalNum+"\t"+ex1.Message, "DBError");
                        }
                      
                    }
               
                    LogServer.WriteLog(ex, "DBError");
        
                }
            }
        }
    }
}
