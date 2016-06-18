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
    public class OkHqbInfo : OkHqb, ISiteClassBLL
    {

        public OkHqbInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(123);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.okhqb.com/list/72.html");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"list_sidebar cfix\">(?<x>.*?)<div class=\"okhqb-footer-gb\">");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\"\\s*target=\"_top\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "http://www.okhqb.com/list/(?<x>.*?).html");
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
            //<div class="breadcrumbs_location">
            string page = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);
            string cromb = RegGroupsX<string>(page, "<div class=\"breadcrumbs_location\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\"\\s*target=\"_top\">(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains("首页") || item.Groups["x"].Value==siteClassInfo.Urlinfo)
                    continue;
                
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "http://www.okhqb.com/list/(?<x>.*?).html");
                

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
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
            string tempname = RegGroupsX<string>(cromb, "<span>(?<x>[\u4e00-\u9fa5]+)</span>");
            if (!string.IsNullOrEmpty(tempname))
                siteClassInfo.ClassName = tempname;
            siteClassInfo.HasChild = plist.Count == 1;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, " <em class=\"num\">(?<x>\\d+)</em>");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
