using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class ErrorInfoDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public ErrorInfoDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SpiderConfig>();
            }
        }

        /// <summary>
        /// 添加或者更新任务
        /// </summary>
        /// <param name="error"></param>
        public void SaveSpiderError(SpiderError error)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(error);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                }
            }
        }
    }
}
