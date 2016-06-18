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
    public class YmatouClassInfo : Ymatou, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private const string domain = "http://www.ymatou.com";
        public YmatouClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(246);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string classurl = "http://www.ymatou.com/allcategory";
            string page = HtmlAnalysis.Gethtmlcode(classurl);
             
            string content = RegGroupsX<string>(page, "<div class=\"kinds\" id=\"itemOfKinds\">(?<x>.*?)<div class=\"pdClsBrands\">");

            var catList = RegGroupCollection(content, "<a href=\"(?<x>.*?)\"( title=\".*?\")?>(?<y>.*?)</a>");
            for (int i = 0; i < catList.Count; i++)
            {
                string tempurl = catList[i].Groups["x"].Value;
                if (string.IsNullOrEmpty(tempurl) || tempurl.Contains("brand"))
                    continue;
                tempurl = domain + tempurl;
                string tempName = catList[i].Groups["y"].Value;
                string catid = RegGroupsX<string>(tempurl, "http://www.ymatou.com/(?<x>.*?)$");
                if (!HasBindClasslist.Exists(p => p.ClassId == catid))
                {
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
                        ClassName = tempName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);
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
            //if (pageinfo.Contains("很抱歉，没有找到相关的商品。"))
            //    return;
            string crumb = RegGroupsX<string>(pageinfo,"<dl class=\"crumb\">(?<x>.*?)</dl>");

            var catlist = RegGroupCollection(crumb, "<dd(?<x>.*?)</dd>");
            if (catlist!=null&&catlist.Count > 0)
            {
                var tempcats = RegGroupCollection(catlist[1].Groups["x"].Value, "<li><a href=\"(?<x>.*?)\">(?<y>.*?)</a></li>");
                if(tempcats!=null)
                {

                    foreach (Match match in tempcats)
                    {
                        string tempcat = match.Groups["x"].Value;
                        if (!ValidCatId(tempcat))
                            continue;
                        tempcat = tempcat.TrimStart('/');
                        if (!HasBindClasslist.Exists(p => p.ClassId == tempcat))
                        {
                            string tempname = match.Groups["y"].Value;
                            SiteClassInfo cat = new SiteClassInfo
                            {

                                ParentUrl = "",
                                ParentClass = "",
                                ParentName = "",
                                TotalProduct = 0,
                                Urlinfo = domain + match.Groups["x"].Value,
                                ClassId = tempcat,
                                UpdateTime = DateTime.Now,
                                IsDel = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = true,
                                IsBind = false,
                                IsHide = false,
                                ClassName = tempname,
                                SiteId = Baseinfo.SiteId,
                                ClassCrumble = "",

                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(cat);
                            shopClasslist.Add(cat);
                        }
                
                    }
                }


            }
            if (catlist!=null&&catlist.Count == 2)
            {
                string tempcatid = RegGroupsX<string>(crumb, "<a class=\"active\" href=\"(?<x>.*?)\">");
                if (tempcatid != null)
                {
                    siteClassInfo.ClassId = tempcatid.TrimStart('/');
                    siteClassInfo.ClassName = RegGroupsX<string>(crumb, "<a class=\"active\" href=\".*?\">(?<x>.*?)</a>");
                }
                siteClassInfo.ParentName = RegGroupsX<string>(crumb, "<h3>(?<x>.*?)</h3>");
                siteClassInfo.ParentClass = RegGroupsX<string>(tempcatid, "/(?<x>.*?)/");

                if (!HasBindClasslist.Exists(p => p.ClassId == siteClassInfo.ParentClass))
                {
                    SiteClassInfo cat = new SiteClassInfo
                    {

                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = "",
                        ClassId = siteClassInfo.ParentClass,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
                        IsBind = false,
                        IsHide = false,
                        ClassName = siteClassInfo.ParentName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    if (ValidCatId(siteClassInfo.ParentClass))
                    {
                        HasBindClasslist.Add(cat);
                        shopClasslist.Add(cat);
                    }
                }


            }

            if (catlist != null && catlist.Count == 3)
            {
                string tempcatid = RegGroupsX<string>(catlist[2].Groups["x"].Value, "href=\"(?<x>.*?)\">");
                if (tempcatid != null)
                {
                    siteClassInfo.ClassId = tempcatid.TrimStart('/');
                    siteClassInfo.ClassName = RegGroupsX<string>(catlist[2].Groups["x"].Value, "href=\".*?\">(?<x>.*?)</a>");
                }

                siteClassInfo.ParentName = RegGroupsX<string>(catlist[1].Groups["x"].Value, "<a class=\"active\" href=\".*?\">(?<x>.*?)</a>");
                siteClassInfo.ParentUrl = domain + RegGroupsX<string>(catlist[1].Groups["x"].Value, "<a class=\"active\" href=\"(?<x>.*?)\">");
                siteClassInfo.ParentClass = RegGroupsX<string>(siteClassInfo.ParentUrl, "http://www.ymatou.com/(?<x>.*?)$");
            }

            string catArea = RegGroupsX<string>(pageinfo, "<ul class=\"xcatlist xclearfix\">(?<x>.*?)</ul>");
            if (!string.IsNullOrEmpty(catArea))
            {
                siteClassInfo.HasChild = !catArea.Contains("active");
                var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" ccount=\"\\d+\"( class=\"active\")? title=\"(?<y>.*?)\">");
                if (list != null)
                {
                    foreach (Match match in list)
                    {
                        string tempid = match.Groups["x"].Value.TrimStart('/');
                        if (!ValidCatId(tempid))
                        {
                            continue;
                        }
                        if (!HasBindClasslist.Exists(p => p.ClassId == tempid))
                        {
                            string tempurl = domain + match.Groups["x"].Value;
                            string tempName = match.Groups["y"].Value;
                            SiteClassInfo cat = new SiteClassInfo
                            {
                                
                                ParentUrl = "",
                                ParentClass = "",
                                ParentName = "",
                                TotalProduct = 0,
                                Urlinfo = tempurl,
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

                    }
                    if (shopClasslist.Count > 0)
                    {
                        new SiteClassInfoDB().AddSiteClass(shopClasslist);
                        shopClasslist.Clear();
                    }
                }
            }
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "<div class=\"percent\"><span>1/(?<x>\\d+)</span></div>") * 40;
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
