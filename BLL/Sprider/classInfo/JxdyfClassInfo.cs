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
    public class JxdyfClassInfo : JuMei, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public JxdyfClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(259) ?? new SiteInfo {SiteId = 259};
        }
        private string domain = "http://www.jxdyf.com";

        public void SaveAllSiteClass()
        {
            string url = "http://www.jxdyf.com/category";
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string popHtml = HtmlAnalysis.Gethtmlcode(url);
            string content = RegGroupsX<string>(popHtml, "<div class=\"fl\">(?<x>.*?)<div id=\"footer\"");
            var catlist = RegGroupCollection(content, "<a href='(?<y>.*?)'( class='.*?')?>(?<x>.*?)</a>");
            for (int i = 0; i < catlist.Count; i++)
            {
                string tempurl = catlist[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, "(?<x>\\d+)");
                if (!ValidCatId(catid) || HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    continue;
                }

                string catName = catlist[i].Groups["x"].Value;
                SiteClassInfo cat = new SiteClassInfo
                {
                    ParentUrl = "",
                    ParentClass = "",
                    ParentName = "",
                    TotalProduct = 0,
                    Urlinfo =domain+ tempurl,
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

            ;

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
            string crumb = RegGroupsX<string>(pageinfo, "<div class=\"curr_position\">(?<x>.*?)</div>");

          
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
                if (tempName.Contains("首页"))
                    continue;
                string tempUrl = list[i].Groups["x"].Value;
                if (!tempUrl.Contains(domain))
                    tempUrl = domain + tempUrl;
                string tempid = RegGroupsX<string>(tempUrl, "(?<x>\\d+)");
                if (string.IsNullOrEmpty(tempid))
                {
                    continue;
                }

                if (tempid == siteClassInfo.ClassId)
                    break;
                parentid = tempid;
                parentName = tempName;
                parentUrl = string.Format("http://www.jxdyf.com/category/{0}.html", tempid);

            }
            if (ValidCatId(parentid))
            {
                siteClassInfo.ParentName = parentName;
                siteClassInfo.ParentUrl = parentUrl;
                siteClassInfo.ParentClass = parentid;
            }
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "共有(?<x>\\d+)个商品");
            siteClassInfo.UpdateTime = DateTime.Now;

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
            string areaCat = RegGroupsX<string>(pageinfo, "<div class=\"menu outline_01\">(?<x>.*?)<div class=\"web_surfer outline_01\">");

            if (areaCat == null)
                return;

            var catList = RegGroupCollection(areaCat, "<a href=\"/category/(?<x>\\d+).html\" >(?<y>.*?)");
            for (int i = 0; i < catList.Count; i++)
            {
                string tempid = catList[i].Groups["x"].Value;
                if (!ValidCatId(tempid) || HasBindClasslist.Exists(c => c.ClassId == tempid))
                    continue;
                string tempName = catList[i].Groups["y"].Value;
                SiteClassInfo cat = new SiteClassInfo
                {
                    ParentUrl = "",
                    ParentClass = "",
                    ParentName = "",
                    TotalProduct = 0,
                    Urlinfo = string.Format("http://www.jxdyf.com/category/{0}.html", tempid),
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
