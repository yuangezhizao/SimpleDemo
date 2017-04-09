using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;
namespace BLL.Sprider.classInfo
{
    public class KaolaClassInfo : Kaola, ISiteClassBLL
    {
        public KaolaClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(241);
        }
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            LoadCurrentCat();
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.kaola.com/getFrontCategory.html");

            var catinfo = JObject.Parse(directoryHtml);

            var list= catinfo["frontCategoryList"];
            foreach (var fitem in list)
            {
                string categoryId = fitem["categoryId"].Value<string>();
                string categoryName = fitem["categoryName"].Value<string>();
                string url = $"http://www.kaola.com/category/{categoryId}.html";
                if (!HasBindClasslist.Exists(c => c.ClassId == categoryId))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = categoryName,
                        ClassId = categoryId,
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
                        Urlinfo = url,
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }


                var firstChild = fitem["childrenNodeList"];
                if(firstChild==null)
                    continue;
                foreach (var fchild in firstChild)
                {
                    string fcategoryId = fchild["categoryId"].Value<string>();
                    string fcategoryName = fchild["categoryName"].Value<string>();
                    string furl = $"http://www.kaola.com/category/{fcategoryId}.html";
                    if (!HasBindClasslist.Exists(c => c.ClassId == fcategoryId))
                    {
                        SiteClassInfo iteminfo = new SiteClassInfo
                        {
                            ParentClass = categoryId,
                            ParentName = categoryName,
                            ClassName = fcategoryName,
                            ClassId = fcategoryId,
                            ParentUrl =url,
                            IsDel = false,
                            IsBind = false,
                            IsHide = false,
                            BindClassId = 0,
                            BindClassName = "",
                            HasChild = true,
                            ClassCrumble = "",
                            TotalProduct = 0,
                            SiteId = Baseinfo.SiteId,
                            Urlinfo = furl,
                            UpdateTime = DateTime.Now,
                            CreateDate = DateTime.Now
                        };
                        HasBindClasslist.Add(iteminfo);
                        shopClasslist.Add(iteminfo);
                    }

                    var secChild = fchild["childrenNodeList"];
                    if (secChild == null)
                        continue;
                    foreach (var sitem in secChild)
                    {
                        if(sitem["categoryId"] ==null)
                            continue;
                        string scategoryId = sitem["categoryId"].Value<string>();
                        string sfcategoryName = sitem["categoryName"].Value<string>();
                        string sfurl = $"http://www.kaola.com/category/{fcategoryId}/{scategoryId}.html";
                        if (!HasBindClasslist.Exists(c => c.ClassId == scategoryId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = fcategoryId,
                                ParentName = fcategoryName,
                                ClassName = sfcategoryName,
                                ClassId = fcategoryId + "/"+ scategoryId,
                                ParentUrl = furl,
                                IsDel = false,
                                IsBind = false,
                                IsHide = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = false,
                                ClassCrumble = "",
                                TotalProduct = 0,
                                SiteId = Baseinfo.SiteId,
                                Urlinfo = sfurl,
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

        }


        public void LoadCurrentCat()
        {
            if (HasBindClasslist == null || HasBindClasslist.Count == 0)
                HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            var page = DocumentServer.ReadFileInfo(@"C:\Users\Administrator\Desktop\kaola.txt");
            var cats = RegGroupCollection(page, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            foreach (Match cat in cats)
            {
                var url = cat.Groups["x"].Value;
                if (url.Length > 0 && url.IndexOf("http:") == -1)
                    url = "http:" + url;
                var name = cat.Groups["y"].Value;
                var catid = RegGroupsX<string>(url, "/category/(?<x>\\d+).html|/category/(?<x>\\d+/\\d+).html");
                if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = name,
                        ClassId = catid,
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
                        Urlinfo = name,
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
                    if(HasBindClasslist[i].ParentClass=="")
                    UpdateCat(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        
        }

        private string cookies = "";
        private void UpdateCat(SiteClassInfo siteClassInfo)
        {
            if (string.IsNullOrEmpty(cookies))
            {
                cookies = HtmlAnalysis.GetResponseCookies(siteClassInfo.Urlinfo);
                if (HtmlAnalysis.Headers.ContainsKey("Cookie"))
                    HtmlAnalysis.Headers["Cookie"] = cookies;
                else
                    HtmlAnalysis.Headers.Add("Cookie", cookies);

            }

            string page = HtmlAnalysis.HttpRequest(siteClassInfo.Urlinfo);
            if (page.Contains("我们的系统检测到您所在的网络对考拉的访问行为存在异常"))
            {
                cookies = "";
                return;
            }
            if (page.Contains("抱歉，没有找到符合条件的商品"))
            {
                new SiteClassInfoDB().SetIsDel(siteClassInfo);
            }
            string cromb = RegGroupsX<string>(page, "<div class=\"resultinfo clearfix\">(?<x>.*?)<label class=\"detail-search\">");
            if (cromb == null)
                return;
            var cats = RegGroupCollection(cromb, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            string parentName = "";
            string parentUrl = "";
            string parentId = "";
            foreach (Match cat in cats)
            {
                var catName = cat.Groups["y"].Value;
                var tempcatId = RegGroupsX<string>(cat.Groups["x"].Value, "/category/(?<x>\\d+).html|/category/(?<x>\\d+/\\d+).html");
                if (catName == "全部结果"|| tempcatId == siteClassInfo.ClassId)
                    continue;
                parentName = catName;
                parentUrl= cat.Groups["x"].Value;
                parentId = RegGroupsX<string>(parentUrl, "/category/(?<x>\\d+).html|/category/(?<x>\\d+/\\d+).html");
            }
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "商品共<i>(?<x>\\d+)</i>个</span>");
            if (!string.IsNullOrEmpty(parentId)&& parentId!=siteClassInfo.ClassId)
            {
                siteClassInfo.ParentClass = parentId;
                siteClassInfo.ParentName = parentName;
                siteClassInfo.ParentUrl = $"http://www.kaola.com/category/{parentId}.html";
            }
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
             cats = RegGroupCollection(page, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            foreach (Match cat in cats)
            {
                var url = cat.Groups["x"].Value;
                if (url.Length > 0 && url.IndexOf("http:") == -1)
                    url = "http:" + url;
                var name = cat.Groups["y"].Value;
                var catid = RegGroupsX<string>(url, "/category/(?<x>\\d+).html|/category/(?<x>\\d+/\\d+).html");
                if (string.IsNullOrEmpty(catid))
                    continue;
                if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = name,
                        ClassId = catid,
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
                        Urlinfo = name,
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

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
