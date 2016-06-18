using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;
namespace BLL.Sprider.classInfo
{
   public class SundianClassInfo : Sundian, ISiteClassBLL
   {
       public SundianClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(185);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.sundan.com/gallery-1.html");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"leftmenu\">(?<x>.*?)<div class=\"mod-box best-goods\">");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\">(?<y>.*?)<|<h4 ><a href=\"(?<x>.*?)\">(?<y>.*?)<|<a href=\"(?<x>.*?)\" class=\"biglink \">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "gallery-(?<x>\\d+).html");
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
                        Urlinfo = string.Format("http://www.sundan.com/gallery-{0}.html", tempid),
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
            if (page.Contains("尊敬的顾客！暂时没有商品"))
            {
                new SiteClassInfoDB().SetIsDel(siteClassInfo);
                return;
            }
                
            string cromb = RegGroupsX<string>(page, "您当前的位置(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a href=\"(?<x>.*?)\" alt=\"\" title=\"\"><em>(?<y>.*?)</em></a>");
            if (plist == null)
                return;


            string catName = RegGroupsX<string>(cromb, "<span>(?<x>.*?)</span>");
            if (!string.IsNullOrEmpty(catName))
            {
                siteClassInfo.ClassName = catName;
            }
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                if (item.ToString().Contains("首页"))
                    continue;

                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "gallery-(?<x>\\d+).html");
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
                        Urlinfo = string.Format("http://www.sundan.com/gallery-{0}.html", parentId),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }

            string brotherCat = RegGroupsX<string>(page, "<ul class=\"list ncate-list\">(?<x>.*?)</ul>");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;
                        string bId = RegGroupsX<string>(burl, "gallery-(?<x>\\d+).html");
                        if (!HasBindClasslist.Exists(c => c.ClassId == bId)&&ValidCatId(bId))
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
                                Urlinfo = string.Format("http://www.sundan.com/gallery-{0}.html", bId),
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


            if (brotherCat != null )
                siteClassInfo.HasChild = true;
            else
                siteClassInfo.HasChild = false;

            if (parentId == "")
            {
                siteClassInfo.ParentClass = "";
                siteClassInfo.ParentUrl = "";
                siteClassInfo.ParentName = "";
            }
            else
            {
                siteClassInfo.ParentClass = parentId;
                siteClassInfo.ParentName = parentName;
                siteClassInfo.ParentUrl = string.Format("http://www.sundan.com/gallery-{0}.html", parentId);
            }
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "<strong class=\"red\" id=\"totalgoodsnum\">(?<x>\\d+)</strong>");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
