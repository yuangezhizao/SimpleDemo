using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class JianyiClass : Jianyi, ISiteClassBLL
    {
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        public JianyiClass()
        {
            Baseinfo = new SiteInfoDB().SiteById(129);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string url = "http://www.j1.com/sitemap.html";
            string page = HtmlAnalysis.Gethtmlcode(url);
            string content = RegGroupsX<string>(page, "<div class=\"sitemap_sortwrap qbfl\">(?<x>.*?)</div>");
            var list = RegGroupCollection(content, "<a target='_blank' href='(?<y>.*?)'>(?<x>.*?)</a>");
            for (int i = 0; i < list.Count; i++)
            {
                string catUrl = list[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(catUrl, "http://www.j1.com/p-(?<x>\\d+)");
                string catName = list[i].Groups["x"].Value;
                if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                {
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = catUrl,
                        ClassId = catid,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
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
            string crumb = RegGroupsX<string>(pageinfo, "<div class=detailnav>(?<x>.*?)</div>");
            if (crumb == null)
                return;
            var deep = RegGroupCollection(crumb, "href=\"(?<y>.*?)\"( target=\"_blank\")?>(?<x>.*?)</a>");
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            for (int i = 0; i < deep.Count; i++)
            {
                if (deep[i].Groups["x"].Value.Contains("首页") || deep[i].Groups["x"].Value == siteClassInfo.ClassName)
                    continue;
                parentUrl = deep[i].Groups["y"].Value;
                parentName = deep[i].Groups["x"].Value;
                parentId = RegGroupsX<string>(parentUrl, "http://www.j1.com/p-(?<x>\\d+)");
            }
            string children = RegGroupsX<string>(pageinfo, "<div class=\"listpageChooseBox\">(?<x>.*?)</div>");
            var catlist = RegGroupCollection(children, "<a href=\"(?<y>.*?)\">(?<x>.*?)<span>");
            
            siteClassInfo.HasChild = !children.Contains(siteClassInfo.ClassName);

            if (catlist != null)
            {
                for (int i = 0; i < catlist.Count; i++)
                {
                    string url = catlist[i].Groups["y"].Value;
                    string catid = RegGroupsX<string>(url, "http://www.j1.com/p-(?<x>\\d+)");
                    if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                    {
                        string catName = catlist[i].Groups["x"].Value;
                        SiteClassInfo cat = new SiteClassInfo
                        {
                            ParentUrl = "",
                            ParentClass = "",
                            ParentName = "",
                            TotalProduct = 0,
                            Urlinfo = url,
                            ClassId = catid,
                            UpdateTime = DateTime.Now,
                            IsDel = false,
                            BindClassId = 0,
                            BindClassName = "",
                            HasChild = true,
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
                }
            }

            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "共(?<x>\\d+)个商品");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
