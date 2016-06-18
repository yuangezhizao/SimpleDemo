using System;
using System.Collections.Generic;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class CommentDb: OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public CommentDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<CommentInfo>();
            }
        }
        /// <summary>
        /// 新增品论
        /// </summary>
        /// <param name="comments"></param>
        public void AddSiteProInfo(IEnumerable<CommentInfo> comments)
        {
            if (comments == null) throw new ArgumentNullException("comments");
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            try
            {
                lock (_dbFactory)
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        db.InsertAll(comments);
                    }
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");

            }
        }
        /// <summary>
        /// 获取商品的最后品论时间
        /// </summary>
        /// <param name="bjid"></param>
        /// <param name="siteid"></param>
        /// <returns></returns>
        public DateTime GetCommentEndDate(int bjid,int siteid)
        {
            int error = 0;
            do
            {
                _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        return db.Scalar<CommentInfo, DateTime>(x => Sql.Max(x.SendTime),
                            x => x.SiteId == siteid && x.Bjid == bjid);
                    }
                }
                catch (Exception ex)
                {
                    error++;
                    LogServer.WriteLog(ex, "DBError");
                    
                }
            } while (error > 3);
            return DateTime.MinValue;
        }
        /// <summary>
        /// 获取最新品论的 商品Id  Spid
        /// </summary>
        /// <returns></returns>
        public int GetLastComentItemId()
        {
            int error = 0;
            do
            {
                _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
                try
                {
                    using (var db = _dbFactory.OpenDbConnection())
                    {
                        var query = db.From<CommentInfo>().Select(x=>x.Spid).OrderByDescending(c => c.Id);
                        return db.Scalar<int>(query);
                    }
                }
                catch (Exception ex)
                {
                    error++;
                    LogServer.WriteLog(ex, "DBError");

                }
            } while (error > 3);
            return 0;
        }

    }
}
