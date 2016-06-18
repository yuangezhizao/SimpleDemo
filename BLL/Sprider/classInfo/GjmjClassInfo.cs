using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class GjmjClassInfo : Gjmj, ISiteClassBLL
    {
        public GjmjClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(177);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.gj5s.com/product/search/qgyys.html");

            GetCatInfo(directoryHtml);
        }


        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"f_category\">(?<x>.*?)<!--商品分类结束-->");
            if (catArea == null)
                return;
            catArea = catArea.Replace("\t", "").Replace("\r", "").Replace("\n", "");

            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid = RegGroupsX<string>(tempUrl, "/\\w*/\\w*/(?<x>.*?).html");
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
                        Urlinfo = string.Format("http://www.gj5s.com{0}", tempUrl),
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

            if (page.Contains("抱歉，没有找到符合条件的商品！"))
            {
                new SiteClassBll().delClass(siteClassInfo);
                return;

            }

            string cromb = RegGroupsX<string>(page, "<div class=\"page_part\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }
                parentId = RegGroupsX<string>(parentUrl, "/\\w*/\\w*/(?<x>.*?).html");
                if (!HasBindClasslist.Exists(c => c.ClassId == parentId)&&ValidCatId(parentId))
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
                        Urlinfo = string.Format("http://www.gj5s.com{0}", parentUrl),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }

            string catArea = RegGroupsX<string>(page,
                "<div class=\"f_category\">(?<x>.*?)<!--商品分类结束-->");
            if (catArea == null)
                return;
            catArea = catArea.Replace("\t", "").Replace("\r", "").Replace("\n", "");

            var list = RegGroupCollection(catArea, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid = RegGroupsX<string>(tempUrl, "/\\w*/\\w*/(?<x>.*?).html");
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid)&& ValidCatId(tempid))
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
                        Urlinfo = string.Format("http://www.gj5s.com{0}", tempUrl),
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


            siteClassInfo.ParentClass = parentId ?? "";
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = string.Format("http://www.gj5s.com{0}", parentUrl);
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "共&nbsp;<span>(?<x>\\d+)</span>&nbsp;个商品");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
