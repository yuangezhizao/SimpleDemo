using System;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using Mode;
using Commons;

namespace DataBase
{
    public class ImageDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public ImageDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<ImageInfo>();
            }
        }

        public ImageInfo ImageByUrl(string url)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Single<ImageInfo>(p => p.FromUrl == url);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public ImageInfo ImageByName(string name)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Single<ImageInfo>(p => p.ImgName == name);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }
        }

        public static void AddSiteInfo(IEnumerable<ImageInfo> imgList)
        {
            if (imgList == null) throw new ArgumentNullException("imgList");
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.InsertAll(imgList);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

        public void AddSiteInfo(ImageInfo img)
        {
            if (img == null) throw new ArgumentNullException("img");
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                try
                {
                    db.Save(img);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    throw;
                }

            }
        }

    }
}
