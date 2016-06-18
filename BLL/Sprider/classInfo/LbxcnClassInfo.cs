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
    public class LbxcnClassInfo : Lbxcn, ISiteClassBLL
    {
        public LbxcnClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(169);
        }
        //private string domain = "http://www.lbxcn.com";
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.lbxcn.com/CategoryAll.html");

            GetCatInfo(directoryHtml);
        }


        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"brandContent\">(?<x>.*?)<div id=\"div2\"");
            if (catArea == null)
                return;
            catArea = catArea.Replace("\t", "").Replace("\r", "").Replace("\n", "");

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" target=\"_blank\" title=\"(?<y>.*?)\">");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid = RegGroupsX<string>(tempUrl, "/Category/(?<x>\\d+)-");
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
                        Urlinfo = string.Format("http://www.lbxcn.com/{0}", tempUrl),
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
            string page = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo,"utf8",false);
            string cromb = RegGroupsX<string>(page, " <p class=\"crumbBox fmir\">(?<x>.*?)</p>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "href=\"(?<x>.*?)\" title=\"(?<y>.*?)\">");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains("首页") || item.ToString().Contains(siteClassInfo.ClassName))
                    continue;

                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }
                if (parentName == "")
                { parentUrl = ""; continue; }
                parentId = RegGroupsX<string>(parentUrl, "/Category/(?<x>\\d+)-");
                if (!ValidCatId(parentId))
                    continue;
                parentUrl = string.Format("http://www.lbxcn.com/{0}", parentUrl);
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

            string catArea = RegGroupsX<string>(page, "<div class=\"cateNan cl\">(?<x>.*?)<script");
            if (catArea == null)
                return;
            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\"( class=\"cateC\")?( title=\"\")?>(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                tempName = WordCenter.FilterHtml(tempName);

                string tempid = RegGroupsX<string>(tempUrl, "/Category/(?<x>\\d+)-");
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
                        Urlinfo = string.Format("http://www.lbxcn.com/{0}", tempUrl),
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


            //if (brotherCat != null && brotherCat.Contains(siteClassInfo.ClassId))
            //    siteClassInfo.HasChild = false;
            //else
            //    siteClassInfo.HasChild = true;

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "此分类中共有</span><span class=\"redOne1 fontTwo\">(?<x>\\d+)</span><span>个商品");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
