using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mode;

namespace BLL.Sprider.ProList
{
    public interface IProList
    {
        SiteClassInfo SiteCatInfo { get; set; }
        ClassInfo CatInfo { get; set; }
        RegProListInfo Reginfo { get; set; }
        int AreaInfoId { get; set; }
        SpiderError ErrorInfo { get; set; }
        string SiteName { get; set; }
        string getListUrl();

        string DownLoadPage(string pageUrl, int p);

        bool HasProducts(string pageHtml);

        int getPageNum(string page_html);

        System.Text.RegularExpressions.MatchCollection GetSigleProduct(string pageHtml);

        string GetProConent(string pageHtml);


        string GetSpName(string singleHtml);

        decimal GetSpPrice(string singleHtml,string skuid);

        string GetSpUrl(string singleHtml);

        string GetItemSku(string url);

        string GetSmallPic(string singleHtml);

        int GetComments(string singleHtml);

        int GetSellType(string singleHtml);

        string GetBigPic(string detial);

        string GetBrand(string detial);

        string GetSkuDes(string detial);

        string GetOtherpic(string detial);

        string GetShopName(string detial);
    }
}
