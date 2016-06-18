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
   public class BaoshuiguojiClassInfo : Baoshuiguoji, ISiteClassBLL
    {
          protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private string domain = "http://www.baoshuiguoji.com";
        public BaoshuiguojiClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(268) ?? new SiteInfo { SiteId = 268 };
        }

        public void SaveAllSiteClass()
        {
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string homePage = HtmlAnalysis.Gethtmlcode("http://www.baoshuiguoji.com/gallery.html");
            string catArea = RegGroupsX<string>(homePage, "<li class=\"item\" id=\"webf1\"(?<x>.*?)</ol>");
            var catidList = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\"\\s+>(?<y>.*?)</a>");

            for (int i = 0; i < catidList.Count; i++)
            {
                var tempcatUrl = catidList[i].Groups["x"].Value;
                if (!tempcatUrl.Contains("gallery"))
                {
                    continue;}
                string catid = RegGroupsX<string>(tempcatUrl, "gallery-(?<x>\\d+).html");
                if (!ValidCatId(catid) || HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    continue;
                }
                tempcatUrl = string.Format("http://www.baoshuiguoji.com/gallery-{0}.html", catid);

                string catName = catidList[i].Groups["y"].Value;
                catName = WordCenter.FilterHtml(catName);
                if (string.IsNullOrEmpty(catName))
                    continue;

                SiteClassInfo cat = new SiteClassInfo
                {
                    ParentUrl = "",
                    ParentClass = "",
                    ParentName = "",
                    TotalProduct = 0,
                    Urlinfo = tempcatUrl,
                    ClassId = catid,
                    UpdateTime = DateTime.Now,
                    IsDel = false,
                    BindClassId = 0,
                    BindClassName = "",
                    HasChild = false,
                    IsBind = false,
                    IsHide = false,
                    ClassName = catName,
                    SiteId = Baseinfo.SiteId,
                    ClassCrumble = "",

                    CreateDate = DateTime.Now
                };
                HasBindClasslist.Add(cat);
                shopClasslist.Add(cat);


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
            string catPage = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);

            string crumbs = RegGroupsX<string>(catPage, "<div class=\"maxPath\">(?<x>.*?)</div>");


            var plist =RegGroupCollection(crumbs, "<a class=\"beb-nav bed\\d+\" href=\"/(?<x>.*?)\" > <span >(?<y>.*?)</span> </a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.Groups["x"].Value.Contains(siteClassInfo.ClassId))
                    continue;

                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
  
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }
                if (parentName == "")
                { parentUrl = ""; continue; }
                parentId = RegGroupsX<string>(parentUrl, "gallery-(?<x>\\d+).html");
 
                parentUrl = string.Format(domain + "/gallery-{0}.html", parentId);
                if (!HasBindClasslist.Exists(c => c.ClassId == parentId))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = parentName,
                        ClassId = parentId,
                        ParentUrl = "",
                        IsDel = false,
                        IsBind = false,
                        IsHide = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
                        ClassCrumble = "",
                        TotalProduct = 0,
                        SiteId = Baseinfo.SiteId,
                        Urlinfo = parentUrl,
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }

            var catarea =RegGroupsX<string>(catPage, "<dt class=\"filter-entries-label\">分类：</dt>(?<x>.*?)</dl>");
            var templist = RegGroupCollection(catarea, "<a href=\"(?<x>.*?)\" class=\"handle action-cat-filter\">(?<y>.*?)</a>");
            if (templist!=null)
               foreach (Match item in templist)
               {
                   string tempurl = item.Groups["x"].Value;


                   string catid = RegGroupsX<string>(tempurl, "gallery-(?<x>\\d+).html");
                   string catName = item.Groups["y"].Value;

                   tempurl = string.Format(domain + "/gallery-{0}.html", catid);
                   if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                   {
                       SiteClassInfo iteminfo = new SiteClassInfo
                       {
                           ParentClass = "",
                           ParentName = "",
                           ClassName = catName,
                           ClassId = catid,
                           ParentUrl = "",

                           IsDel = false,
                           IsBind = false,
                           IsHide = false,
                           BindClassId = 0,
                           BindClassName = "",
                           HasChild = true,
                           ClassCrumble = "",
                           TotalProduct = 0,
                           SiteId = Baseinfo.SiteId,
                           Urlinfo = tempurl,
                           UpdateTime = DateTime.Now,
                           CreateDate = DateTime.Now
                       };
                       HasBindClasslist.Add(iteminfo);
                       shopClasslist.Add(iteminfo);
                   }

               }



            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
           
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
