using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis.Gome;

namespace BLL.Sprider.SitePromo
{
    public class GmSitePromo : Gome, ISitePromo
    {
        public GmSitePromo()
        {
            if (Baseinfo==null)
            Baseinfo = new SiteInfoDB().SiteById(8);
        }

        const string connFormat = "#网购福利#【{5}{6}￥{0}】,{1} 时间：{2}至{3},{4}";
        private void GetLimitbuy()
        {
            GomeCpsApi gmApi = new GomeCpsApi();
            gmApi.Format = "json";
            int totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                gmApi.PageNo = i;
                string urlqinggou = gmApi.GetLimitbuyUrl();
                string qgResult = HtmlAnalysis.Gethtmlcode(urlqinggou);
                var qgList = ServiceStack.Text.JsonSerializer.DeserializeFromString<PromoResponseEF>(qgResult);
                if (qgList == null || qgList.promo_items == null || qgList.promo_items.Count == 0)
                    return;
                if (i == 1)
                {
                    totalPage = qgList.total_count / gmApi.PageSize;
                    if (qgList.total_count % gmApi.PageSize != 0)
                        totalPage++;
                }
                List<PromotionsInfo> list = new List<PromotionsInfo>();
                foreach (var item in qgList.promo_items)
                {
                    try
                    {
                        DateTime startTime;
                        DateTime stopTime;
                        DateTime.TryParse(item.end_date, out stopTime);
                    
                        if (stopTime < DateTime.Now)
                            continue;
                        DateTime.TryParse(item.start_date, out startTime);
                        string tempstartTime = startTime.ToString("MM月dd日 HH:mm:ss").Replace(" 00:00:00", "");
                        string tempstopTime = stopTime.ToString("MM月dd日 HH:mm:ss").Replace(" 00:00:00", "");
                        item.promo_name = item.promo_name.Replace("【", "[").Replace("】", "]").Replace("#","");
                        string conn = string.Format(connFormat, item.promo_price, item.promo_name, tempstartTime, tempstopTime,
                            "http://cps.gome.com.cn/home/JoinUnion?sid=4347&wid=123&feedback=&to=" + item.product_url_wap,"国美","抢购");
                        
                        PromotionsInfo temp = new PromotionsInfo
                        {
                            PromoPrice = item.promo_price,
                            PromoPic= item.picture_url,
                            PromoStartTime = startTime,
                            PromoStopTime = stopTime,
                            PromoWapUrl = item.product_url_wap,
                            PromoTitle = item.promo_name,
                            PromoUrl = item.product_url,
                            SiteId = Baseinfo.SiteId,
                            SiteName=Baseinfo.SiteName,
                            SiteCatID = item.category_id,
                            SiteProId = item.product_id,
                            SkuId = item.sku_id,
                            PromoType="抢购",
                            PromoWeibo=conn,
                            SkuName = item.sku_name,
                            UpdateTime = DateTime.Now,
                            PromoDesc = "",
                            CreateDate = DateTime.Now
                        };
                        list.Add(temp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                new PromotionsDB().SaveRegProList(list);
               

            }
        }

        private void GetGroupPurchase()
        {
            GomeCpsApi gmApi = new GomeCpsApi();
            gmApi.Format = "json";
            int totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                gmApi.PageNo = i;
                string urlqinggou = gmApi.GetGroupPurchase();
                string qgResult = HtmlAnalysis.Gethtmlcode(urlqinggou);
                var qgList = ServiceStack.Text.JsonSerializer.DeserializeFromString<GroupPurchaseEF>(qgResult);
                if (qgList == null || qgList.promo_items == null || qgList.promo_items.Count == 0)
                    return;
                if (i == 1)
                {
                    totalPage = qgList.total_count / gmApi.PageSize;
                    if (qgList.total_count % gmApi.PageSize != 0)
                        totalPage++;
                }
                List<PromotionsInfo> list = new List<PromotionsInfo>();
                foreach (var item in qgList.promo_items)
                {
                    try
                    {
                        DateTime startTime;
                        DateTime stopTime;
                        DateTime.TryParse(item.end_date, out stopTime);

                        if (stopTime < DateTime.Now)
                            continue;
                        DateTime.TryParse(item.start_date, out startTime);
                        string tempstartTime = startTime.ToString("MM月dd日 HH:mm:ss").Replace(" 00:00:00", "");
                        string tempstopTime = stopTime.ToString("MM月dd日 HH:mm:ss").Replace(" 00:00:00", "");
                        item.promo_name = item.sku_name.Replace("【", "[").Replace("】", "]").Replace("#", "");
                        string conn = string.Format(connFormat, item.promo_price, item.promo_name, tempstartTime, tempstopTime,
                            "http://cps.gome.com.cn/home/JoinUnion?sid=4347&wid=123&feedback=&to=" + item.product_url_wap,"国美", "团购");

                        PromotionsInfo temp = new PromotionsInfo
                        {
                            PromoPrice = item.promo_price,
                            PromoPic = item.picture_url,
                            PromoStartTime = startTime,
                            PromoStopTime = stopTime,
                            PromoWapUrl = item.product_url_wap,
                            PromoTitle = item.promo_name,
                            PromoUrl = item.product_url,
                            SiteId = Baseinfo.SiteId,
                            SiteName = Baseinfo.SiteName,
                            SiteCatID = item.category_id,
                            SiteProId = item.product_id,
                            SkuId = item.sku_id,
                            PromoType = "团购",
                            PromoWeibo = conn,
                            SkuName = item.sku_name,
                            UpdateTime = DateTime.Now,
                            PromoDesc = "",
                            CreateDate = DateTime.Now
                        };
                        list.Add(temp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                new PromotionsDB().SaveRegProList(list);


            }
        }

        public void SaveSitePromo()
        {
            GetGroupPurchase();
            GetLimitbuy();
           
        }
    }
}
