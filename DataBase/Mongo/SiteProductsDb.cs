using System;
using System.Collections.Generic;
using Commons;
using Mode;
using MongoDB.Driver.Builders;

namespace DataBase.Mongo
{
   public class SiteProductsDb : BaseDB
    {
     
       public SiteProductsDb()
           : base("SiteProInfo")
        {
        }
       /// <summary>
       /// 查询
       /// </summary>
       /// <param name="SiteSkuId"></param>
       /// <returns></returns>
       public SiteProInfo FindOne(string SiteSkuId)
       {
           try
           {
                var query =  Query.EQ("_id", SiteSkuId);
                return Collection.FindOneAs<SiteProInfo>(query);
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

               var fquery = Query.EQ("_id", pro.SiteSkuId);
               var update = Update.Set("SpPrice", (double) pro.FloorPrice);
               update.Set("SpName", pro.SpName);
               update.Set("UpdateTime", pro.UpdateTime);
               update.Set("FloorPrice", (double)pro.FloorPrice);
               update.Set("ClassId", pro.ClassId);
               update.Set("ProUrl", pro.ProUrl);
               update.Set("CommenCount", pro.CommenCount);
               update.Set("Promotions", pro.Promotions ?? "");
               update.Set("SellType", pro.SellType);
               update.Set("ShopName", pro.ShopName ?? "");
               update.Set("SiteCat", pro.SiteCat);
               update.Set("SiteProId", pro.SiteProId ?? "");

               Collection.Update(fquery, update);
           }
           catch (Exception ex)
           {
               LogServer.WriteLog(ex, "DBError");
           }
           //Collection.FindAndModify(fquery,new IMongoSortBy(), new SiteProInfo { SpPrice = pro.SpPrice });
       }


       public void AddSiteProInfo(SiteProInfo pro)
       {
           try
           {
               Collection.Save(pro);
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
               Collection.InsertBatch(siteList);
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

    public class TempUpdateSitePro : BaseDB
    {

        public TempUpdateSitePro()
            : base("TempUpdateSitePro")
        {
        }

        public void AddSiteProInfo(SiteProInfo pro)
        {
            try
            {
                Collection.Save(pro);
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
                Collection.InsertBatch(siteList);
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

    public class TempAddSitePro : BaseDB
    {

        public TempAddSitePro()
            : base("TempAddSitePro")
        {
        }

        public void AddSiteProInfo(SiteProInfo pro)
        {
            try
            {
                Collection.Save(pro);
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
                Collection.InsertBatch(siteList);
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

}
