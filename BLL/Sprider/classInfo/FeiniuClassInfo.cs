using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class FeiniuClassInfo : Feiniu, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public FeiniuClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(161);
        }


        public void SaveAllSiteClass()
        {
             HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
             string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.feiniu.com/sitemap");
             directoryHtml = RegGroupsX<string>(directoryHtml, "<!-- 全部分类左侧-->(?<x>.*?)</section>");

             var list = RegGroupCollection(directoryHtml, "<a(?<x>.*?)</a>");
            for (int i = 0; i < list.Count; i++)
            {
               
                string item = list[i].Groups["x"].Value;
                string href = RegGroupsX<string>(item, "href=\"(?<x>.*?)\"");
                string proName = RegGroupsX<string>(item, " target=\"_blank\">(?<x>.*?)$");
                if (href.Contains("market"))
                {
                    string parId = RegGroupsX<string>(href, "market/(?<x>.*?)$");
                    if (string.IsNullOrEmpty(parId))
                        continue;
                    if (HasBindClasslist.Exists(p => p.ClassId == parId))
                        continue;
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = href,
                        ClassId = parId,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = false,
                        IsBind = false,
                        IsHide = false,
                        ClassName = proName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);

                }
                else
                    AddNode(href);
                if (shopClasslist.Count > 49)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
                }
             
            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
        }

        public void UpdateSiteCat()
        {

            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                try
                {
                    if (HasBindClasslist[i].ClassId != "")
                        UpdateCat(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }



        }

        private void UpdateCat(SiteClassInfo siteClassInfo)
        {
            string pageinfo = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);

            string crumble = RegGroupsX<string>(pageinfo,
                "<div class=\"item\">(?<x>.*?)<div class=\"search\">");
            if (crumble == null)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            var crumblelist = RegGroupCollection(crumble,
                "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo,
                "<span>共(?<x>\\d+)个商品</span>");

            if (crumblelist == null || crumblelist.Count == 0)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误2\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }

            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";
   

            for (int i = 0; i < crumblelist.Count; i++)
            {
              
                if (i > 0)
                {
   
                    pcatUrl = crumblelist[i-1].Groups["x"].Value;
                    pcatName = crumblelist[i-1].Groups["y"].Value;
                    if (pcatName != "首页" && pcatName != "飞牛网")
                    {
                        pcatId = RegGroupsX<string>(pcatUrl, "http://searchex.yixun.com/(?<x>.*?)-1-/|http://www.feiniu.com/market/(?<x>\\w+)$");
                        if (!ValidCatId(pcatId))
                            pcatId = "";
                        if (!string.IsNullOrEmpty(pcatId))
                        {
                            classCrumble += pcatId + ",";
                        }

                    }
                }
               
            }
            string currentCat = RegGroupsX<string>(crumble, "<h1 style=\"display:inline;\"><a href=\"javascript:;\">(?<x>.*?)</a>");
            if(!string.IsNullOrEmpty(currentCat))
            {
                siteClassInfo.ClassName = currentCat;
            }

            if ( siteClassInfo.ClassId.Contains('t'))
            {
             
                siteClassInfo.ParentClass = siteClassInfo.ClassId.Substring(0, siteClassInfo.ClassId.IndexOf('t'));
            }


            siteClassInfo.ParentUrl = pcatUrl;
            if (pcatName != "")
            siteClassInfo.ParentName = pcatName;
            if (pcatId!="")
            siteClassInfo.ParentClass = pcatId;
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

            var catlist = RegGroupsX<string>(pageinfo, "<div id=\"cata_list\">(?<x>.*?)<div class=\"cata_shop_right\" id=\"tracker_category\">");

            var catidList = RegGroupCollection(catlist, "href=\"(?<x>.*?)\"");
            foreach (Match item in catidList)
            {

                string tempCatUrl = item.Groups["x"].Value;
                string tempCatId = RegGroupsX<string>(tempCatUrl, "http://www.feiniu.com/(category|market)/(?<x>\\w+)$");

                if (tempCatId.Contains("t"))
                {
                    var tempids = tempCatId.Split('t');
                    string catid = tempids[tempids.Length - 1];
                    if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                        AddNode(tempCatUrl);
                }
            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

        }


        private void  AddNode(string url)
        {
            string classid = RegGroupsX<string>(url, "category/(?<x>.*?)$");

            if (string.IsNullOrEmpty(classid))
                return;

            if (HasBindClasslist.Exists(p => p.ClassId == classid))
                return;

            string pageinfo = HtmlAnalysis.Gethtmlcode(url);


            string cromp = RegGroupsX<string>(pageinfo, "<div id=\"path\"(?<x>.*?)</div>");
            int total = RegGroupsX<int>(pageinfo, "\"total\":(?<x>\\d+)");
            if (string.IsNullOrEmpty(cromp)) return;



            var caplist = RegGroupCollection(cromp, "<li>(?<x>.*?)</li>");
            if (caplist == null)
                return;
            int crompCount = caplist.Count;


            string parItem = caplist[crompCount-2].Groups["x"].Value;
            string parentUrl = RegGroupsX<string>(parItem, "href=\"(?<x>.*?)\"");
            string parentid = RegGroupsX<string>(parentUrl, "(market|category)/(?<x>.*?)$");
            string parentName = RegGroupsX<string>(parItem, "\">(?<x>.*?)</a>");
            string proName = RegGroupsX<string>(caplist[crompCount - 1].Groups["x"].Value, "\">(?<x>.*?)</h1>"); 
          
         
          
            SiteClassInfo cat = new SiteClassInfo
            {
                ParentUrl = parentUrl,
                ParentClass = parentid,
                ParentName = parentName,
                TotalProduct = total,
                Urlinfo = url,
                ClassId = classid,
                UpdateTime = DateTime.Now,
                IsDel = false,
                BindClassId = 0,
                BindClassName = "",
                HasChild = false,
                IsBind = false,
                IsHide = false,
                ClassName = proName,
                SiteId = Baseinfo.SiteId,
                ClassCrumble = parentid,

                CreateDate = DateTime.Now
            };
            HasBindClasslist.Add(cat);
            shopClasslist.Add(cat);

         

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
