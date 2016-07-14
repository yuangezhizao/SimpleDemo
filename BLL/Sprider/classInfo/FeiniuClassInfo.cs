using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class FeiniuClassInfo : Feiniu, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public FeiniuClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(161);
        }

        private static CookieContainer _cookie;
        private static CookieContainer Cookies
        {
            get
            {
                _cookie = new CookieContainer();
                var ckdist = new Cookie
                {
                    Value = "CPG1_CS000016",
                    Name = "C_dist",
                    Domain = ".feiniu.com",
                    Path = "/",
                    Expires = DateTime.Now.AddMonths(1)
                };
                var ckdistarea = new Cookie
                {
                    Value = "S000016_310100_310113_3101130001",
                    Name = "C_dist_area",
                    Domain = ".feiniu.com",
                    Path = "/",
                    Expires = DateTime.Now.AddMonths(1)
                };
                _cookie.Add(ckdist);
                _cookie.Add(ckdistarea);
                return _cookie;
            }
        }


        public void SaveAllSiteClass()
        {
             HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
             string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.feiniu.com/sitemap", Cookies);
             directoryHtml = RegGroupsX<string>(directoryHtml, "<!-- 全部分类左侧-->(?<x>.*?)</section>");

             var list = RegGroupCollection(directoryHtml, "<a(?<x>.*?)</a>");
            for (int i = 0; i < list.Count; i++)
            {
               
                string item = list[i].Groups["x"].Value;
                string href = RegGroupsX<string>(item, "href=\"(?<x>.*?)\"");
                if (href.IndexOf("category", StringComparison.Ordinal)==0)
                {
                    href = "http://www.feiniu.com/"+ href;
                }
                string proName = RegGroupsX<string>(item, " target=\"_blank\">(?<x>.*?)$");
                if (href.Contains("market"))
                {
                    string parId = RegGroupsX<string>(href, "market/(?<x>.*?)$");
                    if (string.IsNullOrEmpty(parId))
                        continue;
                    if (HasBindClasslist.Exists(p => p.ClassId == parId))
                        continue;
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = href,
                        ClassId = parId,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = false,
                        IsBind = false,
                        IsHide = false,
                        ClassName = proName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);

                }
                else
                    AddNode(href);
                if (shopClasslist.Count > 49)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
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
                    if(HasBindClasslist[i].Urlinfo.Contains("market"))
                        continue;
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

            string pageinfo = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo, Cookies);

            string fncatid = RegGroupsX<string>(pageinfo, " dsp_object.cate_id = \"(?<x>.*?)\"");
            string one_cate = RegGroupsX<string>(pageinfo, " dsp_object.one_cate = \"(?<x>.*?)\"");
            string two_cate = RegGroupsX<string>(pageinfo, " dsp_object.two_cate = \"(?<x>.*?)\"");
            string three_cate = RegGroupsX<string>(pageinfo, " dsp_object.three_cate = \"(?<x>.*?)\"");
            string four_cate = RegGroupsX<string>(pageinfo, " dsp_object.four_cate = \"(?<x>.*?)\"");
            string parentName = "";
            string catname = "";
            if (siteClassInfo.ClassId != fncatid)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            if (!string.IsNullOrEmpty(four_cate))
            {
                catname = four_cate;
                parentName = three_cate;
            }
            else if (!string.IsNullOrEmpty(three_cate))
            {
                catname = three_cate;
                parentName = two_cate;
            }
            else if (!string.IsNullOrEmpty(two_cate))
            {
                catname = two_cate;
                parentName = one_cate;
            }

            if (parentName != siteClassInfo.ClassName)
            {
                siteClassInfo.ParentName = parentName;
            }

            if (!string.IsNullOrEmpty(catname))
            {
                siteClassInfo.ClassName = catname;
            }
            var sonpage = RegGroupsX<string>(pageinfo, "<ul class=\"v-lst J-lst\">(?<x>.*?)</ul>");
            var soncat = RegGroupCollection(sonpage, "category/(?<x>C\\d+)");
            if (soncat != null && soncat.Count > 0)
            {
                var db = new mmbSiteClassInfoDB();
                foreach (Match catinf in soncat)
                {
                    var catitem = HasBindClasslist.FirstOrDefault(c => c.ClassId == catinf.Groups["x"].Value);
                    if (catitem == null)
                        continue;
                    catitem.ParentUrl = siteClassInfo.Urlinfo;

                    if (siteClassInfo.ClassId != catitem.ClassId)
                    {
                        catitem.ParentClass = siteClassInfo.ClassId;
                    }
               
                    if (siteClassInfo.ClassName != catitem.ClassName)
                    {
                        catitem.ParentName = siteClassInfo.ClassName;
                    }
                   
                    db.UpdateSiteClass(catitem);
                }
            }
            
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "<div class=\"t-s\">共<span>(?<x>\\d+)</span>个商品</div>");
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

            var catlist =RegGroupCollection(pageinfo, "category/C(?<x>\\d+)");

            foreach (Match item in catlist)
            {

                string tempid = item.Groups["x"].Value;
                string tempurl=$"http://www.feiniu.com/category/C{tempid}";
                AddNode(tempurl);
            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

        }
        private void  AddNode(string url)
        {
            string classid = RegGroupsX<string>(url, "category/(?<x>.*?)$");
            if (string.IsNullOrEmpty(classid))
                return;
            if (HasBindClasslist.Exists(p => p.ClassId == classid))
                return;
            string pageinfo = HtmlAnalysis.Gethtmlcode(url, Cookies);

            string cromp = RegGroupsX<string>(pageinfo, "<!-- 面包屑 -->(?<x>.*?)<!-- 品牌 -->");
            int total = RegGroupsX<int>(pageinfo, "<div class=\"t-s\">共<span>(?<x>\\d+)</span>个商品</div>");
            if (string.IsNullOrEmpty(cromp)) return;

            var caplist = RegGroupCollection(cromp, "<div class=\"u-bar-item\">(?<x>.*?)</div>");
            if (caplist == null)
                return;
            string one_cate = RegGroupsX<string>(pageinfo, " dsp_object.one_cate = \"(?<x>.*?)\"");
            string two_cate = RegGroupsX<string>(pageinfo, " dsp_object.two_cate = \"(?<x>.*?)\"");
            string three_cate = RegGroupsX<string>(pageinfo, " dsp_object.three_cate = \"(?<x>.*?)\"");
            string four_cate = RegGroupsX<string>(pageinfo, " dsp_object.four_cate = \"(?<x>.*?)\"");
            string parentName = "";
            string catname = "";

            if (!string.IsNullOrEmpty(four_cate))
            {
                catname = four_cate;
                parentName = three_cate;
            }
            else if (!string.IsNullOrEmpty(three_cate))
            {
                catname = three_cate;
                parentName = two_cate;
            }
            else if (!string.IsNullOrEmpty(two_cate))
            {
                catname = two_cate;
                parentName = one_cate;
            }
            SiteClassInfo cat = new SiteClassInfo
            {
                ParentUrl = "",
                ParentClass = "",
                ParentName = parentName,
                TotalProduct = total,
                Urlinfo = url,
                ClassId = classid,
                UpdateTime = DateTime.Now,
                IsDel = false,
                BindClassId = 0,
                BindClassName = "",
                HasChild = false,
                IsBind = false,
                IsHide = false,
                ClassName = catname,
                SiteId = Baseinfo.SiteId,
                ClassCrumble = "",
                CreateDate = DateTime.Now
            };
            HasBindClasslist.Add(cat);
            shopClasslist.Add(cat);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
