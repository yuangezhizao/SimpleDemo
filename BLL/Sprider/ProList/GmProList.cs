using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis.Gome;

namespace BLL.Sprider.ProList
{
   public class GmProList : Gome, IProList
    {
       public GmProList()
        {
            Baseinfo = new SiteInfoDB().SiteById(8);
            SiteName = Baseinfo.SiteName;
        }
        public Mode.SiteClassInfo SiteCatInfo { get; set; }

        public Mode.ClassInfo CatInfo { get; set; }

        public Mode.RegProListInfo Reginfo { get; set; }

        public int AreaInfoId { get; set; }

        public Mode.SpiderError ErrorInfo { get; set; }

        public string SiteName { get; set; }
        static Random random = new Random();
        public string getListUrl()
        {
            //string ip = random.Next(1, 255)+ "." + random.Next(1, 255)+ "." + random.Next(1, 255);
            string regionId = "";
            switch (AreaInfoId)
            {
                case 1: 
                    regionId = "11010100";
                    break;
                case 2:
                    regionId = "21010100";
                    break;
                case 3:
                    regionId = "31010100";
                    break;
                default:
                       regionId = "21010100";
                    break;
            }
         
            //string cookid = Guid.NewGuid().ToString();

             string mode = "\"mobile\" : false , \"catId\" : \"{0}\" ,\"catalog\" : \"coo8Store\" , \"siteId\" : \"coo8Site\" , \"shopId\" : \"\" , \"regionId\" : \"{1}\" , \"pageName\" : \"list\" , \"et\" : \"\" , \"XSearch\" : false , \"startDate\" : 0 , \"endDate\" : 0 , \"pageSize\" : 48 , \"state\" : 4 , \"weight\" : 0 , \"more\" : 0 , \"sale\" : 0 , \"instock\" : 1 , \"filterReqFacets\" :  null  , \"rewriteTag\" : false , \"userId\" : \"{2}\" , \"priceTag\" : 0 , \"cacheTime\" : 2 , \"parseTime\" : 2";
        
            StringBuilder url = new StringBuilder();
            //string param =
            //    "{\"pageNumber\":pid,\"envReq\":{\"catId\":\"" + SiteCatInfo.ClassId + "\",\"regionId\":\"" + regionId +
            //    "\",\"ip\":\"" + ip +
            //    "\",\"et\":\"\",\"XSearch\":false,\"pageNumber\":pid,\"pageSize\":48,\"more\":0,\"sale\":0,\"instock\":1,\"rewriteTag\":false,\"priceTag\":0,\"promoFlag\":0,\"esIp\":\"\",\"cookieid\":\"" +
            //    cookid + "\",\"t\":\"&cache=8&parse=10\",\"question\":\"\"}}";
            url.Append("http://search.gome.com.cn/cloud/asynSearch?module=product&from=category&page=$page&paramJson=");
            url.Append(HttpUtility.UrlEncode("{"+string.Format(mode, SiteCatInfo.ClassId, regionId, random.Next(10000000, 99999999))+"}"));

            return url.ToString().Replace("pid","$page");
        }

        public string DownLoadPage(string pageUrl, int p)
        {
            //HtmlAnalysis quest = new HtmlAnalysis();
            //quest.RequestMethod = "POST";
            // string page = quest.HttpRequest(pageUrl);
            string page = HtmlAnalysis.Gethtmlcode(pageUrl);

            if (pageUrl.Contains("http://search.gome.com.cn/cloud/asynSearch"))
            {
                if (page.Contains(" \"totalCount\" : 0"))
                    return "";
                var htmlllist = ServiceStack.Text.JsonSerializer.DeserializeFromString<ListItemEF>(page);
                //var templist = htmlllist["products"];
                //  var items = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, string>>(templist);
                var proContent = ServiceStack.Text.XmlSerializer.SerializeToString(htmlllist);
                return proContent;
            }
            return page;

        }

        public bool HasProducts(string pageHtml)
        {
            if (pageHtml.Contains("<totalCount>0</totalCount>") || pageHtml.Contains("<products />"))
                return false;
            return true;
        }

