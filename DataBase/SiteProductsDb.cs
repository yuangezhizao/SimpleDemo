using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using Mode;
using ServiceStack.OrmLite;

namespace DataBase
{
    public class SiteProductsDb : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public SiteProductsDb()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<SiteProInfo>();
            }
        }

        public SiteProInfo FindOne(string SiteSkuId)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    return db.Single<SiteProInfo>(x => x.SiteSkuId == SiteSkuId);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }

        }

        public void UpdateProinfo(SiteProInfo pro)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.UpdateNonDefaults(new SiteProInfo
                    {
                        SpPrice = pro.FloorPrice,
                        SpName=pro.SpName,
                        UpdateTime=pro.UpdateTime,
                        FloorPrice=pro.FloorPrice,
                        ClassId=pro.ClassId,
                        ProUrl=pro.ProUrl,
                        CommenCount=pro.CommenCount,
                        Promotions=pro.Promotions,
                        SellType=pro.SellType,
                        ShopName=pro.ShopName,
                        SiteCat=pro.SiteCat,
                        SiteProId = pro.SiteProId ?? ""
                    }, p => p.SiteSkuId == pro.SiteSkuId);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        public void AddSiteProInfo(SiteProInfo pro)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(pro);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        public void AddSiteProInfo(List<SiteProInfo> siteList)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(siteList);
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < siteList.Count; i++)
                {
                    AddSiteProInfo(siteList[i]);
                }
                LogServer.WriteLog(ex, "DBError");
            }
        }
    }

    public class TempUpdateSitePro : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public TempUpdateSitePro()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<TempSiteProInfo>();
            }
        }

        public void AddSiteProInfo(SiteProInfo pro)
        {
            try
            {
                TempSiteProInfo tempitem = Tempchangobj(pro);
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(tempitem);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        public void AddSiteProInfo(List<SiteProInfo> siteList)
        {
            try
            {
                List<TempSiteProInfo> tempitem = siteList.Cast<TempSiteProInfo>().ToList();
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(tempitem);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        private TempSiteProInfo Tempchangobj(SiteProInfo pro)
        {
            TempSiteProInfo temp = new TempSiteProInfo
            {
                AreaPriceDetial = pro.AreaPriceDetial,
                BigPic = pro.BigPic,
                ClassId = pro.ClassId,
                BrandId = pro.BrandId,
                SiteId = pro.SiteId,
                SingleDesc = pro.SingleDesc,
                ShopName = pro.ShopName,
                SellType = pro.SellType,
                SearchFiles = pro.SearchFiles,
                smallPic = pro.smallPic,
                spBrand = pro.spBrand,
                spSkuDes = pro.spSkuDes,
                SearchSkuJson = pro.SearchSkuJson,
                SiteCat = pro.SiteCat,
                SiteProId = pro.SiteProId,
                SiteSkuId = pro.SiteSkuId,
                SpName = pro.SpName,
                SpPrice = pro.SpPrice,
                IsSell = pro.IsSell,
                QzSort = pro.QzSort,
                CommenCount = pro.CommenCount,
                CommentUrl = pro.CommentUrl,
                CreateDate = pro.CreateDate,
                FloorPrice = pro.FloorPrice,
                Otherpic = pro.Otherpic,
                Promotions = pro.Promotions,
                ProUrl = pro.ProUrl,
                UpdateTime = pro.UpdateTime
            };
            return temp;
        }
    }

    public class TempAddSitePro : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;
        public TempAddSitePro()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<AddSiteProInfo>();
            }
        }

        public void AddSiteProInfo(SiteProInfo pro)
        {
            try
            {
                AddSiteProInfo tempitem = changobj(pro);
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Insert(tempitem);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }

        public void AddSiteProInfo(List<SiteProInfo> siteList)
        {
            try
            {
                //List<AddSiteProInfo> tempitem = siteList.Cast<AddSiteProInfo>().ToList();
                List<AddSiteProInfo> tempitem = siteList.Select(changobj).ToList();
                for (int i = 0; i < siteList.Count(); i++)
                {
                    AddSiteProInfo(siteList[i]);
                }
                using (var db = _dbFactory.OpenDbConnection())
                {
                    //db.CreateTable<AddSiteProInfo>(true);
                    db.Insert(tempitem);
                }
            }
            catch (Exception ex)
            {

                LogServer.WriteLog(ex, "DBError");
            }
        }

        private AddSiteProInfo changobj(SiteProInfo pro)
        {
            AddSiteProInfo temp = new AddSiteProInfo
            {
                AreaPriceDetial = pro.AreaPriceDetial,
                BigPic = pro.BigPic,
                ClassId = pro.ClassId,
                BrandId = pro.BrandId,
                SiteId = pro.SiteId,
                SingleDesc = pro.SingleDesc,
                ShopName = pro.ShopName,
                SellType = pro.SellType,
                SearchFiles = pro.SearchFiles,
                smallPic = pro.smallPic,
                spBrand = pro.spBrand,
                spSkuDes = pro.spSkuDes,
                SearchSkuJson = pro.SearchSkuJson,
                SiteCat = pro.SiteCat,
                SiteProId = pro.SiteProId,
                SiteSkuId = pro.SiteSkuId,
                SpName = pro.SpName,
                SpPrice = pro.SpPrice,
                IsSell = pro.IsSell,
                QzSort = pro.QzSort,
                CommenCount = pro.CommenCount,
                CommentUrl = pro.CommentUrl,
                CreateDate = pro.CreateDate,
                FloorPrice = pro.FloorPrice,
                Otherpic = pro.Otherpic,
                Promotions = pro.Promotions,
                ProUrl = pro.ProUrl,
                UpdateTime = pro.UpdateTime
            };
            return temp;
        }

      
    }
}
