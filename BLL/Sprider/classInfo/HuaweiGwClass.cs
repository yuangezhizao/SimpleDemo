    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Commons;
    using DataBase;
    using Mode;
    using SpriderProxy.Analysis;
namespace BLL.Sprider.classInfo
{
    public class HuaweiGwClass : HuaweiGw, ISiteClassBLL
    {
        public HuaweiGwClass()
        {
            Baseinfo = new SiteInfoDB().SiteById(142);
            if (Baseinfo == null)
            {
                Baseinfo = new SiteInfo { SiteId = 142 };
            }
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.vmall.com/index.html");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<ol class=\"category-list\">(?<x>.*?)<a href=\"http://app.vmall.com\" target=\"_blank\"><span>应用市场");
            if (catArea == null)
                return;

            catArea = catArea.Replace("\r", "").Replace("\n", "").Replace("\t", "");
           
            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" (target=\"_blank\")?><span>(?<y>.*?)</span>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "list-(?<x>\\d+)$");
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
                        Urlinfo = string.Format("http://www.vmall.com{0}", tempUrl),
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
            string cromb = RegGroupsX<string>(page, "<div class=\"breadcrumb-area fcn\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\" title=\"(?<y>.*?)\">");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains("首页"))
                    continue;
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "list-(?<x>\\d+)");
                if (!ValidCatId(parentId))
                {
                    parentId = "";
                    continue;
                }
                
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
                        Urlinfo = string.Format("http://www.vmall.com/{0}", parentUrl),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }

            string brotherCat = RegGroupsX<string>(page, "<div class=\"p-title\">分类：</div>(?<x>.*?)<div class=\"pro-cate-sort clearfix\">");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, " <li ><a href=\"(?<x>.*?)\">(?<y>.*?)</a></li>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "list-(?<x>\\d+)$");
                        if (!ValidCatId(bId))
                        {
                            continue;
                        }
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
                                Urlinfo = string.Format("http://www.vmall.com/{0}", burl),
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


            if (brotherCat != null)
                siteClassInfo.HasChild = false;
            else
                siteClassInfo.HasChild = true;


            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            if (!string.IsNullOrEmpty(parentUrl))
                siteClassInfo.ParentUrl = string.Format("http://www.vmall.com/{0}", parentUrl);
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
