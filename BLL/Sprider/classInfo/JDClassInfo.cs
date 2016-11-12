using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Commons;
using Mode;
using SpriderProxy.Analysis;
using DataBase;

namespace BLL.Sprider.classInfo
{
    public class JdClassInfo : JingDong,ISiteClassBLL
    {
        public JdClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(1);
        }

        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();

        private const string Regid ="http://channel.jd.com/(?<x>\\w+).html|/(?<x>\\d*-\\d*(-\\d*)?).html|cat=(?<x>\\d*,\\d*(,\\d*)?)";
        /// <summary>
        /// 获取所有分类
        /// </summary>
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            HtmlAnalysis req= new HtmlAnalysis();
            req.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.Headers = new Dictionary<string, string>();
            req.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            req.Headers.Add("Upgrade-Insecure-Requests", "1");
    
            req.RequestUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36";
     
            string directoryHtml = req.HttpRequest("http://www.jd.com/allSort.aspx");
            //string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.jd.com/allSort.aspx");
   
            string allsort = RegGroupsX<string>(directoryHtml, "<!--左主体分类-->(?<x>.*?)<!--彩票-->");
            var list = RegGroupCollection(allsort, "<a( title=\".*?\")? href=\"(?<x>.*?)\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                AddJdNode(item.Groups["x"].Value, item.Groups["y"].Value);
                if (shopClasslist.Count > 50)
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

            //directoryHtml = directoryHtml.Substring("\ncategory.getDataService({\"data\":".Length);
            //directoryHtml = directoryHtml.Substring(0, directoryHtml.Length - 2);
            //var htmlllist = ServiceStack.Text.JsonSerializer.DeserializeFromString<List<JDcategorysJson>>(directoryHtml);
            //GetJdClassInfo(htmlllist);

        }

