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
    public class LeFengClassInfo : LeFeng, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public LeFengClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(43);
        }

        public void SaveAllSiteClass()
        {
            string url = "http://s.lefeng.com/";// "http://m.lefeng.com/index.php/widget/get_all_category?aid=-x-equ-html5-&output=json";
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode(url);

            var catArea = RegGroupsX<string>(directoryHtml, "<div class=\"akList\" id=\"aklist1\">(?<x>.*?)<div id=\"Cfooter\" class=\"Cfooter\">");
            if (catArea == null)
                return;
            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\"");
            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "http://s.lefeng.com/(directory|sweater|coat)/(?<x>.*?).html");
                string tempName = item.Groups["y"].Value;
                if (tempid == null)
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
                        HasChild = false,
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
            int count = RegGroupsX<int>(page, "相关商品<b class=\"pri\">(?<x>\\d+)</b>件");
            string cromb = RegGroupsX<string>(page, "<div class=\"path\">(?<x>.*?)</div>");
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\" title=\"(?<y>.*?)\">");
            if (plist == null)
            {
                siteClassInfo.UpdateTime = DateTime.Now;
                siteClassInfo.ClassName = RegGroupsX<string>(page, "<h1>(?<x>.*?)</h1>");
                siteClassInfo.TotalProduct = count;
                new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
                return;
            }
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "http://s.lefeng.com/(directory|sweater|coat)/(?<x>.*?).html");
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


            string brotherCat = RegGroupsX<string>(page, "<div class=\"sidebar\" id=\"sidebar\">(?<x>.*?)<div class=\"skimbuy\" id=\"seen_and_buy\"");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "<a rel=\"nofollow\" href=\"(?<x>.*?)\\s*\" id=\"\\d+\">(?<y>.*?)\\s*</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "Sort-\\d+-(?<x>.*?).htm");
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


            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.TotalProduct = count;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.ClassName = RegGroupsX<string>(page, "<h1>(?<x>.*?)</h1>");
            if (page.Contains("cat3name: ''"))
                siteClassInfo.HasChild = true;
            else
                siteClassInfo.HasChild = false;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
