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
    public class KadClassInfo : Kad, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public KadClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(256) ?? new SiteInfo { SiteId = 256 };
        }
        private string  domain = "http://www.360kad.com";

        public void SaveAllSiteClass()
        {
            string url = "http://www.360kad.com/dymhh/allclass.shtml";
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string popHtml = HtmlAnalysis.Gethtmlcode(url);
            string content = RegGroupsX<string>(popHtml, "<ul class=\"clearfix ksBoxs\" id=\"ksBoxs\"(?<x>.*?)</ul>");
            var catlist = RegGroupCollection(content, "<a href=\"(?<y>.*?)\" target=\"_blank\">(?<x>.*?)</a>");
            for (int i = 0; i < catlist.Count; i++)
            {
                string tempurl = catlist[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, "http://www.360kad.com/Category_(?<x>\\d+)/Index.aspx");
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
                    //if (HasBindClasslist[i].ParentClass != "") continue;
         
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
            string crumb = RegGroupsX<string>(pageinfo, "(<div class=\"Crumbs\">|<div class=\"crumb\">)(?<x>.*?)</div>|<div class=\"navigation\"(?<x>.*?)</div>");
            if(crumb==null)
            {
                siteClassInfo.IsDel = true;
                new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            var list = RegGroupCollection(crumb, "<a href=\"(?<x>.*?)\".*?>(?<y>.*?)</a>");
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
                string tempid = RegGroupsX<string>(tempUrl, "Category_(?<x>\\d+)/");
                if (string.IsNullOrEmpty(tempid))
                {
                    continue;
                }
              
                if (tempid == siteClassInfo.ClassId)
                    break;
                parentid = tempid;
                parentName = tempName;
                parentUrl = string.Format("http://www.360kad.com/Category_{0}/Index.aspx", tempid);

            }
            if (ValidCatId(parentid))
            {
                siteClassInfo.ParentName = parentName;
                siteClassInfo.ParentUrl = parentUrl;
                siteClassInfo.ParentClass = parentid;
            }
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "共(?<x>\\d+)个商品");
            siteClassInfo.UpdateTime = DateTime.Now;

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
                                                       
            var catList = RegGroupCollection(pageinfo, "href=\"/Category_(?<x>\\d+)/Index.aspx\".*?>(?<y>.*?)</a>|href=\"http://www.360kad.com/Category_(?<x>\\d+)/Index.aspx\">(?<y>.*?)</a>");
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
                    Urlinfo = string.Format("http://www.360kad.com/Category_{0}/Index.aspx",tempid),
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
