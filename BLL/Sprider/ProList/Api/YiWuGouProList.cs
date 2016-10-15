using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;


namespace BLL.Sprider.ProList.Api
{
    public class YiWuGouProList : YiWuGou, IApiProList
    {
        public YiWuGouProList()
        {
            Baseinfo = new SiteInfoDB().SiteById(2)?? new SiteInfo{SiteId=2};
        }

        private const string UrlMode = "http://www.yiwugou.com/shop_list/i_{0}_{1}.html";

        public void GetAllProducts()
        {
          var  catlist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
          for (var i = 46; i < catlist.Count; i++)
          {
              if (!catlist[i].HasChild)
              {
                  int maxpage = 1;
                  for (int j = 0; j < maxpage; j++)
                  {
                      string url = string.Format(UrlMode, j + 1, catlist[i].ClassId);
                      string page = HtmlAnalysis.Gethtmlcode(url);
                      GetShopList(page, catlist[i].ClassId, catlist[i].ClassName, catlist[i].ParentName);
                      if (j == 0)
                          maxpage = RegGroupsX<int>(page, "<ul class=\"page\"><li><span>1/(?<x>.*?)</span>");
                  }

              }
          }
        }

        public void AddNewProducts()
        {
            var list = new ShopInfoBll().getAllSite();
        }

        private void GetShopList(string page,string catid,string catName,string parentName)
        {
         
            string shopArea = RegGroupsX<string>(page, "<!-- 广告商品 list end-->(?<x>.*?)<!--page start -->");
            if (string.IsNullOrEmpty(shopArea))
                return;
            var list = RegGroupCollection(shopArea, "<div class=\"pro_list_company_img\">(?<x>.*?)</div>");
            if (list == null)
                return;
            List<ShopInfo> shoplist = new List<ShopInfo>();
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    string shop = list[i].Groups["x"].Value;
                    string shopUrl = RegGroupsX<string>(shop, "<span><a href=\"(?<x>.*?)\"");
                    if (string.IsNullOrEmpty(shopUrl))
                        continue;
                    if (!shopUrl.Contains("http://www.yiwugou.com"))
                    {
                        shopUrl = "http://www.yiwugou.com" + shopUrl;
                    }
                    string shophtml = HtmlAnalysis.Gethtmlcode(shopUrl);

                    string kefu = RegGroupsX<string>(shophtml, "<ul class=\"shop-qq-bord\">(?<x>.*?)</ul>");
                    var qqlist = RegGroupCollection(kefu, "uin=(?<x>\\d+)");
                    StringBuilder qqinfo = new StringBuilder();
                    if (qqlist != null)
                    {
                     
                        for (int j = 0; j < qqlist.Count; j++)
                        {
                            string tempqq = qqlist[j].Groups["x"].Value;
                            if (string.IsNullOrEmpty(tempqq))
                                continue;
                            qqinfo.Append(tempqq);
                            qqinfo.Append(",");
                        }
                    }
                    ShopInfo info = new ShopInfo();
                    info.ShopLevel = RegGroupsX<string>(shop, "title=\"商铺信用等级\">(?<x>.*?)星");
                    info.ShopUrl = shopUrl;
                    info.ShopUserID = RegGroupsX<string>(shophtml, "id=\"shop_userid\" value=\"(?<x>.*?)\">");
                    info.UserName = RegGroupsX<string>(shophtml,
                        "id=\"operator_box\"><font style=\"float:left\">(?<x>.*?)</font>|<li><i class=\"ico-shop-01\"></i>(?<x>.*?)</li>");
                    info.UserPhone = RegGroupsX<string>(shophtml, "<i class=\"ico-shop-03\"></i>(?<x>.*?)</li>");

                    if (!string.IsNullOrEmpty(info.UserPhone))
                    {
                        if (info.UserPhone.Length == 8)
                        {
                            info.UserPhone = "0579-" + info.UserPhone;
                        }
                        else if (info.UserPhone.Length == 12)
                        {
                            info.UserPhone =new StringBuilder(info.UserPhone).Insert(4,"-").ToString();
                        }
                    }

                    info.UserMobile = RegGroupsX<string>(shophtml, "<i class=\"ico-shop-02\"></i>(?<x>.*?)</li>");
                    info.UserEmail = RegGroupsX<string>(shophtml, "<i class=\"ico-shop-04\"></i>(?<x>.*?)</li>");
                    info.ShopName = RegGroupsX<string>(shophtml, "tonick='(?<x>.*?)'");
                    info.Userqq = qqinfo.ToString().TrimEnd(',');
                    info.ShopNo = RegGroupsX<string>(shophtml,
                        "商位号</span>时间</li>\r\n                    <li><span>(?<x>.*?)</span>");
                    info.ShopAddress = RegGroupsX<string>(shophtml,
                        "<span class=\"font999 tit\">商铺地址：</span>\\s*<span class=\"font666 con\">(?<x>.*?)</span>");
                    info.ShopAddress = WordCenter.FilterHtml(info.ShopAddress);
                    info.ShopSummary = WordCenter.FilterHtml(RegGroupsX<string>(shophtml, "<font class=\"font666\">商铺介绍：</font>\r\n\t\t\t<font class=\"font999\">(?<x>.*?)</font>"));
          
                    info.CatId = catid;
                    info.CatName = catName;
                    info.ParentName = parentName;
                    info.CreateDate = DateTime.Now;
                    info.UpdateTime = DateTime.Now;
                    shoplist.Add(info);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }

            }
            new ShopInfoBll().AddSiteInfo(shoplist);
        }


    }
}
