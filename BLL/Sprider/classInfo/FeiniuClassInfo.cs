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
                var session = new Cookie
                {
                    Value = "BF5CABE0A6DB1754F54EBB8551283433",
                    Name = "JSESSIONID",
                    Domain = "list.feiniu.com",
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
                _cookie.Add(session);
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
                if (href.IndexOf("http://list.feiniu.com/", StringComparison.Ordinal)==-1)
                {
                    href = "http://list.feiniu.com/" + href;
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
            HtmlAnalysis requent = new HtmlAnalysis();
            requent.Headers.Add("Cookie", "cart_token=cb924284c3a93939d49947a2c52bdadd_1473496950515; guid=8E7A996D-D48A-4249-9EE9-A9CD3B0C01F4; first_login_time=1473496955710; access=192; Hm_lvt_7f78a821324600a0f059acdb24cf0937=1478745768,1478825562,1479197457,1479287754; _ga=GA1.2.2146744620.1473496958; _uniut_id.633=3eb90e8fda122a08%7C1473496958%7C1%7C1479326814%7C1479306233%7C80DBB23C04294257D3C779; _uni_id=80DBB23C04294257D3C779; _jzqa=1.3351778468201770500.1473496958.1479306234.1479308663.194; _pzfxuvpc=1473496958495%7C7429572381140915729%7C710%7C1479326813730%7C200%7C1308848803776733003%7C3755478111650129946; C_dist=CPG1_CS000016; C_dist_area=CS000016_310100_310113_3101130001; abToken=72; _jzqx=1.1477718245.1477718245.1.jzqsr=c%2Eduomai%2Ecom|jzqct=/track%2Ephp.-; _jzqckmp=1; Hm_lpvt_7f78a821324600a0f059acdb24cf0937=1479326814; _jzqc=1; _qzja=1.1797373537.1473496958384.1479306233483.1479308663278.1479326483747.1479326813706..0.0.710.194; CNZZDATA1256948007=1677849392-1477715224-%7C1478589142; CNZZDATA1256614422=283867746-1477716746-%7C1478585114; CNZZDATA1256613015=2111691907-1477717194-%7C1478585626; resource_id=undefined; _qzjto=39.0.0; TS01efe1c2=01cfbf1eb5213b341f69517951b47fc3af39bdb07f85a9d1893767fa0e705525d917514dba; _qzjc=1");
            requent.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101";
            string url = siteClassInfo.Urlinfo.Replace("http://www.feiniu.com/category/", "http://list.feiniu.com/");

            string pageinfo = requent.HttpRequest(url); //HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo, Cookies);
            siteClassInfo.Urlinfo = url;

            string fncatid = RegGroupsX<string>(pageinfo, " dsp_object.cate_id = \"(?<x>.*?)\"");
            string one_cate = RegGroupsX<string>(pageinfo, " dsp_object.one_cate = \"(?<x>.*?)\"");
            string two_cate = RegGroupsX<string>(pageinfo, " dsp_object.two_cate = \"(?<x>.*?)\"");
            string three_cate = RegGroupsX<string>(pageinfo, " dsp_object.three_cate = \"(?<x>.*?)\"");
            string four_cate = RegGroupsX<string>(pageinfo, " dsp_object.four_cate = \"(?<x>.*?)\"");
            string parentName = "";
            //string catname = "";
            if (siteClassInfo.ClassId != fncatid)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + siteClassInfo.Urlinfo, "AddClassError");
                return;
            }
            if (!string.IsNullOrEmpty(four_cate))
            {
                //catname = four_cate;
                parentName = three_cate;
            }
            else if (!string.IsNullOrEmpty(three_cate))
            {
                //catname = three_cate;
                parentName = two_cate;
            }
            else if (!string.IsNullOrEmpty(two_cate))
            {
                //catname = two_cate;
                parentName = one_cate;
            }

            if (parentName != siteClassInfo.ClassName&& siteClassInfo.ParentName=="")
            {
                siteClassInfo.ParentName = parentName;
            }

            string cromp = RegGroupsX<string>(pageinfo, "<!-- 面包屑 -->(?<x>.*?)<!-- 品牌 -->");
            //string firstparent= RegGroupsX<string>(cromp, "<div class=\"u-bar-item\">\\s+<a class=\"link\" href=\"(?<y>.*?)\">(?<x>.*?)</a>");
            //string firsturl = RegGroupsX<string>(cromp, "<div class=\"u-bar-item\">\\s+<a class=\"link\" href=\"(?<x>.*?)\">");
            var cromplist = RegGroupCollection(cromp, "href=\"http://list.feiniu.com/(?<x>.*?)\""); //<a class=\"link\" href=\"javascript:;\">糖果巧克力</a>
            if(cromplist!=null&&cromplist.Count>0)
            foreach (Match cat in cromplist)
            {
                var tempcatid = cat.Groups["x"].Value;
                if (string.IsNullOrEmpty(tempcatid))
                    continue;
                var catitem = HasBindClasslist.FirstOrDefault(c => c.ClassId == tempcatid);
                if (catitem == null)
                {
                    AddNode($"http://list.feiniu.com/C{tempcatid}");
     
                }
            }

            //if (!string.IsNullOrEmpty(catname))
            //{
            //    siteClassInfo.ClassName = catname;
            //}
            var sonpage = RegGroupsX<string>(pageinfo, "<ul class=\"v-lst J-lst\">(?<x>.*?)</ul>");
            var soncat = RegGroupCollection(sonpage, "http://list.feiniu.com/(?<x>C\\d+)");
            if (soncat != null && soncat.Count > 0)
            {
             
                foreach (Match catinf in soncat)
                {
                    var tempcatid = catinf.Groups["x"].Value;
                    if(string.IsNullOrEmpty(tempcatid))
                        continue;
                    var catitem = HasBindClasslist.FirstOrDefault(c => c.ClassId == tempcatid);
                    if (catitem == null)
                    {
                        AddNode($"http://list.feiniu.com/C{tempcatid}");
                        continue;
                    }
                    if (string.IsNullOrEmpty(catitem.ParentClass))
                    {
                        var db = new mmbSiteClassInfoDB();
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
            }
            
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "<div class=\"t-s\">共<span>(?<x>\\d+)</span>个商品</div>");
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

            var catlist =RegGroupCollection(pageinfo, "http://list.feiniu.com/(?<x>C\\d+)");

            foreach (Match item in catlist)
            {

                string tempid = item.Groups["x"].Value;
                if (HasBindClasslist.Exists(p => p.ClassId == tempid))
                    continue;
                string tempurl=$"http://list.feiniu.com/{tempid}";
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
            string classid = RegGroupsX<string>(url, "http://list.feiniu.com/(?<x>.*?)$");
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
