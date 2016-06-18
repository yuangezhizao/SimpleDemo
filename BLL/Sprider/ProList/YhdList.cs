using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.ProList
{
    public class YhdList : Yhd, IProList
    {
        public YhdList()
        {
            Baseinfo = new SiteInfoDB().SiteById(13);
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
            string mode = "http://list.yhd.com/{0}/b/a-s1-v0-p$page-price-d0-f0-m1-rt0-pid-mid0-k";
            return string.Format(mode, SiteCatInfo.ClassId);
           
        }

        public string DownLoadPage(string pageUrl, int p)
        {
            return HtmlAnalysis.Gethtmlcode(pageUrl);
        }

        public bool HasProducts(string pageHtml)
        {
            if (pageHtml.Contains("<ul class=\"clearfix\" id=\"itemSearchList\">"))
                return true;
            return false;
        }

        public int getPageNum(string page_html)
        {
            return RegGroupsX<int>(page_html, Reginfo.MaxpageReg);
            //if (total % 72 == 0)
            //{
            //    return total / 72;
            //}
            //return total / 72 + 1;
        }

        public System.Text.RegularExpressions.MatchCollection GetSigleProduct(string pageHtml)
        {
            var res = RegGroupCollection(pageHtml, Reginfo.SingleReg);
            return res;
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

        public decimal GetSpPrice(string singleHtml, string skuid)
        {
            var res = RegGroupsX<decimal>(singleHtml, Reginfo.PriceReg);
            if (res == 0)
            {
                ErrorInfo = new SpiderError();
                ErrorInfo.Lvevl = 4;
                ErrorInfo.SingleHtml = singleHtml;
                ErrorInfo.SiteId = Baseinfo.SiteId;
                ErrorInfo.ErrType = "产品价格错误";
                return 0;
            }
            return res;
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
                if (Baseinfo == null)
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

            if (singleHtml.Contains("本商品由1号店自营提供"))
                return 1;
            return 2;
        }

        public string GetBigPic(string detial)
        {
            return "";
        }

        public string GetBrand(string detial)
        {
            string brandName = RegGroupsX<string>(detial, "id=\"brandName\" value=\"(?<x>.*?)\">");

            return brandName ?? "";
        }

        public string GetSkuDes(string detial)
        {
            string skudetial = RegGroupsX<string>(detial, "<dt>规格参数\r\n<a id=\"medica_record\"(?<x>.*?)</dl>");
            if (skudetial == null)
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
            var list = RegGroupCollection(skudetial, "<dd title=\"(?<x>.*?)\" >");
            StringBuilder res = new StringBuilder();
            const string fomat = "\"{0}\":\"{1}\"";
            res.Append("[");
            if (list.Count == 0)
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
            foreach (Match item in list)
            {
                string title = item.Groups["x"].Value;
                string[] templist = title.Split('：');
                if (templist.Length == 2)
                {
                    res.Append("{");
                    res.AppendFormat(fomat, templist[0], templist[1]);
                    res.Append("},");
                }
            }
            res.Remove(res.Length - 1, 1);
            res.Append("]");
            return res.ToString();
        }

        public string GetOtherpic(string detial)
        {
               string tempimg = RegGroupsX<string>(detial, "<ul class=\"imgtab_con\">(?<x>.*?)</ul>");
            if (string.IsNullOrEmpty(tempimg))
                return "";
            var list = RegGroupCollection(tempimg, "src=\"(?<x>.*?)\"");
            if (list == null || list.Count == 0)
                return "";
            StringBuilder imgs = new StringBuilder();
            foreach (Match src in list)
            {
                imgs.Append(src.Groups["x"].Value.Replace("_60x60", "_280x280"));
                imgs.Append(",");
            }
            return imgs.ToString().TrimEnd(',');

        }

        public string GetShopName(string detial)
        {
            string shopname = RegGroupsX<string>(detial, "id=\"companyName\" value=\"(?<x>.*?)\">");
            if (string.IsNullOrEmpty(shopname))
                return "";
            return shopname;
        }
    }
}
