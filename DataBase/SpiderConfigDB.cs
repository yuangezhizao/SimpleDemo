using System;
using System.Collections.Generic;
using Commons;
using ServiceStack.OrmLite;
using Mode;

namespace DataBase
{
    public class SpiderConfigDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public SpiderConfigDB()
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
        /// <param name="spiderConfigList"></param>
        public void SaveSpiderConfig(SpiderConfig spiderConfig)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(spiderConfig);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }


            }
        }

        public SpiderConfig GetById(int id)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.SingleById<SpiderConfig>(id);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }
            }
        }

        /// <summary>
        /// 加载所有任务
        /// </summary>
        /// <returns></returns>
        public List<SpiderConfig> LoadSpiderconfig()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    return db.Select<SpiderConfig>();
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    return null;
                }
            }
        }


    }
}
