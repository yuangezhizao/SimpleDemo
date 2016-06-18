using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class DangdangClassInfo : DangDang, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public DangdangClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(3);
        }
        
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://category.dangdang.com/");

            string tempPage = RegGroupsX<string>(directoryHtml, "<div  class=\"col col_2\" name=5596>(?<x>.*?)<style></style><script type=\"text/javascript\"");
            var list = RegGroupCollection(tempPage, "href=\"(?<x>.*?)\"|href='(?<x>.*?)'");
            for (int i = 0; i < list.Count; i++)
            {
                string tempurl = list[i].Groups["x"].Value;

                if(regIsMatch(tempurl,"^http://category.dangdang.com/cid\\d+.html$"))
                    AddNode(tempurl);
                if (shopClasslist.Count > 0)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
                }
            }


        }

        private void AddNode(string tempurl)
        {
            string tempid = RegGroupsX<string>(tempurl, "^http://category.dangdang.com/cid(?<x>\\d+).html$");
            if (HasBindClasslist.Exists(p => p.ClassId == tempid))
                return;
            string pageInfo = HtmlAnalysis.Gethtmlcode(tempurl);
            if ( pageInfo.Contains("分类发生了变化"))
                return;
            string cromp = RegGroupsX<string>(pageInfo, "<div class=\"breadcrumb\"( id=\"B\")?>(?<x>.*?)</div>|<div class=\"crumbs_fb_left\">(?<x>.*?)<script>");
            string catName = RegGroupsX<string>(pageInfo, "<span class=\"current\">(?<x>.*?)</span>");
          
            int total = RegGroupsX<int>(pageInfo, "<span>共(?<x>.*?)个商品</span>|共<span>(?<x>.*?)</span>个商品|共<span class=\"or\">(?<x>\\d+)</span>件商品");
            var parentList = RegGroupCollection(cromp, "<a(?<x>.*?)<span>|<a class=\"a\" title=\".*?\"(?<x>.*?)</a>");
            string parentName = "";
            string parentUrl = "";
            string parentId = "";
            string catcromp = "";
            List<string> newCat = new List<string>();
            if (parentList != null && parentList.Count>1)
            {
            
                if (string.IsNullOrEmpty(catName))
                {
                    catName = RegGroupsX<string>(parentList[parentList.Count - 1].Value, ">(?<x>.*?)</a>");
         
                }
                for (int i = 1; i < parentList.Count; i++)
                {
                    
                    string pro = parentList[i].Value;
                    if (RegGroupsX<string>(pro, ">(?<x>.*?)</a>") == catName)
                        break;

                    parentUrl = RegGroupsX<string>(pro, "href=\"(?<x>.*?)\"");
                    if (!parentUrl.Contains("http://"))
                        parentUrl = "http://category.dangdang.com" + parentUrl;

                    parentName = RegGroupsX<string>(pro, ">(?<x>.*?)</a>");
                    parentId = RegGroupsX<string>(parentUrl, "cid(?<x>\\d+).html");
                    if (!HasBindClasslist.Exists(p => p.ClassId == parentId))
                    {
                        newCat.Add(parentUrl);
                    }
                    if (parentId != null)
                        catcromp += parentId + ",";
                }
          
                
            }
            catcromp = catcromp.TrimEnd(',');

            if (string.IsNullOrEmpty(cromp))
                return;

            SiteClassInfo cat = new SiteClassInfo { 
            ClassId=tempid,
            ClassCrumble=catcromp,
            HasChild=false,
            ClassName=catName,
            CreateDate=DateTime.Now,
            UpdateTime=DateTime.Now,
            SiteId=Baseinfo.SiteId,
            IsDel=false,
            IsBind=false,
            IsHide=false,
            BindClassId=0,
            BindClassName="",
            ParentClass=parentId,
            ParentName=parentName,
            ParentUrl=parentUrl,
            Urlinfo=tempurl,
            TotalProduct = total
            };
            HasBindClasslist.Add(cat);
            shopClasslist.Add(cat);
            foreach (var item in newCat)
            {
                AddNode(item);
            }

            string catHtml = RegGroupsX<string>(pageInfo, "<div class=\"sort_box\" name=\"\\w*\">(?<x>.*?)</ul>");
            if (string.IsNullOrEmpty(catHtml))
                return;
            var list = RegGroupCollection(catHtml, "href=\"/cid(?<x>\\d+).html#");

            for (int i = 0; i < list.Count; i++)
            {
                string catId = list[i].Groups["x"].Value;
                string catUrl = string.Format("http://category.dangdang.com/cid{0}.html", catId);
                if (!HasBindClasslist.Exists(p => p.ClassId == catId))
                {
                    AddNode(catUrl);
                }

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
            string tempurl = string.Format("http://category.dangdang.com/cid{0}.html", siteClassInfo.ClassId);

            string pageInfo = HtmlAnalysis.Gethtmlcode(tempurl);



            if (pageInfo.Contains("<span>没有找到相关的商品</span>") || pageInfo.Contains("分类发生了变化"))
            {
                new SiteClassBll().delClass(siteClassInfo);
                return;
            }
            string cromp = RegGroupsX<string>(pageInfo, "<div class=\"breadcrumb\"( id=\"B\")?>(?<x>.*?)</div>|<div class=\"crumbs_fb_left\">(?<x>.*?)<script>");
            //if (string.IsNullOrEmpty(cromp))
            //    return;
            cromp = cromp.Replace("<span class=\"current\"></span>", "");
            string catName = RegGroupsX<string>(cromp, "<span class=\"current\">(?<x>.*?)</span>");
            //if (string.IsNullOrEmpty(catName))
            //    return;
            int total = RegGroupsX<int>(pageInfo, "<span>共(?<x>.*?)个商品</span>|共<span>(?<x>.*?)</span>个商品|共<span class=\"or\">(?<x>\\d+)</span>件商品");
            var parentList = RegGroupCollection(cromp, "<a(?<x>.*?)<span>|<a class=\"a\" title=\".*?\"(?<x>.*?)</a>");
            string parentName = "";
            string parentUrl = "";
            string parentId = "";
            string catcromp = "";
            List<string> newCat = new List<string>();
            if (parentList != null && parentList.Count > 0)
            {
                  if (string.IsNullOrEmpty(catName))
                {
                    catName = RegGroupsX<string>(parentList[parentList.Count - 1].Value, ">(?<x>.*?)</a>");
                }
                for (int i = 1; i < parentList.Count; i++)
                {
                    string pro = parentList[i].Value;

                    if (RegGroupsX<string>(pro, ">(?<x>.*?)</a>") == catName)
                        break;
                    parentUrl = RegGroupsX<string>(pro, "href=\"(?<x>.*?)\"");
                    if (!parentUrl.Contains("http://"))
                        parentUrl = "http://category.dangdang.com" + parentUrl;

                    parentName = RegGroupsX<string>(pro, ">(?<x>.*?)</a>");
                    parentId = RegGroupsX<string>(parentUrl, "cid(?<x>\\d+).html");
                    if (!HasBindClasslist.Exists(p => p.ClassId == parentId))
                    {
                        newCat.Add(parentUrl);
                    }
                    if (parentId != null)
                        catcromp += parentId + ",";

                }
              
            }
            catcromp = catcromp.TrimEnd(',');

            if (string.IsNullOrEmpty(cromp))
                return;

            siteClassInfo.ClassName = catName;
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.Urlinfo = tempurl;
            siteClassInfo.TotalProduct = total;
            siteClassInfo.ClassCrumble = catcromp;
            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.UpdateTime = DateTime.Now;
            new SiteClassBll().UpdateSiteCat(siteClassInfo);
            foreach (var item in newCat)
            {
                AddNode(item);
            }

            string catHtml = RegGroupsX<string>(pageInfo, "<div class=\"sort_box\" name=\"\\w*\">(?<x>.*?)</ul>");
            if (string.IsNullOrEmpty(catHtml))
                return;
            var list = RegGroupCollection(catHtml, "href=\"/cid(?<x>\\d+).html#");

            for (int i = 0; i < list.Count; i++)
            {
                string catId = list[i].Groups["x"].Value;
                string catUrl = string.Format("http://category.dangdang.com/cid{0}.html", catId);
                if (!HasBindClasslist.Exists(p => p.ClassId == catId))
                {
                    AddNode(catUrl);
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
