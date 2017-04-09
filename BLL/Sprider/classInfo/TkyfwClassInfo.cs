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
    public class TkyfwClassInfo : Tkyfw, ISiteClassBLL
    {

        public TkyfwClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(297);
        }
        protected List<SiteClassInfo> ShopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }


        public void SaveAllSiteClass()
        {
            string url = "http://www.tkyfw.com/Ching_list_30.html";
            var page = HtmlAnalysis.httpsGet(url);
            var catContent = RegGroupsX<string>(page, "<!-- 左侧导航 -->(?<x>.*?)<!-- 左侧导航 结束-->");
            var list = RegGroupCollection(catContent, "<a href=\"(?<x>.*?)\" title=\"\">(?<y>.*?)</a>");
            if(list==null||list.Count==0)
                return;
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            foreach (Match match in list)
            {
                string caturl = string.Format("http://www.tkyfw.com{0}", match.Groups["x"].Value);
                string catid = RegGroupsX<string>(caturl, "(?<x>\\d+)");
                if(!ValidCatId(catid))
                    continue;
                string catname = match.Groups["y"].Value;
                if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = catname,
                        ClassId = catid,
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
                        Urlinfo = caturl,
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    ShopClasslist.Add(iteminfo);
                }

                if (ShopClasslist.Count > 20)
                {
                    new SiteClassInfoDB().AddSiteClass(ShopClasslist);
                    ShopClasslist.Clear();
                }
            }

            if (ShopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(ShopClasslist);
                ShopClasslist.Clear();
            }

        }

        public void UpdateSiteCat()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            foreach (SiteClassInfo t in HasBindClasslist)
            {
                try
                {
                    UpdateCat(t);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }

        private void UpdateCat(SiteClassInfo siteClassInfo)
        {
            string page = HtmlAnalysis.httpsGet(siteClassInfo.Urlinfo);
            string crumbs = RegGroupsX<string>(page, "<div class=\"crumbs\">(?<x>.*?)</div>");
            if(string.IsNullOrEmpty(crumbs))
                return;
            var list = RegGroupCollection(crumbs, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            string parentName = "";
            string parentUrl = "";
            string parentId = "";
            foreach (Match match in list)
            {
                var catName = match.Groups["y"].Value;
                if(catName=="首页")
                    continue;
                if(catName==siteClassInfo.ClassName)
                    break;
                parentName = catName;
                parentUrl= string.Format("http://www.tkyfw.com{0}", match.Groups["x"].Value);
                parentId = RegGroupsX<string>(parentUrl, "(?<x>\\d+)");
            }

            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "<span>共<strong>(?<x>\\d+)</strong>");

            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
