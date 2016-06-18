using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class PromotionsDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public PromotionsDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ConnectionString, SqliteDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<PromotionsInfo>();
            }
        }

        public void SaveRegProList(IEnumerable<PromotionsInfo> promoList)
        {
            if (promoList == null) throw new ArgumentNullException("promoList");
            foreach (PromotionsInfo item in promoList)
            {
                try
                {
                    SavePromo(item);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "DBError");
                    continue;
                }
            }
        }

        public void SavePromo(PromotionsInfo info)
        {
            try
            {
                
                using (var db = _dbFactory.OpenDbConnection())
                {
                    var old = db.Single<PromotionsInfo>(p => p.SiteCatID == info.SiteCatID && p.SkuId == info.SkuId);
                    if (old != null)
                    { 
                        info.Id = old.Id;
                    info.CreateDate = old.CreateDate;
                        db.Update(info);
                    }
                    else
                    {
                        db.Insert(info);
                    }
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }

        }


        public List<PromotionsInfo> LoadPromotionsInfo()
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                return db.Select<PromotionsInfo>();
            }
        }
    }
}
