using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class SfbestClassInfo : Sfbest, ISiteClassBLL
    {
        public SfbestClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(61);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.sfbest.com/html/web/categorys.html");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<!-------mainStart--------->(?<x>.*?)<!----mainOver------>");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "^http://www.sfbest.com/(?<x>.*?)/$|http://www.sfbest.com/\\w*/(?<x>\\d+)-0-0-0-0-\\d+-0-0-0-0-0.html");
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
            string cromb = RegGroupsX<string>(page, "<div class='breadcrumb'>(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href='(?<x>.*?)'>(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.Groups["y"].Value==siteClassInfo.ClassName)
                    continue;
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "^http://www.sfbest.com/(?<x>.*?)/$|http://www.sfbest.com/\\w*/(?<x>\\d+)-0-0-0-0-\\d+-0-0-0-0-0.html");
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

            string brotherCat = RegGroupsX<string>(page, "<div class=\"catlist\" id=\"cateall\">(?<x>.*?)<div id=\"salesRank\"");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "href=\"(?<x>.*?)\"  class=\"(cur trackref)|trackref\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "^http://www.sfbest.com/(?<x>.*?)/$|http://www.sfbest.com/\\w*/(?<x>\\d+)-0-0-0-0-\\d+-0-0-0-0-0.html");
                        if (!ValidCatId(bId))
                            continue;
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


            if (brotherCat != null && brotherCat.Contains(siteClassInfo.ClassId))
                siteClassInfo.HasChild = false;
            else
                siteClassInfo.HasChild = true;


            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page,"<div class=\"total\">共<span>(?<x>\\d+)</span>个商品</div>");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