        /// <summary>
        /// 更新分类
        /// </summary>
        public void UpdateSiteCat()
        {

            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                if (string.IsNullOrEmpty(HasBindClasslist[i].ClassId))
                    continue;
                try
                {
                    UpdateJdNode(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }

        }

   

        private void UpdateJdNode(SiteClassInfo catinfo)
        {

            if (catinfo.ClassId.Split(',').Length == 2)
            {
                string pclass= RegGroupsX<string>(catinfo.ParentUrl, Regid);
                if (!string.IsNullOrEmpty(pclass))
                    catinfo.ParentClass = RegGroupsX<string>(catinfo.ParentUrl, Regid);
                catinfo.UpdateTime = DateTime.Now;
                catinfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == catinfo.ClassId);
                new SiteClassBll().UpdateSiteCat(catinfo);
            }


            
            
            string pageinfo = HtmlAnalysis.Gethtmlcode(catinfo.Urlinfo);
            if (pageinfo.Contains("抱歉，没有找到相关的商品"))
                return;


            string sortlist = RegGroupsX<string>(pageinfo,
                "<div id=\"sortlist\"(?<x>.*?)(<div id=\"limitBuy\">|<div class=\"m rank\" clstag=)|<div class=\"crumbs-nav\">(?<x>.*?)id=\"J_searchWrap\"");
            if (sortlist == null)
            {
                if (!catinfo.HasChild)
                    LogServer.WriteLog("未取得分类编号2" + catinfo.Urlinfo + "\t" + catinfo.ClassName, "AddClassError");
                return;
            }

            MatchCollection catList = RegGroupCollection(sortlist, "<a[^>]*?>(?<Text>[^<]*)</a>");

            if (catList == null)
                return;
                //string newCat = "";
            foreach (Match t in catList)
            {
                string classname = t.Groups["Text"].Value.Trim();
                string urlinfo = RegGroupsX<string>(t.ToString(),
                    "<a href = \"(?<x>.*?)\">|<a href=\"(?<x>.*?)\"|<a href='(?<x>.*?)'");
                string tempCatId = RegGroupsX<string>(urlinfo,Regid);
                if (!ValidCatId(tempCatId))
                {
                    continue;
                }
                if (tempCatId.Contains('-'))
                    tempCatId = tempCatId.Replace('-', ',');

                if (tempCatId != "" && !HasBindClasslist.Exists(p => p.ClassId == tempCatId))
                {

                    AddJdNode(tempCatId, "");
                    //newCat += t.ToString();
                }
                if (tempCatId == "" && classname != "" && !HasBindClasslist.Exists(p => p.ClassName == classname))
                {
                    AddJdNode(tempCatId, "");
                    //newCat += t.ToString();
                }
            }

            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

            List<SiteClassInfo> preCrumbinfo = new List<SiteClassInfo>();
            try
            {
                preCrumbinfo = GetCurrentCrumb(pageinfo);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }
            if (preCrumbinfo == null || preCrumbinfo.Count == 0)
                return;
            var newcat = preCrumbinfo[preCrumbinfo.Count - 1];
            if (catinfo.ClassId != newcat.ClassId && catinfo.ClassName != newcat.ClassName)
                return;
            catinfo.ClassName = newcat.ClassName;
            catinfo.TotalProduct = RegGroupsX<int>(pageinfo, "共&nbsp;<span>(?<x>\\d+)</span>个商品");
            catinfo.ClassCrumble = newcat.ClassCrumble ?? catinfo.ClassCrumble;
            if (preCrumbinfo.Count > 1)
            {
                var newParcat = preCrumbinfo[preCrumbinfo.Count - 2];
                if (!string.IsNullOrEmpty(newParcat.ClassId))
                    catinfo.ParentClass = newParcat.ClassId;
                catinfo.ParentUrl = newParcat.Urlinfo;
                if (!regIsMatch(catinfo.ParentClass, "[0-9]"))
                {
                    var tempcats = catinfo.ClassId.Split(',');

                    if (tempcats.Length > 2)
                    {
                        string pcat = "";
                        for (int i = 0; i < tempcats.Length - 1; i++)
                        {
                            if (i == 0)
                                pcat = tempcats[i];
                            else
                                pcat = pcat + "," + tempcats[i];
                        }
                        catinfo.ParentClass = pcat;
                    }
                }
                catinfo.ParentName = newParcat.ClassName;

            }
            catinfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == catinfo.ClassId);
            catinfo.UpdateTime = DateTime.Now;
            new SiteClassBll().UpdateSiteCat(catinfo);

       
        }

        private void AddJdNode(string cid, string className)
        {
            if (string.IsNullOrEmpty(cid))
            {
                LogServer.WriteLog("未取得分类编号1" + cid + "\t" + className, "AddClassError");
                return;
            }
            string url;
            if (cid.Contains("http"))
                url = cid;
            else
                url = "http://list.jd.com/list.html?cat=" + cid.Replace('-', ',');
            var tempid = (RegGroupsX<string>(url, Regid) ?? "").Replace('-', ',');
            if (HasBindClasslist.Exists(c => c.ClassId == tempid))
                return;

            if (url.Contains("http://channel.jd.com/"))
            {
                SiteClassInfo tempCat = new SiteClassInfo();
                tempCat.ClassId = tempid;
                tempCat.TotalProduct = 0;
                tempCat.ClassName =className;
                tempCat.Urlinfo = url;
                tempCat.ClassCrumble = "";
                tempCat.ParentClass = "";
                tempCat.ParentUrl = "";
                tempCat.ParentName = "";
                tempCat.IsHide = false;
                tempCat.IsDel = false;
                tempCat.SiteId = Baseinfo.SiteId;
                tempCat.BindClassName = "";
                tempCat.CreateDate = DateTime.Now;
                tempCat.UpdateTime = DateTime.Now;
                HasBindClasslist.Add(tempCat);
                shopClasslist.Add(tempCat);
                return;
            }


            if (cid != "" && HasBindClasslist.Exists(c => c.ClassId == cid))
                return;
          
            string pageinfo = HtmlAnalysis.Gethtmlcode(url);
            SiteClassInfo curCat = new SiteClassInfo();
            
            var curCrumbinfo = GetCurrentCrumb(pageinfo);
            if (curCrumbinfo == null || curCrumbinfo.Count == 0)
                return;
            var newcat = curCrumbinfo[curCrumbinfo.Count - 1];
            curCat.ClassId = newcat.ClassId;
            
            if (HasBindClasslist.Exists(c => c.ClassId == curCat.ClassId))
                return;
            curCat.TotalProduct = newcat.TotalProduct;
            curCat.ClassName = newcat.ClassName;
            curCat.Urlinfo = url;
            curCat.ClassCrumble = newcat.ClassCrumble ?? curCat.ClassCrumble;
            if (curCrumbinfo.Count > 1)
            {
                var newParcat = curCrumbinfo[curCrumbinfo.Count - 2];
                curCat.ParentClass = newParcat.ClassId;
                curCat.ParentUrl = newParcat.Urlinfo;
                curCat.ParentName = newParcat.ClassName;
            }
            curCat.IsHide = false;
            curCat.IsDel = false;
            curCat.SiteId = Baseinfo.SiteId;
            curCat.BindClassName = "";
            curCat.CreateDate = DateTime.Now;
            curCat.UpdateTime = DateTime.Now;
            HasBindClasslist.Add(curCat);
            shopClasslist.Add(curCat);


            string sortlist = RegGroupsX<string>(pageinfo, "<div id=\"sortlist\"(?<x>.*?)(<div id=\"limitBuy\">|<div class=\"m rank\" clstag=)|<div class=\"crumbs-nav\">(?<x>.*?)id=\"J_searchWrap\"");
            if (sortlist == null)
            {
                LogServer.WriteLog("未取得分类编号2" + url, "AddClassError");
                return;
            }

            MatchCollection catList = RegGroupCollection(sortlist, "<a[^>]*?>(?<Text>[^<]*)</a>");
            //string newCat = "";
            foreach (Match t in catList)
            {
                string classname = t.Groups["Text"].Value.Trim();
                string urlinfo = RegGroupsX<string>(t.ToString(),
                    "<a href = \"(?<x>.*?)\">|<a href=\"(?<x>.*?)\"|<a href='(?<x>.*?)'");
                string tempCatId = RegGroupsX<string>(urlinfo, Regid);
                if (!ValidCatId(tempCatId))
                {
                    continue;
                }
                if (tempCatId.Contains('-'))
                    tempCatId = tempCatId.Replace('-', ',');

                if (tempCatId != "" && !HasBindClasslist.Exists(p => p.ClassId == tempCatId))
                {
                    AddJdNode(tempCatId, "");
                    //newCat += t.ToString();
                }
                if (tempCatId == "" && classname != "" && !HasBindClasslist.Exists(p => p.ClassName == classname))
                {
                    AddJdNode(tempCatId, "");
                    //newCat += t.ToString();
                }
            }


          
        }

        private List<SiteClassInfo> GetCurrentCrumb(string pageInfo)
        {
            int total = RegGroupsX<int>(pageInfo, "共<strong>(?<x>\\d*)</strong>个商品|共&nbsp;<span>(?<x>\\d+)</span>个商品");
            string breadcrumb = RegGroupsX<string>(pageInfo, "<div class=\"breadcrumb\">(?<x>.*?)<div class=\"w main\">|replaceWith\\(\"<strong>(?<x>.*?)\"\\)</script>");
            if (breadcrumb == null)
            {
                List<SiteClassInfo> catslist = new List<SiteClassInfo>();
                breadcrumb = RegGroupsX<string>(pageInfo, "<div class=\"crumbs-nav\">(?<x>.*?)id=\"J_searchWrap\"");
                if (breadcrumb == null)
                    return null;
                var currlist = RegGroupCollection(breadcrumb, "<div class=\"crumbs-nav-item\">(?<x>.*?)</ul>");
                if (currlist == null)
                    return null;
                foreach (Match t in currlist)
                {
                    var curr = RegGroupsX<string>(t.ToString(), "<div class=\"trigger\"><span class=\"curr\">(?<x>.*?)</span><i class=\"menu-drop-arrow\"></i></div>");
                    MatchCollection templi = RegGroupCollection(breadcrumb, "<li><a href=\"(?<y>.*?)\" title=\".*?\">(?<x>.*?)</a></li>");
                    if (templi == null)
                        continue;
                    foreach (Match li in templi)
                    {
                        if (li.Groups["x"].Value == curr)
                        {
                            var tempurl = li.Groups["y"].Value;
                            if (!tempurl.Contains("http:"))
                                tempurl = "http://channel.jd.com" + tempurl;
                            var tempid = (RegGroupsX<string>(tempurl, Regid) ?? "").Replace('-', ',');
                            catslist.Add(new SiteClassInfo { TotalProduct = total, ClassName = curr, ClassId = tempid ,Urlinfo=tempurl});
                        }
                    }
                       
                }

                return catslist;
            }
              
            MatchCollection crumbMatch = RegGroupCollection(breadcrumb, "<a[^>]*?>(?<Text>[^<]*)</a>");
            List<SiteClassInfo> classlist = new List<SiteClassInfo>();
            
            string crumb = "";
            //抓取面包屑导航用来定位父类
            foreach (Match t in crumbMatch)
            {
                SiteClassInfo classinfo = new SiteClassInfo();
                classinfo.ClassName = t.Groups["Text"].Value.Trim();
                if (classinfo.ClassName == "图书")
                    return null;
                classinfo.Urlinfo = RegGroupsX<string>(t.ToString(),
                    "<a href = \"(?<x>.*?)\">|<a href=\"(?<x>.*?)\"|<a href=\\\\\"(?<x>.*?)\\\\\"");

                classinfo.TotalProduct = total;

                classinfo.ClassId = RegGroupsX<string>(classinfo.Urlinfo, Regid);
                if (!ValidCatId(classinfo.ClassId))
                    continue;

                if (classinfo.ClassId.IndexOf('-') > -1)
                    classinfo.ClassId = classinfo.ClassId.Replace('-', ',');
                crumb += classinfo.ClassId + "|";
                classlist.Add(classinfo);
            }
            if (classlist.Count == 0)
                return null;
            classlist[classlist.Count - 1].ClassCrumble = crumb.TrimEnd('|');
            return classlist;
        }

      
        private List<SiteClassInfo> HasBindClasslist { get; set; }



        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }




}
