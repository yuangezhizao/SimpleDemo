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
    class ZmmmClassInfo : Zmmm, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private const string domain = "http://www.zuimeimami.com";
        public ZmmmClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(276);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string page = HtmlAnalysis.Gethtmlcode(domain);
             
            string content = RegGroupsX<string>(page, "<h2>所有商品分类</h2><div class=\"submenu\">(?<x>.*?)<div class=\"nav\">");

            var catList = RegGroupCollection(content, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            for (int i = 0; i < catList.Count; i++)
            {
                string tempurl = catList[i].Groups["x"].Value;
                if (string.IsNullOrEmpty(tempurl))
                    continue;
                tempurl = domain + tempurl;
                string tempName = catList[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, "/list-(?<x>\\d+)");
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
            string crumb = RegGroupsX<string>(pageinfo,"<div class=\"w1200 breadNav\">(?<x>.*?)<div class=\"w1200\">");
            var catlist = RegGroupCollection(crumb, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (catlist == null)
                return;
            int deep = catlist.Count;
            if (deep > 1)
            {
                siteClassInfo.ParentUrl =domain+ catlist[deep - 2].Groups["x"].Value;
                siteClassInfo.ParentName = catlist[deep - 2].Groups["y"].Value;
                siteClassInfo.ParentClass = RegGroupsX<string>(siteClassInfo.ParentUrl, "/list-(?<x>\\d+)");
            }
            siteClassInfo.HasChild = deep <= 1;
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "搜索到 <span class=\"red\">(?<x>\\d+)</span> 件相关商品");

            string catArea = RegGroupsX<string>(pageinfo, "<div class=\"sortlist mb10\">(?<x>.*?)<!--左侧产品分类列表 E--");

            if (!string.IsNullOrEmpty(catArea))
            {

                var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\"\\s*>(?<y>.*?)</a>");
                if (list != null)
                {
                    foreach (Match match in list)
                    {
                        string tempurl =domain+ match.Groups["x"].Value;
                        string tempid = RegGroupsX<string>(tempurl, "/list-(?<x>\\d+)");
                        if (!ValidCatId(tempid))
                        {
                            continue;
                        }
                        if (!HasBindClasslist.Exists(p => p.ClassId == tempid))
                        {
                       
                            string tempName = match.Groups["y"].Value;
                            if(string.IsNullOrEmpty(tempName))
                            { continue; }

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
