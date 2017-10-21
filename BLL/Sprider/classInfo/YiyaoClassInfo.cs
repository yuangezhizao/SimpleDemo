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
    class YiyaoClassInfo : Yiyao, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private const string domain = "http://www.111.com.cn";
        public YiyaoClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(248);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string classurl = "http://www.111.com.cn/categories/";
            string page = HtmlAnalysis.Gethtmlcode(classurl);
             
            string content = RegGroupsX<string>(page, "<div class=\"allsort sortwidth\">(?<x>.*?)<!--分类结束-->");

            var catList = RegGroupCollection(content, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            for (int i = 0; i < catList.Count; i++)
            {
                string tempurl = catList[i].Groups["x"].Value;
                if (string.IsNullOrEmpty(tempurl))
                    continue;

                string tempName = catList[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, "http://www.111.com.cn/list/(?<x>\\d+)");
                if (!ValidCatId(catid))
                    continue;
                if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                {
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = tempurl,
                        ClassId = catid,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
                        IsBind = false,
                        IsHide = false,
                        ClassName = tempName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);
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
            //if (pageinfo.Contains("很抱歉，没有找到相关的商品。"))
            //    return;
            string crumb = RegGroupsX<string>(pageinfo,"<div class=\"searchCrumb clearfix\">(?<x>.*?)<div id=\"search_result\">");
            var catlist = RegGroupCollection(crumb, "<a\\s+class=\"linkOne catalogs\".*?href=\"(?<x>.*?)\"><span>(?<y>.*?)</span>");
            if (catlist == null)
                return;
            int deep = catlist.Count;
            if (deep > 1)
            {
                siteClassInfo.ParentUrl = catlist[deep - 2].Groups["x"].Value;
                siteClassInfo.ParentName = catlist[deep - 2].Groups["y"].Value;
                siteClassInfo.ParentClass = RegGroupsX<string>(siteClassInfo.ParentUrl, "http://www.111.com.cn/list/(?<x>\\d+)");
            }
            siteClassInfo.HasChild = deep <=3;
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "<span id=\"num\">(?<x>\\d+)</span>");
            string catname= RegGroupsX<string>(pageinfo, "<title>(?<x>.*?)用药,|<title>(?<x>.*?)价格");
            if (siteClassInfo.ClassName != catname)
            {
                siteClassInfo.ClassName = catname;
            }

            string catArea = RegGroupsX<string>(pageinfo, "<!--三级列表开始-->(?<x>.*?)<!--三级列表结束-->");

            if (!string.IsNullOrEmpty(catArea))
            {
                siteClassInfo.HasChild = !catArea.Contains("active");
                var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" cid=\"\\d+\"\\s+rel=\"nofollow\"\\s+>(?<y>.*?)<span>");
                if (list != null)
                {
                    foreach (Match match in list)
                    {
                        string tempurl = match.Groups["x"].Value;
                        string tempid = RegGroupsX<string>(tempurl, "http://www.111.com.cn/list/(?<x>\\d+)");
                        if (!ValidCatId(tempid))
                        {
                            continue;
                        }
                        if (!HasBindClasslist.Exists(p => p.ClassId == tempid))
                        {
                       
                            string tempName = match.Groups["y"].Value;
                            if(string.IsNullOrEmpty(tempName))
                            { continue; }
                            tempName = tempName.Replace("\r\n", "").Trim(' ');
                            SiteClassInfo cat = new SiteClassInfo
                            {
                                
                                ParentUrl = "",
                                ParentClass = "",
                                ParentName = "",
                                TotalProduct = 0,
                                Urlinfo = tempurl,
                                ClassId = tempid,
                                UpdateTime = DateTime.Now,
                                IsDel = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = true,
                                IsBind = false,
                                IsHide = false,
                                ClassName = tempName,
                                SiteId = Baseinfo.SiteId,
                                ClassCrumble = "",

                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(cat);
                            shopClasslist.Add(cat);
                        }

                    }
                    if (shopClasslist.Count > 0)
                    {
                        new SiteClassInfoDB().AddSiteClass(shopClasslist);
                        shopClasslist.Clear();
                    }
                }
            }

            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
