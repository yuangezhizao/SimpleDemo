using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    /// <summary>
    /// 保税超市分类处理
    /// </summary>
    public class CnbuyersClassInfo : Cnbuyers, ISiteClassBLL
    {
        public CnbuyersClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(189);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.cnbuyers.cn/index.php?app=category");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catare = RegGroupsX<string>(directoryHtml, "<div class=\"recommend\">(?<x>.*?)</div>");
           
            var catlist = RegGroupCollection(catare, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (catlist == null || catlist.Count == 0)
                return;
            foreach (Match item in catlist)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "cate_id=(?<x>.*?)$");
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
                        Urlinfo = string.Format("http://www.cnbuyers.cn/{0}", tempUrl),
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
            siteClassInfo.Urlinfo = siteClassInfo.Urlinfo.Replace("&amp;", "&");
            string page = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);
            string cromb = RegGroupsX<string>(page, "<div class=\"keyword\">\r\n        当前位置:(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "'cate_id', '(?<x>\\d+)'");
                if (!ValidCatId(parentId))
                {
                    parentUrl = "";
                    parentName = "";
                    parentId = "";
                    continue;
                }
                else
                {
                    parentUrl = string.Format("http://www.cnbuyers.cn/index.php?app=search&cate_id={0}", parentId);
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
                        Urlinfo = string.Format("http://www.cnbuyers.cn/index.php?app=search&cate_id={0}", parentId),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }
            string childrenCat = RegGroupsX<string>(page, "<ul ectype=\"ul_category\">(?<x>.*?)</ul>");
            if (childrenCat != null)
            {
                var blist = RegGroupCollection(childrenCat, "id=\"(?<x>.*?)\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string bId = item.Groups["x"].Value;
                        if (!ValidCatId(bId))
                            continue;
                        string bName = item.Groups["y"].Value;
                        string burl = string.Format("http://www.cnbuyers.cn/index.php?app=search&cate_id={0}", bId);
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
                                Urlinfo = burl,
                                UpdateTime = DateTime.Now,
                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(iteminfo);
                            shopClasslist.Add(iteminfo);
                        }

                    }
                }

                if (blist != null && blist.Count>0)
                    siteClassInfo.HasChild = true;
                else
                    siteClassInfo.HasChild = false;

            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page," <a class=\"stat\">共 (?<x>\\d+) 个项目</a>");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);




        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
