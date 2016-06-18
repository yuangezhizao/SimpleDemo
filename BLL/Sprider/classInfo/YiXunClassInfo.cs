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
    public class YiXunClassInfo : YiXun, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public YiXunClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(11);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://st.icson.com/static_v1/js/app/categories_1.js");

            var list = RegGroupCollection(directoryHtml, "\"url\":\"(?<x>.*?)\"");
            for (int i = 0; i < list.Count; i++)
            {

                string url = list[i].Groups["x"].Value;
                if (url.Contains("http://searchex.yixun.com"))
                {
                    string id = RegGroupsX<string>(url,
                        "path=(?<x>[A-Za-z0-9]+)|^http://searchex.yixun.com/(?<x>.*?)\\-");
                    if (id == null)
                        continue;

                    if (id.Contains("t"))
                    {
                        var tempids = id.Split('t');
                        string catid = tempids[tempids.Length - 1];
                        if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                        {
                            string temurl = string.Format("http://searchex.yixun.com/{0}-1-/", id);
                            AddNode(temurl);
                        }
                    }
                    else
                    {
                        if (!HasBindClasslist.Exists(p => p.ClassId == id))
                        {
                            // add
                            string temurl = string.Format("http://searchex.yixun.com/{0}-1-/", id);
                            AddNode(temurl);
                        }
                    }


                    if (shopClasslist.Count > 0)
                    {
                        new SiteClassInfoDB().AddSiteClass(shopClasslist);
                        shopClasslist.Clear();
                    }
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

            string crumble = RegGroupsX<string>(pageinfo,
                "<div class=\"crumb_wrap\">(?<x>.*?)<div class=\"crumb_search \">");
            if (crumble == null)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            var crumblelist = RegGroupCollection(crumble,
                "<a class=\"crumb_lk\" href=\"(?<x>.*?)\" rg=\"\\d+_?\\d+\" ytag=\"\\d+\">(?<y>.*?)</a>");
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo,
                "<div class=\"sort_page_txt\">共<b>(?<x>\\d+)</b>件商品</div>");
            if (siteClassInfo.TotalProduct == 0)
            {
                siteClassInfo.IsDel = true;
                new SiteClassBll().delClass(siteClassInfo);
            }
            if (crumblelist == null || crumblelist.Count == 0)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误2\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }

            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";

            foreach (Match item in crumblelist)
            {
                if (item.ToString().Contains("首页"))
                    continue;
                 if (item.ToString().Contains(siteClassInfo.ClassName))
                {
                    string itemurl = item.Groups["x"].Value;

                    string tempid = RegGroupsX<string>(itemurl, "http://searchex.yixun.com/(?<x>.*?)-1-/");
                    if (!ValidCatId(tempid))
                        continue;
                    if (tempid.Contains('t'))
                    {
                        var tempids = tempid.Split('t');
                        string catid = tempids[tempids.Length - 1];
                        siteClassInfo.ClassId = catid;
                        if (tempids.Length > 1)
                            siteClassInfo.ParentClass = tempids[tempids.Length - 2];
                    }
                    else
                    {
                        siteClassInfo.ClassId = tempid;
                    }

                    siteClassInfo.ClassName = item.Groups["y"].Value;
                    siteClassInfo.Urlinfo = itemurl;

                }
                else
                {
                    pcatUrl = item.Groups["x"].Value;
                    pcatName = item.Groups["y"].Value;
                    pcatId = RegGroupsX<string>(pcatUrl, "http://searchex.yixun.com/(?<x>.*?)-1-/");
                    if (!string.IsNullOrEmpty(pcatId))
                        classCrumble += pcatId + ",";
                }
            }
   
            if ( siteClassInfo.ClassId.Contains('t'))
            {
                siteClassInfo.ParentClass = siteClassInfo.ClassId.Substring(0, siteClassInfo.ClassId.IndexOf('t'));
            }

            string catArea = RegGroupsX<string>(pageinfo, "<div class=\"cate cate_2\" id=\"cateList\">(?<x>.*?)<div id=\"zdmArticle\" class=\"article_relative hide\">");
            var tempcatlist = RegGroupCollection(catArea, "<a class=\"cate_lk2 \" href=\"(?<x>.*?)\" title=(?<y>.*?) navvalue");
            foreach (Match item in tempcatlist)
            {
                string tempurl = item.Groups["x"].Value;
                string tempId = RegGroupsX<string>(tempurl, "http://searchex.yixun.com/(?<x>.*?)-1-/");
                if (tempId.Contains("t"))
                {
                    tempId = tempId.Substring(tempId.IndexOf('t') + 1);
                }
                string tempName = item.Groups["y"].Value;
                if (!HasBindClasslist.Exists(c => c.ClassId == tempId))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = tempName,
                        ClassId = tempId,
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
            //siteClassInfo.ParentUrl = pcatUrl;
            if (pcatName != "")
                siteClassInfo.ParentName = pcatName;

            //if (pcatId!="")
            //siteClassInfo.ParentClass = pcatId;
            siteClassInfo.ClassCrumble = classCrumble;
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }


        private void AddNode(string url)
        {
            string classid = RegGroupsX<string>(url, "^http://searchex.yixun.com/(?<x>.*?)\\-");

            if (string.IsNullOrEmpty(classid))
                return;

            if (classid.Contains("t"))
            {
                var tempids = classid.Split('t');
                string catid = tempids[tempids.Length - 1];
                if (HasBindClasslist.Exists(p => p.ClassId == catid))
                    return;
            }
            else
            {

                if (HasBindClasslist.Exists(p => p.ClassId == classid))
                    return;
            }
            string pageinfo = HtmlAnalysis.Gethtmlcode(url);
            if (!pageinfo.Contains("<div class=\"goods\""))
                return;

            string cromp = RegGroupsX<string>(pageinfo, "<div id=\"crumbBox\" class=\"crumb \">(?<x>.*?)<div class=\"crumb_search \">");
            if (string.IsNullOrEmpty(cromp)) return;



            var caplist = RegGroupCollection(cromp, "<a class=\"crumb_lk\" href=\"(?<x>.*?)\" rg=\"[0-9_]+\" ytag=\"\\d+\">(?<y>.*?)</a>");
            if (caplist == null || caplist.Count < 1)
                return;
            string proName = caplist[caplist.Count - 1].Groups["y"].Value;

            string parentUrl = "";
            string parentName = "";
            string parentid = "";
            string classCrumble = "";
            List<string> lessCat = new List<string>();
            for (int i = 0; i < caplist.Count - 1; i++)
            {
                if (i == 0)
                    continue;

                parentUrl = caplist[i].Groups["x"].Value;
                parentName = caplist[i].Groups["y"].Value;
                parentid = RegGroupsX<string>(parentUrl, "path=(?<x>[A-Za-z0-9]+)|^http://searchex.yixun.com/(?<x>.*?)\\-");
              
                if (!string.IsNullOrEmpty(parentid))
                {
                    if (parentid.Contains(','))
                        parentid = parentid.Substring(parentid.LastIndexOf(',') + 1);
                    classCrumble += parentid + ",";
                    if (!HasBindClasslist.Exists(p => p.ClassId == parentid))
                    {
                        lessCat.Add(string.Format("http://searchex.yixun.com/{0}-1-/", parentid));
                    }
                }
                
            }
           

            if (classid.Contains("t"))
            {
                var tempids = classid.Split('t');
                parentid = tempids[0];
                classid = tempids[1];
                if (classCrumble == "")
                    classCrumble = parentid;
            }
            classCrumble = classCrumble.TrimEnd(',');

            if (HasBindClasslist.Exists(p => p.ClassId == classid))
                return;
            int total = RegGroupsX<int>(pageinfo, "共<b>(?<x>\\d+)</b>件商品");
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
                ClassCrumble = classCrumble,

                CreateDate = DateTime.Now
            };
            HasBindClasslist.Add(cat);
            shopClasslist.Add(cat);

            string catList = RegGroupsX<string>(pageinfo, "<div class=\"cate_bd\">(?<x>.*?)<div id=\"viewedGoods\"");
            var temCats = RegGroupCollection(catList,"href=\"(?<x>.*?)\"");
            if (temCats == null)
                return;
            for (int i = 0; i < temCats.Count; i++)
            {
                string tempCatUrl = temCats[i].Groups["x"].Value;
                string tempcatid = RegGroupsX<string>(tempCatUrl, "path=(?<x>[A-Za-z0-9]+)|^http://searchex.yixun.com/(?<x>.*?)\\-");
                if (string.IsNullOrEmpty(tempcatid))
                    continue;
                if (!HasBindClasslist.Exists(p => p.ClassId == parentid))
                {
                    lessCat.Add(string.Format("http://searchex.yixun.com/{0}-1-/", tempcatid));
                }

            }
            for (int i = 0; i < lessCat.Count; i++)
            {
                AddNode(lessCat[i]);
            }

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
