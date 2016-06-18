    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Commons;
    using DataBase;
    using Mode;
    using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class HaierClassInfo : Haier, ISiteClassBLL
    {
        public HaierClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(194) ?? new SiteInfo {SiteId = 194};
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.ehaier.com/subject/allsort.html");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"all-category-box\">(?<x>.*?)<div class=\"category-contact\">");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\".*?>(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (tempName == "商品分类")
                    continue;
                string tempid = RegGroupsX<string>(tempUrl, "http://www.ehaier.com/l/(?<x>\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+-\\d+).html");
                if (!ValidCatId(tempid))
                    continue;
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = tempName,
                        ClassId = tempid,
                        ParentUrl = "",
                        IsDel = false,
                        IsBind = false,
                        IsHide = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild =tempid.Contains("-"),
                        ClassCrumble = "",
                        TotalProduct = 0,
                        SiteId = Baseinfo.SiteId,
                        Urlinfo = tempUrl,
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




        }

        public void UpdateSiteCat()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                try
                {
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
            string page = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);
            string cromb = RegGroupsX<string>(page, "您现在的位置： <a href=\"http://www.ehaier.com\">海尔商城</a>(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "http://www.ehaier.com/l/(?<x>\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+-\\d+).html");
                if (!ValidCatId(parentId))
                    continue;
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

            string brotherCat = RegGroupsX<string>(page, "<div class=\"list-subnav\">(?<x>.*?)<dl class=\"dl-subnav dl-subnav-now\">");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "http://www.ehaier.com/l/(?<x>\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+).html|http://www.ehaier.com/l/(?<x>\\d+-\\d+-\\d+).html");
                        if (!HasBindClasslist.Exists(c => c.ClassId == bId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = "",
                                ParentName = "",
                                ClassName = bName,
                                ClassId = bId,
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
                                Urlinfo = burl,
                                UpdateTime = DateTime.Now,
                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(iteminfo);
                            shopClasslist.Add(iteminfo);
                        }

                    }
                }
 
            }
               if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
            if(HasBindClasslist.Exists(c=>c.ParentClass==siteClassInfo.ClassId))
                siteClassInfo.HasChild = true;
            else
                siteClassInfo.HasChild = false;

            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "共<strong class=\"haierred\">(?<x>\\d+)</strong> 件");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
