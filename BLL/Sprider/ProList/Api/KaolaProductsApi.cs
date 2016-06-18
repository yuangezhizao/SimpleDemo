using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using SpriderProxy;

namespace BLL.Sprider.ProList.Api
{
    public class KaolaiProductsApi : BaseSiteInfo, IApiProList
   {
       private List<string> catListUrl;

       public KaolaiProductsApi()
        {
            Baseinfo = new SiteInfoDB().SiteById(241);
        }

        public void GetAllProducts()
        {
            const string domain = "http://www.kaola.com/";
            string homepage = HtmlAnalysis.Gethtmlcode(domain);
            catListUrl = new List<string>();
            var catlist = RegGroupCollection(homepage, "http://www.kaola.com/activity/detail/(?<x>\\d+).html");
            for (int i = 0; i < catlist.Count; i++)
            {
                if (!catListUrl.Contains(catlist[i].Groups["x"].Value))
                    catListUrl.Add(catlist[i].Groups["x"].Value);
            }
            for (int i = 0; i < catListUrl.Count; i++)
            {
                try
                {
                    string url = string.Format("http://www.kaola.com/activity/detail/{0}.html", catListUrl[i]);
                    SaverProduct(url);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex, "apiError");
                }
        
            }
        }


        public void SaverProduct(string url)
        {
            string homepage = HtmlAnalysis.Gethtmlcode(url);

            var catlist = RegGroupCollection(homepage, "http://www.kaola.com/activity/detail/(?<x>\\d+).html");
            for (int i = 0; i < catlist.Count; i++)
            {

                if (!catListUrl.Contains(catlist[i].Groups["x"].Value))
                    catListUrl.Add(catlist[i].Groups["x"].Value);
            }
            var itemList = RegGroupCollection(homepage, "/product/(?<x>\\d+).html");
            if (itemList == null)
                return;
            List<string> idlist = new List<string>();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (!idlist.Contains(itemList[i].Groups["x"].Value))
                    idlist.Add(itemList[i].Groups["x"].Value);
            }


            List<SiteProInfo> items = new List<SiteProInfo>();
            for (int i = 0; i < idlist.Count; i++)
            {
                var item = getProduct(idlist[i]);
                if (item != null)
                    items.Add(item);
            }

            //var items = idlist.Select(getProduct).Where(item => item != null);
            if (idlist.Count > 0)
            {
                new SiteProInfoDB().AddSitePro(items);
            }

        }

        private SiteProInfo getProduct(string proid)
        {
            if (!ValidItemId(proid))
                return null;
            string url = string.Format("http://www.kaola.com/product/{0}.html", proid);

            string propage = HtmlAnalysis.Gethtmlcode(url);
            SiteProInfo item = new SiteProInfo();

            item.SiteSkuId = string.Format("{0}|{1}", Baseinfo.SiteId, proid);
            item.SiteCat = RegGroupsX<string>(propage, "categoryId:'(?<x>\\d+)'");
            item.SpName = RegGroupsX<string>(propage, "name:'(?<x>.*?)'|<dt class=\"product-title\">(?<x>.*?)</dt>");


            item.SpPrice = RegGroupsX<decimal>(propage, "price:'(?<x>.*?)'|id=\"js_currentPrice\">¥<span>(?<x>.*?)</span>");

            item.smallPic = RegGroupsX<string>(propage, "imgUrl:'(?<x>.*?)'|<div id=\"showImgBox\">\\s*<img src=\"(?<x>.*?)\"|'showImgBox','(?<x>.*?)'");
            item.BigPic = RegGroupsX<string>(propage, "<div id=\"showDetails\"><img src=\"(?<x>.*?)\"");
            item.CommenCount = RegGroupsX<int>(propage, "<p class=\"commNum\">\\((?<x>\\d+)人评价\\)</p>");
            item.ProUrl = url;
            item.Promotions = RegGroupsX<string>(propage, "promotion:'(?<x>.*?)'|<dt class=\"subTit\">(?<x>.*?)</dt>");
            item.spBrand = RegGroupsX<string>(propage, "brand:'(?<x>.*?)'");
            item.SearchFiles = RegGroupsX<string>(propage, "category:'(?<x>.*?)'");
            item.IsSell = !propage.Contains("暂时缺货");
            if (string.IsNullOrEmpty(item.SpName) || string.IsNullOrEmpty(item.smallPic) || string.IsNullOrEmpty(item.ProUrl) || item.SpPrice == 0)
                return null;
            item.UpdateTime = DateTime.Now;
            item.CreateDate = DateTime.Now;
            item.SellType = 1;

            return item;
        }

        private bool ValidItemId(string itemid)
        {
            if (string.IsNullOrEmpty(itemid))
                return false;
            int id;
            return int.TryParse(itemid, out id);
        }
        public override bool ValidCatId(string catId)
        {
            throw new NotImplementedException();
        }


        public void AddNewProducts()
        {
            throw new NotImplementedException();
        }
   }
}
