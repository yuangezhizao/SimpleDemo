using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Commons;
using Mode;
using SpriderProxy.Analysis;
using DataBase;

namespace BLL.Sprider.classInfo
{
    public class YhdClassInfo : Yhd,ISiteClassBLL
    {
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public YhdClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(13);
        }



        /// <summary>
        /// 获取所有分类
        /// </summary>
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = Gethtmlcode("http://www.yhd.com/marketing/allproduct.html");
            var list = RegGroupCollection(directoryHtml, "<em><span><a href=\"(?<x>.*?)\"");

            foreach (Match item in list)
            {
                string url = item.Groups["x"].Value;

                GetYhdClassInfo(url);
       
            }

            //directoryHtml = directoryHtml.Substring("\ncategory.getDataService({\"data\":".Length);
            //directoryHtml = directoryHtml.Substring(0, directoryHtml.Length - 2);
            //var htmlllist = ServiceStack.Text.JsonSerializer.DeserializeFromString<List<JDcategorysJson>>(directoryHtml);
            //GetJdClassInfo(htmlllist);
        }

        private void GetYhdClassInfo(string url)
        {
            if (!url.Contains("b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/"))
                url += "b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/";
            string pageInfo = Gethtmlcode(url);
    
            string classInfo = RegGroupsX<string>(pageInfo, "<ul class=\"listCon clearfix\">(?<x>.*?)<a class=\"c_btn c_next iconSearch\"");


            var list = RegGroupCollection(classInfo, "href=\"(?<x>.*?)\".*?title=\"(?<y>.*?)\"");
            if (list == null)
                return;
        
            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";
            int total = RegGroupsX<int>(pageInfo, "共(?<x>\\d+?)条");

            if (list.Count == 1)
            {
                string categoryname = RegGroupsX<string>(pageInfo, "var categoryName = '(?<x>.*?)'");
                string extid = RegGroupsX<string>(pageInfo, "var expectCategoryId = \"(?<x>\\d+)\"");
                string tempcurcatid = "c" + extid + "-" + categoryname;
                if(HasBindClasslist.Exists(p => p.ClassId == tempcurcatid))
                    return;

                string current = RegGroupsX<string>(pageInfo, "<title>(?<x>.*?)品种齐全|<div class=\"guide_title\"><span title=\"(?<x>.*?)\">");
                SiteClassInfo catInfo = new SiteClassInfo
                {
                    Urlinfo = $"http://list.yhd.com/{tempcurcatid}/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k",
                    ClassId = tempcurcatid,
                    ClassName = current,
                    BindClassId = 0,
                    BindClassName = "",
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    SiteId = Baseinfo.SiteId,
                    IsBind = false,
                    IsDel = false,
                    IsHide = false
                };



                catInfo.HasChild = HasBindClasslist.Exists(p => p.ParentClass == catInfo.ClassId);
                catInfo.ParentClass = pcatId;
                catInfo.ParentName = pcatName;
                catInfo.ParentUrl = pcatUrl;
                if (regIsMatch(tempcurcatid, "^(?<x>c\\d+-\\d+(-\\d+)?)$"))
                {

                    if (!HasBindClasslist.Exists(p => p.ClassId == catInfo.ClassId))
                    {
                        new SiteClassInfoDB().AddSiteClass(catInfo);
                        LogServer.WriteLog("线程id：" + Thread.CurrentThread.ManagedThreadId + "\t" + catInfo.ClassId + "\t" + catInfo.ClassName + "111111111111111", "addpro");
                        HasBindClasslist.Add(catInfo);
                    }

                }
                return;
            }



            //SiteClassInfoDB db = new SiteClassInfoDB();
            for (int i = 0; i < list.Count; i++)
            {
                SiteClassInfo catInfo = new SiteClassInfo();
                catInfo.Urlinfo = list[i].Groups["x"].Value;
                catInfo.ClassId = RegGroupsX<string>(catInfo.Urlinfo, "http://list.yhd.com/(?<x>.*?)/");
                catInfo.ClassName = list[i].Groups["y"].Value;
                if (catInfo.Urlinfo == null || catInfo.ClassId == null || catInfo.ClassName == null)
                {
                    LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1url\t" + url, "AddClassError");
                    continue;
                }

                if (!HasBindClasslist.Exists(p => p.ClassId == catInfo.ClassId) )
                {
                    catInfo.ParentClass = pcatId;
                    catInfo.ParentName = pcatName;
                    catInfo.ParentUrl = pcatUrl;
                    if (i != 0 && pcatId!="")
                        classCrumble += pcatId + ",";
                    
                    catInfo.ClassCrumble = classCrumble.TrimEnd(',');
                    catInfo.BindClassId = 0;
                    catInfo.BindClassName = "";
                    catInfo.CreateDate = DateTime.Now;
                    catInfo.UpdateTime = DateTime.Now;
                    catInfo.SiteId = Baseinfo.SiteId;
                    catInfo.IsBind = false;
                    catInfo.IsDel = false;
                    catInfo.IsHide = false;
                    catInfo.HasChild = true;
                    if( list.Count - 1 == i)
                    {
                        catInfo.HasChild = false;
                        catInfo.TotalProduct = total;
                    }
                    HasBindClasslist.Add(catInfo);
                    LogServer.WriteLog("线程id："+Thread.CurrentThread.ManagedThreadId+ "\t" + catInfo.ClassId +"\t"+catInfo.ClassName+"111111111111111","addpro");
                    new SiteClassInfoDB().AddSiteClass(catInfo);
                   

                }

                pcatUrl = catInfo.Urlinfo;
                pcatName = catInfo.ClassName;
                pcatId = catInfo.ClassId;
            }

         



        }

        /// <summary>
        /// 更新分类
        /// </summary>
        public void UpdateSiteCat()
        {

            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            int total = HasBindClasslist.Count;

            for (int i = 0; i < total; i++)
            {
                if (string.IsNullOrEmpty(HasBindClasslist[i].ClassId))
                    continue;

                //if (string.IsNullOrEmpty(HasBindClasslist[i].ParentClass))
                //    continue;

                //if (HasBindClasslist.Any(p => p.ClassId == HasBindClasslist[i].ParentClass))
                //    continue;

                try
                {
                    ////if (string.IsNullOrEmpty(HasBindClasslist[i].ParentClass)||HasBindClasslist.Any(p => p.ClassId == HasBindClasslist[i].ParentClass))
                    ////    continue;
       
                    UpdateNode(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }

        }

        private void UpdateNode(SiteClassInfo siteClassInfo)
        {
            //if (!siteClassInfo.Urlinfo.Contains("b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k"))
            //    siteClassInfo.Urlinfo += "b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/";

            siteClassInfo.Urlinfo = $"http://list.yhd.com/{siteClassInfo.ClassId}/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k";
            string pageInfo = Gethtmlcode(siteClassInfo.Urlinfo);

            string categoryname = RegGroupsX<string>(pageInfo, "var categoryName = '(?<x>.*?)'");
            string extid = RegGroupsX<string>(pageInfo, "var expectCategoryId = \"(?<x>\\d+)\"");

            string tempcurcatid = "c" + extid + "-" + categoryname;
            if (!regIsMatch(tempcurcatid, "^(?<x>c\\d+-\\d+(-\\d+)?)$"))
            {
                var aa = "ddd";
                return;
            }
            if (categoryname == "0-0")
                tempcurcatid = "c" + extid;

            if (extid=="0")
                return;

            //string classInfo = RegGroupsX<string>(pageInfo, "<div id=\"searchColSub\"(?<x>.*?)<div id=\"bodyRight\"|<div class=\"crumbClip\">(?<x>.*?)<li class=\"crumb_search search_empty\">");
            //string classInfo = RegGroupsX<string>(pageInfo, "<div class=\"classWrap\">(?<x>.*?)<div class=\"brandWrap\">");
            string classInfo = RegGroupsX<string>(pageInfo, "<ul class=\"listCon clearfix\">(?<x>.*?)<a class=\"c_btn c_next iconSearch\"");
            string classinfo2 = RegGroupsX<string>(pageInfo, "<ul class=\"guide_con clearfix\">(?<x>.*?)</ul>");
            if (!string.IsNullOrEmpty(classinfo2))
            {
                classInfo += classinfo2;
            }
            string tempcatid = RegGroupsX<string>(siteClassInfo.Urlinfo, "c\\d+-0-(?<x>\\d+)/");
            if (!string.IsNullOrEmpty(tempcatid))
            {
                string classurl =
                    string.Format(
                        "http://list.yhd.com/lazyLoadBrotherCategory/nc{0}-a-f0d-mid0-k/?urlFilterSuffix=/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/",
                        tempcatid);

                string catpage = Gethtmlcode(classurl);
                if (!string.IsNullOrEmpty(catpage))
                    classInfo += catpage.Replace("\\\"","\"");
            }



            var catList = RegGroupCollection(classInfo, "href=\"(?<x>.*?)\".*?title=\"(?<y>.*?)\"");
            if (catList == null)
            {
                if (pageInfo.Contains("没有找到符合条件的商品，建议您更改下搜索条件")||pageInfo.Contains("很抱歉！没有找到与<span class=\"color_red\">\"\"</span>相关的商品，要不你换个关键词我帮你再找找吧"))
                    new SiteClassBll().delClass(siteClassInfo);
                return;
            }
            for (int i = 0; i < catList.Count; i++)
            {
                string url = catList[i].Groups["x"].Value;
                string catname = catList[i].Groups["y"].Value;
                if (string.IsNullOrEmpty(url))
                    continue;
                string catId = RegGroupsX<string>(url, "http://list.yhd.com/(?<x>.*?)/");
                if (!HasBindClasslist.Exists(p => p.ClassId == catId) && !HasBindClasslist.Exists(p => p.ClassName == catname))
                {
                    GetYhdClassInfo(url);
                }
            }
            
            //string crumb = RegGroupsX<string>(pageInfo,
            //    "<div class=\"mod_search_crumb clearfix\"(?<x>.*?)<div id=\"searchColSub\"");
            //string crumb = RegGroupsX<string>(pageInfo,"<div class=\"crumbClip\">(?<x>.*?)<li class=\"crumb_search search_empty\">|<div class=\"mod_search_crumb clearfix\"(?<x>.*?)<div id=\"searchColSub\"");
            //string crumb = RegGroupsX<string>(pageInfo, "<div class=\"crumbClip\">(?<x>.*?)<li class=\"crumb_search search_empty\">|<div class=\"mod_search_crumb clearfix\"(?<x>.*?)<div id=\"searchColSub\"");

            //if (crumb == null)
            //    return;
            //var list = RegGroupCollection(crumb, "<div class=\"crumb_list\">(?<x>.*?)</div>");
            var list = RegGroupCollection(classInfo, "<li class=\"crumb_list\">(?<x>.*?)</li>");
            if (list == null)
                return;
            string current = RegGroupsX<string>(pageInfo, "<title>(?<x>.*?)品种齐全|<div class=\"guide_title\"><span title=\"(?<x>.*?)\">");
            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";
            int total = RegGroupsX<int>(pageInfo, "共(?<x>\\d+?)条");

            SiteClassInfo catInfo = new SiteClassInfo();
       
            for (int i = 0; i < list.Count; i++)
            {
                string div = list[i].Groups["x"].Value;
                catInfo.Urlinfo = RegGroupsX<string>(div, "href=\"(?<x>.*?)\"");
                catInfo.ClassId = RegGroupsX<string>(catInfo.Urlinfo, "http://list.yhd.com/(?<x>.*?)/");
                if(string.IsNullOrEmpty(catInfo.Urlinfo))
                    continue;

                if (!catInfo.Urlinfo.Contains("b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/"))
                    catInfo.Urlinfo += "b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/";



                catInfo.ClassName = RegGroupsX<string>(div, "\" >(?<x>.*?)</a>|title=\"(?<x>.*?)\"");
                if (catInfo.ClassName== "全部结果")
                    continue;
              
                if (catInfo.Urlinfo == null || catInfo.ClassId == null || catInfo.ClassName == null)
                {
                    LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1", "AddClassError");
                    continue;
                }
                if (!HasBindClasslist.Exists(p => p.ClassId == catInfo.ClassId) && !HasBindClasslist.Exists(p => p.ClassName == catInfo.ClassName))
                {
                    GetYhdClassInfo(catInfo.Urlinfo);
                }
           
                catInfo.ParentClass = pcatId;
                catInfo.ParentName = pcatName;
                catInfo.ParentUrl = pcatUrl;
                if (i != 0 && pcatId != "")
                    classCrumble += pcatId + ",";
                catInfo.ClassCrumble = classCrumble.TrimEnd(',');

                if (list.Count - 1 == i)
                {
                    catInfo.HasChild = false;
                    catInfo.TotalProduct = total;
                }
          
                if (catInfo.ClassName == current)
                {
                    break;
                }
                pcatUrl = catInfo.Urlinfo;
                pcatName = catInfo.ClassName;
                pcatId = catInfo.ClassId;
            }
            if (!string.IsNullOrEmpty(pcatId))
            {
                if (HasBindClasslist.Any(p => p.ClassId == pcatId))
                {
                    GetYhdClassInfo(pcatUrl);
                }
            }


            string childCat = RegGroupsX<string>(pageInfo,"<div class=\"classWrap\">(?<x>.*?)</ul>");
            if (childCat != null)
            {
                var childList = RegGroupCollection(childCat, "href=\"(?<x>.*?)\".*?<span title=\"(?<y>.*?)\">");
                foreach (Match item in childList)
                {
                    string tempUrl = item.Groups["x"].Value;
                    string tempName = item.Groups["y"].Value;
                    string tempid = RegGroupsX<string>(tempUrl, "http://list.yhd.com/(?<x>.*?)/");
                    if (!HasBindClasslist.Exists(c => c.ClassId == tempid) && !HasBindClasslist.Exists(p => p.ClassName == tempName))
                    {
                        GetYhdClassInfo(tempUrl);
                    }
                }
                siteClassInfo.HasChild = true;
            }
            else
            { siteClassInfo.HasChild = false; }


            if (siteClassInfo.ClassId != catInfo.ClassId)
            {

                LogServer.WriteLog("分类id 更改 old id:" + siteClassInfo.Id + siteClassInfo.Id + "oldclass:" + siteClassInfo.ClassId + "newclass:" + catInfo.ClassId);
                if (HasBindClasslist.Exists(c => c.ClassId == catInfo.ClassId))
                {
                    new SiteClassBll().delClass(siteClassInfo);
                    return;
                }

                siteClassInfo.Urlinfo = catInfo.Urlinfo;
                siteClassInfo.ClassId = catInfo.ClassId;
            
     
            }


            if (tempcurcatid != siteClassInfo.ClassId && regIsMatch(tempcurcatid, "^(?<x>c\\d+-\\d+(-\\d+)?)$"))
            {
                siteClassInfo.ClassId = tempcurcatid;
                siteClassInfo.Urlinfo =$"http://list.yhd.com/{tempcurcatid}/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k";
            }

            siteClassInfo.ClassName = catInfo.ClassName;
            siteClassInfo.TotalProduct = catInfo.TotalProduct;
            siteClassInfo.ParentUrl = catInfo.ParentUrl;
            siteClassInfo.ParentClass = catInfo.ParentClass;
            if (regIsMatch(siteClassInfo.ParentClass, "c\\d+-0"))
            {
                string parentpage = Gethtmlcode(catInfo.ParentUrl);
                string pcategoryname = RegGroupsX<string>(parentpage, "var categoryName = '(?<x>.*?)'");
                string pextid = RegGroupsX<string>(parentpage, "var expectCategoryId = \"(?<x>\\d+)\"");
                string ptempcurcatid = "c" + pextid + "-" + pcategoryname;
                if (regIsMatch(ptempcurcatid, "^(?<x>c\\d+-\\d+(-\\d+)?)$"))
                {

                    siteClassInfo.ParentClass = ptempcurcatid;
                    siteClassInfo.ParentUrl =
                        $"http://list.yhd.com/{ptempcurcatid}/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k";
                }
            }
            siteClassInfo.HasChild = HasBindClasslist.Exists(p => p.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentName = catInfo.ParentName;
  
            siteClassInfo.UpdateTime = DateTime.Now;
            new SiteClassBll().UpdateSiteCat(siteClassInfo);

        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }


}