        public int getPageNum(string page_html)
        {
           // return RegGroupsX<int>(page_html, "<totalPage>(?<x>.*?)</totalPage>|" + Reginfo.MaxpageReg);
            return RegGroupsX<int>(page_html, "<totalPage>(?<x>.*?)</totalPage>");
        }

        public System.Text.RegularExpressions.MatchCollection GetSigleProduct(string pageHtml)
        {
            var res = RegGroupCollection(pageHtml, "<proItem>(?<x>.*?)</proItem>|" + Reginfo.SingleReg);
            return res;
        }

        public string GetProConent(string pageHtml)
        {
            return pageHtml;
        }

        public string GetSpName(string singleHtml)
        {
            //var title = RegGroupsX<string>(singleHtml, Reginfo.TitleReg);
            var title = RegGroupsX<string>(singleHtml, "<name>(?<x>.*?)</name>");
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
           // return RegGroupsX<decimal>(singleHtml, Reginfo.PriceReg);
            return RegGroupsX<decimal>(singleHtml, "<price>(?<x>.*?)</price>");
            
        }

        public string GetSpUrl(string singleHtml)
        {
         //  var url = RegGroupsX<string>(singleHtml, Reginfo.UrlReg);
            var url = RegGroupsX<string>(singleHtml, "<sUrl>(?<x>.*?)</sUrl>");
            
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
            //var img =RegGroupsX<string>(singleHtml, Reginfo.PicReg);
            var img = RegGroupsX<string>(singleHtml, "<sImg>(?<x>.*?)</sImg>");
            if(string.IsNullOrEmpty(img))
            {
                img = RegGroupsX<string>(singleHtml.Replace("<sImg></sImg>", ""), "<sImg>(?<x>.*?)</sImg>");
                if(!string.IsNullOrEmpty(img))
                    return img + "_360.jpg";
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
            return img + "_360.jpg";
            
        }

        public int GetComments(string singleHtml)
        {
            //var count = RegGroupsX<int>(singleHtml, Reginfo.CommentCountReg);
            return RegGroupsX<int>(singleHtml, "<evaluateCount>(?<x>.*?)</evaluateCount>");
           
        }

        public int GetSellType(string singleHtml)
        {
            if (singleHtml.Contains("由<b>国美在线</b>配送并提供保障监管"))
                return 1;
            return 2;
        }

        public string GetBigPic(string detial)
        {
            return "";
        }

        public string GetBrand(string detial)
        {
            return RegGroupsX<string>(detial, "breadName:\"(?<x>.*?)\"");
        }

        public string GetSkuDes(string detial)
        {
            
          string skudetial = RegGroupsX<string>(detial,
               "<ul class=\"specbox\">(?<x>.*?)</ul>");
          if (string.IsNullOrEmpty(skudetial))
              return "";
          var list = RegGroupCollection(skudetial, "<li>(?<x>.*?)</li>");
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
              var txt = li.Groups["x"].Value;
              string tempkey = RegGroupsX<string>(txt, "<span class=\"specinfo\">(?<x>.*?)</span>");
              string tempval = RegGroupsX<string>(txt, "<span>(?<x>.*?)</span>");
              if (!string.IsNullOrEmpty(tempkey) && !string.IsNullOrEmpty(tempval))
              {
                  res.Append("{");
                  res.AppendFormat(fomat, tempkey, tempval);
                  res.Append("},");
              }
          }
          res.Remove(res.Length - 1, 1);
          res.Append("]");

          return res.ToString();
        }

        public string GetOtherpic(string detial)
        {
            

           string tempimg = RegGroupsX<string>(detial, "<div class=\"pic-small j-gRbox j-pichover clearfix\">(?<x>.*?)</ul>");
            if (string.IsNullOrEmpty(tempimg))
                return "";
            var list = RegGroupCollection(tempimg, "bpic=\"(?<x>.*?)\"");
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
            if (detial.Contains("由<b>国美在线</b>配送并提供保障监管"))
                return "国美在线";
            string shopname = RegGroupsX<string>(detial, "bbcShopName = \"(?<x>.*?)\"");
            if (string.IsNullOrEmpty(shopname))
                return "";

            return shopname;
        }
    }
}
