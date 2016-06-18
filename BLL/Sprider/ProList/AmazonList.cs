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
    public class AmazonList : Amazon, IProList
    {
        public AmazonList()
        {
            Baseinfo = new SiteInfoDB().SiteById(4);
            SiteName = Baseinfo.SiteName;
        }

        public SiteClassInfo SiteCatInfo { get; set; }

        public ClassInfo CatInfo { get; set; }

        public RegProListInfo Reginfo { get; set; }

        public int AreaInfoId { get; set; }

        public SpiderError ErrorInfo { get; set; }

        public string SiteName { get; set; }

        public string getListUrl()
        {
            const string urlmode = "http://www.amazon.cn/s/ref=lp_{0}_pg_2?rh=n%3A{0}&page=$page&ie=UTF8&qid={0}";
            return string.Format(urlmode, SiteCatInfo.ClassId);
        }

        public string DownLoadPage(string pageUrl, int p)
        {
            HtmlAnalysis req = new HtmlAnalysis();
            req.RanAgent = false;
            return req.HttpRequest(pageUrl);
        }

        public bool HasProducts(string pageHtml)
        {
            if (pageHtml.Contains("没有找到任何与"))
                return false;
             if (!pageHtml.Contains("<div id=\"atfResults\" class=\"a-row s-result-list-parent-container\">"))
                return false;
            return true;
        }

        public int getPageNum(string pagehtml)
        {
            int maxpage;
            int total = RegGroupsX<int>(pagehtml, Reginfo.MaxpageReg);
            int pagecount = RegGroupsX<int>(pagehtml, "显示： 1-(?<x>\\d+)条");
            if (pagecount == 0 && total > 0)
                return 1;
            if (total == 0 || pagecount == 0)
            {
                ErrorInfo = new SpiderError();
                ErrorInfo.Lvevl = 4;
                ErrorInfo.SingleHtml = pagehtml;
                ErrorInfo.SiteId = Baseinfo.SiteId;
                ErrorInfo.ErrType = "获取最大页数错误";
                return 0;
            }
            if (total % pagecount == 0)
            {
                maxpage = total / pagecount;
            }
            else
            {
                maxpage = total / pagecount + 1;
            }
            return maxpage;
        }

        public System.Text.RegularExpressions.MatchCollection GetSigleProduct(string pageHtml)
        {
            return RegGroupCollection(pageHtml, Reginfo.SingleReg);
        }

        public string GetProConent(string pageHtml)
        {
            return RegGroupsX<string>(pageHtml, Reginfo.ListsReg);
        }

        public string GetSpName(string singleHtml)
        {
            return RegGroupsX<string>(singleHtml, Reginfo.TitleReg);
        }

        public decimal GetSpPrice(string singleHtml, string skuid)
        {
            return RegGroupsX<decimal>(singleHtml, Reginfo.PriceReg);
        }

        public string GetSpUrl(string singleHtml)
        {

            string id = RegGroupsX<string>(singleHtml, "data-asin=\"(?<x>.*?)\"|name=\"asin\" value=\"(?<x>.*?)\"");
            var url = "http://www.amazon.cn/dp/" + id;
            if (!ValidItemurl(url) || !regIsMatch(singleHtml, "data-asin=\"(?<x>.*?)\"|name=\"asin\" value=\"(?<x>.*?)\""))
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
            return url.Replace("http://www.amazon.cn/dp/", "");
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
                if (!singleHtml.Contains("product-reviews"))
                    return 0;
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

        public int GetSellType(string detial)
        {
            if (detial.Contains("由亚马逊直接销售和发货"))
                return 1;
            return 2;
        }

        public string GetBigPic(string detial)
        {
            return "";
        }

        public string GetBrand(string detial)
        {
            if (!regIsMatch(detial, "<a id=\"brand\" class=\"a-link-normal\" href=\".*?\">(?<x>.*?)</a>"))
            {
                if (Baseinfo == null)
                    Baseinfo = new SiteInfoDB().SiteById(4);
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "品牌获取失败",
                    SingleHtml = detial
                };
            }
            return RegGroupsX<string>(detial, "<a id=\"brand\" class=\"a-link-normal\" href=\".*?\">(?<x>.*?)</a>");
        }

        public string GetSkuDes(string detial)
        {
            return "";
        }

        public string GetOtherpic(string detial)
        {
            string imgs =RegGroupsX<string>(detial,"data-a-dynamic-image=\"(?<x>.*?)\"");
          
            if (imgs == null)
            {
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "获取更多图片失败",
                    SingleHtml = detial
                };
                return "";
            }
            var list = RegGroupCollection(imgs, "http://(?<x>.*?)\\&quot;");
            if (list == null)
            {
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "获取更多图片失败",
                    SingleHtml = detial
                };
                return "";
            }
            StringBuilder pics = new StringBuilder();
            foreach (Match img in list)
            {
                pics.Append("http://");
                pics.Append(img.Groups["x"].Value);
                pics.Append(",");
            }
          
            return pics.ToString();
        }

        public string GetShopName(string detial)
        {
            if (detial.Contains("由亚马逊直接销售和发货"))
                return "亚马逊自营";
            string name = RegGroupsX<string>(detial, "<span class='seller'>卖家：<b>(?<x>.*?)</b>|由 <b>(?<x>.*?)</b> 销售");
            if (name == null)
            {
                ErrorInfo = new SpiderError
                {
                    Lvevl = 4,
                    UrlPath = "",
                    SiteId = Baseinfo.SiteId,
                    ErrType = "店铺名称获取错误",
                    SingleHtml = detial
                };
                return "";
            }
            return name;

        }
    }
}
