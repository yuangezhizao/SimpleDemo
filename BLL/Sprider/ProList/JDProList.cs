using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.ProList
{
    public class JdProList : JingDong,IProList
    {
        public JdProList()
        {
            Baseinfo = new SiteInfoDB().SiteById(1);
            SiteName = Baseinfo.SiteName;
        }

        public SiteClassInfo SiteCatInfo { get; set; }


        public ClassInfo CatInfo { get; set; }

        public RegProListInfo Reginfo { get; set; }

        public int AreaInfoId { get; set; }

        public SpiderError ErrorInfo { get; set; }

        private void getProlistPrice(string page_html)
        {
            string prolist = RegGroupsX<string>(page_html, "<ul class=\"list-h\">(?<x>.*?)</ul>\\s+</div><!--plist end-->|<ul class=\"list-h[^\\\"]*?\">(?<x>.*?)<div class=\"m clearfix\">|<ul class=\"list-h\">(?<x>.*?)<script type=\"text/javascript\">|<div id=\"plist\" class=\"goods-list gl-type-6\">(?<x>.*?)<div id=\"J_bottomPage\" class=\"p-wrap\">");
            string Cityinfo = "1";
            if (prolist ==null)
                return;
            var skulist = RegGroupCollection(prolist, "sku=\"(?<x>.*?)\"|data-sku=\"(?<x>.*?)\"");
            string results = "";
            if (skulist == null)
                return;
            foreach (Match item in skulist)
            {
                results += "J_" + item.Groups["x"].Value + ",";
            }
            prolist = results.TrimEnd(',');


            //if (IsAreaspider && siteAreaInfo != null)
            //{
            //    Cityinfo = "&area=" + siteAreaInfo.urlInfo;
            //}
            //else
            //{
            switch (Cityinfo)
            {
                case "1":
                    Cityinfo = "&area=1-72-4137";
                    break;
                case "2":
                    Cityinfo = "&area=2-2811-2860";
                    break;
                case "3":
                    Cityinfo = "&area=19-1601-51091";
                    break;
                case "4":
                    Cityinfo = "&area=7-412-4337";
                    break;
                case "5":
                    Cityinfo = "&area=22-1930-4284";
                    break;
                case "6":
                    Cityinfo = "&area=8-560-50819";
                    break;
                case "7":
                    Cityinfo = "&area=27-2376-4343";
                    break;
            }
            //}

            if (prolist == "")
                return;

            string priceurl = "http://p.3.cn/prices/mgets?skuIds=" + prolist + "&type=1" + Cityinfo;

            string price_json = HtmlAnalysis.Gethtmlcode(priceurl);
            if (price_json == "" || price_json.Length < 10)
            {
                //Error = new ErrorInfo();
                //Error.UrlPath = checknewProduct.checkurl;
                //Error.Lvevl = 4;
                //Error.SiteId = checknewProduct.siteid;
                //Error.ErrType = "正则验证测试";
                //Error.ErrDetial = "商品价格列表信息错误";
                return;
            }



            //price_json = price_json.Substring(0, price_json.LastIndexOf('}'));

            //var list = price_json.Split('}');
            priceList = new Dictionary<string, string>();
            var jsondict = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, string>[]>(price_json);
            foreach (var item in jsondict)
            {
                string tempid = item["id"].Replace("J_", "");
                if (priceList.ContainsKey(tempid))
                    continue;
                priceList.Add(tempid, item["p"]);
            }

            //for (int i = 0; i < list.Length; i++)
            //{
            //    string price = Regex.Match(list[i], "\"p\":\"(?<x>.*?)\"", base.ro).Groups["x"].Value;
            //    string id = Regex.Match(list[i], "\"id\":\"J_(?<x>.*?)\"", base.ro).Groups["x"].Value;
            //    var temp = priceList.Where(c => c.Key == id);
            //    if (temp.Count() == 0)
            //        priceList.Add(id, price);
            //}

        }

        public string getListUrl()
        {
           return "http://list.jd.com/list.html?cat=" + SiteCatInfo.ClassId + "&page=$page&stock=0";
        }

        public string DownLoadPage(string pageUrl, int p)
        {
            var temp=HtmlAnalysis.Gethtmlcode(pageUrl);
            getProlistPrice(temp);
            return temp;
        }

        public int getPageNum(string pagehtml)
        {
            return RegGroupsX<int>(pagehtml, Reginfo.MaxpageReg);
        }

        public MatchCollection GetSigleProduct(string pageHtml)
        {
            var res = RegGroupCollection(pageHtml, Reginfo.SingleReg);
            return res;
        }


        public bool HasProducts(string pageHtml)
        {
            if (pageHtml.Contains("id=\"plist\""))
                return true;
      
            ErrorInfo = new SpiderError();
            ErrorInfo.Lvevl = 5;
            ErrorInfo.ErrType = "列表也没有产品";
            ErrorInfo.PageHtml = pageHtml;
            //ErrorInfo.UrlPath = pageUrl;
            ErrorInfo.SiteId = Baseinfo.SiteId;
            ErrorInfo.SiteName = Baseinfo.SiteName;
            return false;
        }

        public string GetProConent(string pageHtml)
        {
            return RegGroupsX<string>(pageHtml, Reginfo.ListsReg);
        }


        public string GetSpName(string singleHtml)
        {
            var title = RegGroupsX<string>(singleHtml, Reginfo.TitleReg);
            if (!ValidItemName(title))
            {
                ErrorInfo = new SpiderError();
                ErrorInfo.Lvevl = 4;
                ErrorInfo.SingleHtml = singleHtml;
                ErrorInfo.SiteId = Baseinfo.SiteId;
                ErrorInfo.ErrType = "产品标题错误";
                return "";
            }
            return title;
        }

        protected Dictionary<string, string> priceList { get; set; }
        public decimal GetSpPrice(string singleHtml,string skuid)
        {
            decimal itemPrice ;
            string itemid = skuid.Substring(2);
            if (priceList != null && priceList.ContainsKey(itemid))
            {
                decimal.TryParse(priceList[itemid], out itemPrice);
                if (itemPrice > 0 || itemPrice==-1)
                    return itemPrice;
              
            }

            string tempid = RegGroupsX<string>(singleHtml, "data-skuid=\"(?<x>.*?)\"|skuid='(?<x>.*?)'|sku=\"(?<x>.*?)\"");

            if (tempid == "")
                return 0;
            string priceurl = "http://p.3.cn/prices/mgets?skuIds=J_" + tempid + "&type=1" ;
            string price_json = HtmlAnalysis.Gethtmlcode(priceurl);
            itemPrice =RegGroupsX<decimal>(price_json, "\"p\":\"(?<x>.*?)\"");
            return itemPrice;
        }


        public string GetSpUrl(string singleHtml)
        {
            var url = RegGroupsX<string>(singleHtml, Reginfo.UrlReg);
            if (!ValidItemurl(url))
            {
                ErrorInfo = new SpiderError();
                ErrorInfo.Lvevl = 4;
                ErrorInfo.SingleHtml = singleHtml;
                ErrorInfo.SiteId = Baseinfo.SiteId;
                ErrorInfo.ErrType = "产品url错误";
                return "";
            }
            return url;
        }


        public string GetItemSku(string url)
        {
            var tempid = RegGroupsX<string>(url, Reginfo.SkuReg);
            if (!ValidItemurl(url))
            {
                ErrorInfo = new SpiderError();
                ErrorInfo.Lvevl = 4;
                ErrorInfo.UrlPath = url;
                ErrorInfo.SiteId = Baseinfo.SiteId;
                ErrorInfo.ErrType = "产品id错误";
                return "";
            }
            tempid = Baseinfo.SiteId + "|" + tempid;
            return tempid;
        }

        public string GetSmallPic(string singleHtml)
        {
            if (!regIsMatch(singleHtml, Reginfo.PicReg))
            {
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "获取小图错误",
                    SingleHtml = singleHtml
                };
                return "";
            }
            return RegGroupsX<string>(singleHtml, Reginfo.PicReg);
        }


        public int GetComments(string singleHtml)
        {
            if (!regIsMatch(singleHtml, Reginfo.CommentCountReg))
            {
                if (Baseinfo==null)
                    Baseinfo = new SiteInfoDB().SiteById(1);
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "获取评论错误",
                    SingleHtml = singleHtml
                };
                return 0;
            }
            var count = RegGroupsX<int>(singleHtml, Reginfo.CommentCountReg);
            return count;
        }


        public int GetSellType(string singleHtml)
        {
            var spbh = RegGroupsX<string>(singleHtml, Reginfo.SkuReg);
            if (spbh.Length < 11)
                return 1;
            return 2;
        }


        public string SiteName { get; set; }


        public string GetBigPic(string detial)
        {
            return "";
        }

        public string GetBrand(string detial)
        {
            return RegGroupsX<string>(detial,
                "<a href=\"http://www.jd.com/pinpai/\\d+-\\d+.html\" target=\"_blank\">(?<x>.*?)</a></li>");

           
        }

        public string GetSkuDes(string detial)
        {
            string skudetial = RegGroupsX<string>(detial,
               "<ul class=\"detail-list\">(?<x>.*?)</ul>");
            if (string.IsNullOrEmpty(skudetial))
                return "";
            var list = RegGroupCollection(skudetial, "<li( title=\".*?\"|)>(?<x>.*?)</li>");
     
            if (list == null || list.Count == 0)
            {
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "获取sku参数错误",
                    SingleHtml = detial
                };
                return "";
            }
            StringBuilder res = new StringBuilder();
            const string fomat = "\"{0}\":\"{1}\"";
            res.Append("[");
            foreach (Match li in list)
            {
                var txt =WordCenter.FilterHtml( li.Groups["x"].Value).Replace('"','\'');
                string[] templist = txt.Split('：');
                if (templist.Length == 2)
                {
                    res.Append("{");
                    res.AppendFormat(fomat, templist[0], templist[1]);
                    res.Append("},");
                }
            }
            res.Remove(res.Length-1,1);
            res.Append("]");

            return res.ToString();
        }


        public string GetOtherpic(string detial)
        {
            string tempimg = RegGroupsX<string>(detial, "<div class=\"spec-items\">(?<x>.*?)</div>");
            if (string.IsNullOrEmpty(tempimg))
                return "";
            var list = RegGroupCollection(tempimg, "src=\"(?<x>.*?)\"");
            if(list==null||list.Count==0)
                return "";
            StringBuilder imgs = new StringBuilder();
            foreach (Match src in list)
            {
                imgs.Append(src.Groups["x"].Value);
                imgs.Append(",");
            }
            return imgs.ToString().TrimEnd(',');
        }


        public string GetShopName(string detial)
        {
            if (detial.Contains("shopId:'0',"))
                return "京东自营";
            string shopname = RegGroupsX<string>(detial,
                "<li>店铺：<a href=\"http://mall.jd.com/index-\\d+.html\" target=\"_blank\">(?<x>.*?)</a></li>");
            if (string.IsNullOrEmpty(shopname))
                return "";
            return shopname;
        }
    }
}
