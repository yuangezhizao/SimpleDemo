using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace DataBase
{
    public class ApiConfigDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public ApiConfigDB()
        {
            //_dbFactory = new OrmLiteConnectionFactory(ConnectionString,SqliteDialect.Provider);

            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                
                db.CreateTable<ApiConfig>();
            }
        }

        public void GetApiConfig(ApiConfig api)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Single<ApiConfig>(c=>c.ApiId==api.ApiId);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex, "DBError");
            }

        }

        public List<ApiConfig> GetAllConfig()
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<ApiConfig>().ToList();
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex, "DBError");
                return null;
            }

        }

        public void SaveApiConfig(ApiConfig api)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Save(api);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex, "DBError");
            }
        }
    }
}
