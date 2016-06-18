using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class VmeiClassInfo : Vmei, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public VmeiClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(181);
        }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://m.vmei.com/category");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {

            string catArea = RegGroupsX<string>(directoryHtml, "<section class=\"container\" id=\"category\" role=\"region\">(?<x>.*?)</section>");
            var catlist = RegGroupCollection(catArea, "<a href=\"(?<y>.*?)\">(?<x>.*?)</a>");
            foreach (Match item in catlist)
            {
                string tempUrl = item.Groups["y"].Value;
                string tempName = item.Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "cid=(?<x>\\d+)$");
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
                        Urlinfo = string.Format("http://www.vmei.com/products?cid={0}", tempid),
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
            string cromb = RegGroupsX<string>(page, "<div class=\"crumb\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<li><a href=\"(?<x>.*?)\">(?<y>.*?)</a></li>");
            if (plist == null)
                return;
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains(siteClassInfo.ClassId))
                    continue;
                string parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "cid=(?<x>\\d+)$");
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
                        Urlinfo = string.Format("http://www.vmei.com/products?cid={0}", parentId),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }

            string childrenCat = RegGroupsX<string>(page, "<th>分类 </th>(?<x>.*?)</ul>");
            if (childrenCat != null)
            {
                var blist = RegGroupCollection(childrenCat, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "cid=(?<x>\\d+)$");
                        if (!HasBindClasslist.Exists(c => c.ClassId == bId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = "",
                                ParentName = "",
                                ClassName = bName,
                                ClassId = bId,
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
                                Urlinfo = string.Format("http://www.vmei.com/products?cid={0}", bId),
                                UpdateTime = DateTime.Now,
                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(iteminfo);
                            shopClasslist.Add(iteminfo);
                        }

                    }
                }

            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }


            if (childrenCat != null)
                siteClassInfo.HasChild = true;
            else
                siteClassInfo.HasChild = false;


            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = string.Format("http://www.vmei.com/products?cid={0}", parentId);
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "<p>共搜索到<em class=\"count\">(?<x>\\d+)</em>个结果");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
