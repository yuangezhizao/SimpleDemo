using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class JdJosApiDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public JdJosApiDB()
        {
            //_dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            _dbFactory = new OrmLiteConnectionFactory(ZnmDbConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<JdJosApi>();
            }
        }

        public void AddJosApi(JdJosApi jos)
        {
            if (jos == null) throw new ArgumentNullException("JdJosApi");
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    var old = db.Single<JdJosApi>(c => c.AppKey == jos.AppKey);
                    if(old==null)
                    db.Save(jos);
                    else
                    {
                        old.AccessTime = jos.AccessTime;
                        old.ExpiresTime = jos.ExpiresTime;
                        old.AccessToken = jos.AccessToken;
                        db.Update(old);
                    }
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public JdJosApi GetJosApi(int id)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {

                    return db.Single<JdJosApi>(c => c.Id == id);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }
        }

    }
}
