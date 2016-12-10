using System;
using System.Collections.Generic;
using System.Linq;
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
                "<div class=\"category-content\" id=\"category_box\"(?<x>.*?)var category");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" target=\"_blank\" class=\"level\\d+\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                if(!tempUrl.Contains("gallery"))
                { continue;}
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
            if ( page.Contains("http://114.119.9.120/public/images/05/79/7c/12ca40f78adb89f3f29cbff1adf5255e4927baef.jpg"))
            {
                new SiteClassInfoDB().SetIsDel(siteClassInfo);
                return;
            }
            string catName = RegGroupsX<string>(page, "<meta name=\"description\" content=\"(?<x>.*?)\" />");
            if (!string.IsNullOrEmpty(catName))
            {
                siteClassInfo.ClassName = catName;
            }

            string cromb = RegGroupsX<string>(page, "您已选择：</dt>(?<x>.*?)</dl>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<abbr>(?<x>.*?)</abbr>");
            if (plist == null)
                return;

            string parentUrl = "";
            string parentName = "";
            string parentId = "";


            for (int i = 0; i < plist.Count-1; i++)
            {
                var tempName = plist[i].Groups["x"].Value;
                var catitem = HasBindClasslist.FirstOrDefault(c => c.ClassName == tempName);
                if (catitem == null)
                    continue;
                parentName = catitem.ClassName;
                parentUrl = catitem.Urlinfo;
                parentId = RegGroupsX<string>(parentUrl, "gallery-(?<x>\\d+).html");
            }

            string brotherCat = RegGroupsX<string>(page, "您已选择：(?<x>.*?)<div id=\"filter_lists\"");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "<li id=\"\\d+\"><a href=\"(?<x>.*?)\">(?<y>.*?)</a></li>");
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
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "total:(?<x>\\d+), pagecurrent");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
