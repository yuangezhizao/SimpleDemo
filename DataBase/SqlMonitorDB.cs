using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class SqlMonitorDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public SqlMonitorDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(mmbpriceDBConnectionString, SqlServerDialect.Provider);
        }

        /// <summary>
        /// 查看是否存在数据库死锁
        /// </summary>
        public bool IsLocked()
        {
            string lockedSql = "SELECT   count(1) FROM    sys.sysprocesses where blocked!=null";
            using (var db = _dbFactory.OpenDbConnection())
            {
                int total = db.Scalar<int>(lockedSql);
                if (total > 0)
                    return true;
                return false;
            }
        }

    }
}
