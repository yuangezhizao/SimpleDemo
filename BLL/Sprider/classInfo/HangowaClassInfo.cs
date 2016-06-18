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
    public class HangowaClassInfo : Hangowa, ISiteClassBLL
    {
        public HangowaClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(166);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.hangowa.com/");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml, "<div class=\"category-box\" id=\"category_box\">(?<x>.*?)</ul>");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a(?<x>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = RegGroupsX<string>(item.ToString(), "href=\"(?<x>.*?)\"");
                if (string.IsNullOrEmpty(tempUrl))
                    continue;
                tempUrl = string.Format("http://www.hangowa.com{0}", tempUrl);
                string tempName = RegGroupsX<string>(item.ToString(), ">(?<x>.*?)</a>");
                string tempid = RegGroupsX<string>(tempUrl, "gallery-(?<x>\\d+?).html");
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {

                    int page = RegGroupsX<int>(directoryHtml, "共<b class=\"op-search-result\">(?<x>\\d+?)</b>件");
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
                        HasChild = !item.ToString().Contains("class=\"level3\""),
                        ClassCrumble = "",
                        TotalProduct = page,
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
            string cromb = RegGroupsX<string>(page, "<span class=\"pos-front\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\" alt=\"\" title=\"\">(?<y>.*?)</a></span>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains("首页"))
                    continue;
                parentUrl = item.Groups["x"].Value;
                if (string.IsNullOrEmpty(parentUrl))
                    continue;
                parentUrl = string.Format("http://www.hangowa.com{0}", parentUrl);
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "gallery-(?<x>\\d+?).html");
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

            string chlidCat = RegGroupsX<string>(page, "<dt class=\"filter-entries-label\">分类：</dt>\n            <dd class=\"filter-entries-values\">(?<x>.*?)</dd>");
            if (chlidCat != null)
            {
                var blist = RegGroupCollection(chlidCat, "<a href=\"(?<x>.*?)\" class=\"handle action-cat-filter\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        if (string.IsNullOrEmpty(burl))
                            continue;
                        burl = "http://www.hangowa.com" + burl.TrimEnd('?');
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "gallery-(?<x>\\d+?).html");
                        if (!HasBindClasslist.Exists(c => c.ClassId == bId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass =siteClassInfo.ClassId,
                                ParentName =siteClassInfo.ClassName,
                                ClassName = bName,
                                ClassId = bId,
                                ParentUrl = siteClassInfo.Urlinfo,
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


            //if (chlidCat != null && chlidCat.Contains(siteClassInfo.ClassId))
            //    siteClassInfo.HasChild = false;
            //else
            //    siteClassInfo.HasChild = true;


            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl =parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "共<b class=\"op-search-result\">(?<x>\\d+)</b>件商品");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
