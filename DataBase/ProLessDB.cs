using Commons;
using Mode;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBase
{
    public class ProLessDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public List<ProLessInfo> GetProLessList()
        {
            string  Sqlstr="select top 100 id,spbh as SkuId,spname as ProName, spPrice as ProPrice , spurl as ProUrl,sppic as ProImg,youhui as Promotions,ppid as BrandID,pingpai as BrandName,HisZeKou as HisDiscounts,oldPrice as HisDiscounts,jiangjia as balance,minPrice as floorPrice,ziying as SellType,classid ,siteid,siteclass asSiteClassId, createdate from [dbo].[temp_PriceLess] with(nolock) order by id desc";
            _dbFactory = new OrmLiteConnectionFactory(MmbUpdateTempEConnectionString, SqlServerDialect.Provider);
            try
            {
                
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Select<ProLessInfo>(Sqlstr);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                throw;
            }

        }
    }
}
