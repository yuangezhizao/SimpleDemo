using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class YaofangwangClassinfo : Yaofangwang, ISiteClassBLL
    {
         protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public YaofangwangClassinfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(257) ?? new SiteInfo { SiteId = 257 };
        }

        public void SaveAllSiteClass()
        {
            string url = "http://yaofang.cn/";
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string popHtml = HtmlAnalysis.Gethtmlcode(url);
            string content = RegGroupsX<string>(popHtml, "<div class=\"s_subcontent_140106\">(?<x>.*?)<div class=\"s_navbreadcrumb\">");
            var catlist = RegGroupCollection(content, "<a href=\"(?<y>.*?)\"( class=\"yao\")?>(?<x>.*?)</a>");


            for (int i = 0; i < catlist.Count; i++)
            {
                string tempurl = catlist[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, @"http://www.yaofang.cn/category-(?<x>\d+)-");
                
                if (!ValidCatId(catid) || HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    continue;
                }
                tempurl = string.Format("http://www.yaofang.cn/a/category/?cat_id={0}", catid);
                string catName = catlist[i].Groups["x"].Value;
         
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
                    if (!HasBindClasslist[i].HasChild)
                        continue;
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
            string crumb = RegGroupsX<string>(pageinfo, "<div class=\"w_listtop_\\d+\">(?<x>.*?)</div>");
            if (crumb == null)
            {
                siteClassInfo.IsDel = true;
                new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            var list = RegGroupCollection(crumb, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (list == null)
                return;
            string parentid = "";
            string parentName = "";
            string parentUrl = "";

            for (int i = 0; i < list.Count; i++)
            {
                string tempName = list[i].Groups["y"].Value;
       
                string tempUrl = list[i].Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempUrl, @"cat_id=(?<x>\d+)");
                if (string.IsNullOrEmpty(tempid))
                {
                    continue;
                }

                if (tempid == siteClassInfo.ClassId)
                    break;
                if (!ValidCatId(tempid))
                {
                    continue;}
                parentid = tempid;
                parentName = tempName;
                parentUrl = tempUrl;

                var obj = HasBindClasslist.SingleOrDefault(c => c.ClassId == tempid);
                if (obj==null)
                {
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = parentUrl,
                        ClassId = parentid,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
                        IsBind = false,
                        IsHide = false,
                        ClassName = parentName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);
                }

            }
            if (ValidCatId(parentid))
            {
                siteClassInfo.ParentName = parentName;
                siteClassInfo.ParentUrl = parentUrl;
                siteClassInfo.ParentClass = parentid;
            }
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "<div class=\"total\"> <span>\\(共<strong >(?<x>\\d+)</strong>");
            siteClassInfo.UpdateTime = DateTime.Now;

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);



            string catArea = RegGroupsX<string>(pageinfo, "<ul id=\"w_menu_\\d*\">(?<x>.*?)<script");

            var catList = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" title=\"(?<y>.*?)\">");

            for (int i = 0; i < catList.Count; i++)
            {
                string tempurl = catList[i].Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempurl, @"cat_id=(?<x>\d+)");
                if (!ValidCatId(tempid) || HasBindClasslist.Exists(c => c.ClassId == tempid))
                    continue;
                string tempName = catList[i].Groups["y"].Value;
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
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }



            
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
