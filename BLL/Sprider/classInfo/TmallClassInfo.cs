using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Commons;
using DataBase;
using DataBase.Mongo;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class TmallClassInfo : TMall, ISiteClassBLL
    {
        //http://list.tmall.com//search_product.htm?brand=11016
        //http://brand.tmall.com/brandInfo.htm?brandId=30641
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        private List<SiteBandInfo> HasSiteBandlist { get; set; }

        private List<SiteClassBand> HasBindBandlist { get; set; }

        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        public TmallClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(10);
        }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string url = "http://3c.tmall.com/"; //电器
          
            for (int i = 1; i < 16; i++)
            {
                url = string.Format("http://www.tmall.com/tmall-fp/3.0/mods/load.php?path=tmall-fp%2F3-0%2Fcategory-{0}.php",i);
                Thread.Sleep(new Random().Next(10, 60) * 1000);
                string directoryHtml = HtmlAnalysis.Gethtmlcode(url);

                var catList = RegGroupCollection(directoryHtml, "cat=(?<x>\\d+)");
                if (catList == null)
                    return;
                foreach (Match item in catList)
                {
                    SaveCat(item.Groups["x"].Value);
                }
                if (shopClasslist.Count > 0)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
                }
            }
           
        }

        private void SaveCat(string catId)
        {
            if (!ValidCatId(catId)) return;
            Thread.Sleep(new Random().Next(10, 60) * 1000);
            string catPage = HtmlAnalysis.Gethtmlcode("http://list.tmall.com/search_product.htm?cat=" + catId);

            var crumbsList = RegGroupCollection(catPage,
                "<li data-tag=\"cat\">(?<x>.*?)</li>");
            if (crumbsList == null)
                return;
            SiteClassInfo catinfo = new SiteClassInfo();

            string paraInfo = "";
            string paraUrl = "";
            string paraCatId = "";
            string paraName = "";
            for (int i = 0; i < crumbsList.Count; i++)
            {
                catinfo.ParentName = paraName;
                catinfo.ParentClass = paraCatId;
                catinfo.ParentUrl  = paraCatId=="" ?"": "http://list.tmall.com/search_product.htm?cat=" + paraCatId;

                //添加父类
                if (ValidCatId(paraCatId) &&!HasBindClasslist.Exists(c => c.ClassId == paraCatId))
                {

                    string tempparaInfo = "";
                    string tempparaUrl = "";
                    string tempparaCatId = "";
                    string tempparaName = "";
                    if (i > 1)
                    {
                        Match pnode = crumbsList[i - 2];
                        tempparaInfo = pnode.Groups["x"].Value;
                        tempparaUrl = RegGroupsX<string>(tempparaInfo, "href=\"(?<x>.*?)\"");
                        tempparaCatId = RegGroupsX<string>(paraUrl, "cat=(?<x>\\d+)");
                        tempparaUrl = "http://list.tmall.com/search_product.htm?cat=" + tempparaCatId;
                        tempparaName = RegGroupsX<string>(tempparaInfo, "title=\"(?<x>.*?)\"");
                    }

                    SiteClassInfo catPareInfo = new SiteClassInfo
                    {
                        ClassName = paraName,
                        ClassId = paraCatId,
                        SiteId = Baseinfo.SiteId,
                        CreateDate = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsHide = false,
                        ParentUrl = tempparaUrl,
                        ParentName = tempparaName,
                        ClassCrumble = tempparaCatId + ",",
                        IsDel=false,
                        
                        ParentClass = tempparaCatId,
                        Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + paraCatId,

                        HasChild = true,
                        IsBind = false
                    };
                    catPareInfo.ClassCrumble = catPareInfo.ClassCrumble.TrimEnd(',');
                    HasBindClasslist.Add(catPareInfo);
                    shopClasslist.Add(catPareInfo);
                    //父类的同级分类
                    GetAllBrotherCats(catPareInfo);
                    //子类
                    GetChildCats(catPareInfo, "");

                }

                Match node = crumbsList[i];
                paraInfo = node.Groups["x"].Value;
                paraUrl = RegGroupsX<string>(paraInfo, "href=\"(?<x>.*?)\"");
                paraCatId = RegGroupsX<string>(paraUrl, "cat=(?<x>\\d+)");
                paraName = RegGroupsX<string>(paraInfo, "title=\"(?<x>.*?)\"");

                catinfo.ClassName = paraName;
                catinfo.ClassId = paraCatId;
                catinfo.SiteId = Baseinfo.SiteId;
                catinfo.CreateDate = DateTime.Now;
                catinfo.UpdateTime = DateTime.Now;
                catinfo.Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + paraCatId;
                catinfo.TotalProduct = RegGroupsX<int>(catPage, "共<span> (?<x>\\d+)</span>件相关商品");
                catinfo.IsHide = false;
                catinfo.IsBind = false;
                catinfo.ClassCrumble += paraCatId + ",";
                catinfo.IsDel = false;
                GetAllBrotherCats(catinfo);

            }
            GetChildCats(catinfo, catPage);
            catinfo.ClassCrumble = catinfo.ClassCrumble.TrimEnd(',');
            if (catinfo.ClassId != "" && !HasBindClasslist.Exists(c => c.ClassId == catinfo.ClassId))
            {
                catinfo.HasChild = true;
                HasBindClasslist.Add(catinfo);
                shopClasslist.Add(catinfo);
            }
            else if (catinfo.ClassName != "" &&
                     !HasBindClasslist.Exists(c => c.ClassName == catinfo.ClassName))
            {
                catinfo.HasChild = true;
                HasBindClasslist.Add(catinfo);
                shopClasslist.Add(catinfo);
            }


            if (shopClasslist.Count > 100)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
        }
        /// <summary>
        /// 获取同父级的分类
        /// </summary>
        /// <param name="catinfo"></param>
        private void GetAllBrotherCats(SiteClassInfo catinfo)
        {
           // Thread.Sleep(new Random().Next(5, 30) * 1000);
            string catPage = HtmlAnalysis.Gethtmlcode("http://list.tmall.com/ajax/getAllBrotherCats.htm?cat=" + catinfo.ClassId);
            var catList = RegGroupCollection(catPage, "\"href\":\"(?<x>.*?)\",\r\n\"title\":\"(?<y>.*?)\",\r\n\"atp\"");
               if (catList == null)
                return;
               foreach (Match item in catList)
               {
                   string catUrl = item.Groups["x"].Value;
                   string catId = RegGroupsX<string>(catUrl, "cat=(?<x>\\d+)");
                   if (string.IsNullOrEmpty(catId))
                       continue;

                   if (!HasBindClasslist.Exists(c => c.ClassId == catId))
                   {
                       SiteClassInfo cat = new SiteClassInfo { 
                           ClassId =catId,
                           ClassCrumble = catinfo.ClassCrumble,
                           ParentClass = catinfo.ParentClass,
                           ParentName = catinfo.ParentName,
                           ClassName=item.Groups["y"].Value,
                           IsHide=false,
                           ParentUrl=catinfo.ParentUrl,
                           UpdateTime=DateTime.Now,
                           IsBind=false,
                           IsDel=false,
                           SiteId=Baseinfo.SiteId,
                           Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + catId,
                           CreateDate=DateTime.Now


                       };

                       HasBindClasslist.Add(cat);
                       shopClasslist.Add(cat);
                       
                   }
               }
        }

        /// <summary>
        /// 获取子分类
        /// </summary>
        /// <param name="catinfo"></param>
        private void GetChildCats(SiteClassInfo catinfo,string pageinfo)
        {

            if (pageinfo == "")
            {
                Thread.Sleep(new Random().Next(10, 30)*1000);
                pageinfo = HtmlAnalysis.Gethtmlcode(catinfo.Urlinfo);
            }
            string catInfo = RegGroupsX<string>(pageinfo, "<div class=\"cateAttrs\" data-spm=\".*?\">(?<x>.*?)<div class=\"propAttrs\"");

            if (catInfo == null)
                return;
            var catList = RegGroupCollection(catInfo, "<a title=\"(?<x>.*?)\">\r\n <b>(?<y>.*?)</b><span>\\((?<z>\\d+)\\)</span>\r\n </a>");
               if (catList == null)
                return;
               foreach (Match item in catList)
               {
                   string catUrl = item.Groups["x"].Value;
                   string catId = RegGroupsX<string>(catUrl, "cat=(?<x>\\d+)");
                   if (string.IsNullOrEmpty(catId))
                       continue;
                   int total = 0;
                   int.TryParse(item.Groups["z"].Value, out total);
                   if (!HasBindClasslist.Exists(c => c.ClassId == catId))
                   {
                       SiteClassInfo cat = new SiteClassInfo { 
                           ClassId =catId,
                           ClassCrumble = catinfo.ClassCrumble+","+catinfo.ClassId,
                           ParentClass = catinfo.ClassId,
                           ParentName = catinfo.ClassName,
                           ClassName=item.Groups["y"].Value,
                           IsHide=false,
                           ParentUrl=catinfo.Urlinfo,
                           UpdateTime=DateTime.Now,
                           IsBind=false,
                           IsDel=false,
                           SiteId=Baseinfo.SiteId,
                           Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + catId,
                           TotalProduct=total,
                           CreateDate=DateTime.Now


                       };

                       HasBindClasslist.Add(cat);
                       shopClasslist.Add(cat);
                       
                   }
               }
        }

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
         
                    UpdateTmallNode(HasBindClasslist[i]);
                    //Thread.Sleep(1000*10);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }

        }

        private void UpdateTmallNode(SiteClassInfo item)
        {
            Thread.Sleep(new Random().Next(6, 30)*1000);
            string catPage = HtmlAnalysis.Gethtmlcode("http://list.tmall.com/search_product.htm?cat=" + item.ClassId);
            if (catPage.Contains("ResponseUri:http://www.tmall.com/") && item.UpdateTime.AddDays(15)<DateTime.Now)
            {
                new SiteClassBll().delClass(item);
                return;
            }
            var crumbsList = RegGroupCollection(catPage,
                "<li data-tag=\"cat\">(?<x>.*?)</li>");
            if (crumbsList == null)
                return;
            SiteClassInfo catinfo = new SiteClassInfo();

            string paraInfo = "";
            string paraUrl = "";
            string paraCatId = "";
            string paraName = "";
            for (int i = 0; i < crumbsList.Count; i++)
            {
                catinfo.ParentName = paraName;
                catinfo.ParentClass = paraCatId;
                catinfo.ParentUrl = paraCatId == "" ? "" : "http://list.tmall.com/search_product.htm?cat=" + paraCatId;

                //添加父类
                if (paraCatId != "" && paraCatId != "" &&
                    !HasBindClasslist.Exists(c => c.ClassId == paraCatId))
                {

                    string tempparaInfo = "";
                    string tempparaUrl = "";
                    string tempparaCatId = "";
                    string tempparaName = "";
                    if (i > 1)
                    {
                        Match pnode = crumbsList[i - 2];
                        tempparaInfo = pnode.Groups["x"].Value;
                    //  tempparaUrl = RegGroupsX<string>(tempparaInfo, "href=\"(?<x>.*?)\"");
                        tempparaCatId = RegGroupsX<string>(paraUrl, "cat=(?<x>\\d+)");
                        tempparaUrl = "http://list.tmall.com/search_product.htm?cat=" + tempparaCatId;
                        tempparaName = RegGroupsX<string>(tempparaInfo, "title=\"(?<x>.*?)\"");
                    }

                    SiteClassInfo catPareInfo = new SiteClassInfo
                    {
                        ClassName = paraName,
                        ClassId = paraCatId,
                        SiteId = Baseinfo.SiteId,
                        CreateDate = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsHide = false,
                        ParentUrl = tempparaUrl,
                        ParentName = tempparaName,
                        ClassCrumble = tempparaCatId + ",",

                        ParentClass = tempparaCatId,
                        Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + paraCatId,
                        IsDel=false,
                        HasChild = true,
                        IsBind = false
                    };
                    catPareInfo.ClassCrumble = catPareInfo.ClassCrumble.TrimEnd(',');
                    HasBindClasslist.Add(catPareInfo);
                    shopClasslist.Add(catPareInfo);
                    //父类的同级分类
                    GetAllBrotherCats(catPareInfo);
                    //子类
                    GetChildCats(catPareInfo, catPage);

                }

                Match node = crumbsList[i];
                paraInfo = node.Groups["x"].Value;
                paraUrl = RegGroupsX<string>(paraInfo, "href=\"(?<x>.*?)\"");
                paraCatId = RegGroupsX<string>(paraUrl, "cat=(?<x>\\d+)");
                paraName = RegGroupsX<string>(paraInfo, "title=\"(?<x>.*?)\"");

                catinfo.ClassName = paraName;
                catinfo.ClassId = paraCatId;
                catinfo.SiteId = Baseinfo.SiteId;
                catinfo.CreateDate = DateTime.Now;
                catinfo.UpdateTime = DateTime.Now;
                catinfo.Urlinfo = "http://list.tmall.com/search_product.htm?cat=" + paraCatId;
                catinfo.TotalProduct = RegGroupsX<int>(catPage, "共<span> (?<x>\\d+)</span>件相关商品");
                catinfo.IsHide = false;
                catinfo.IsBind = false;
                catinfo.IsDel = false;
                catinfo.ClassCrumble += paraCatId + ",";

                GetAllBrotherCats(catinfo);

            }
            if (string.IsNullOrEmpty(catinfo.ClassId))
                return;
            if(regIsMatch(catPage, "<div class=\"cateAttrs\" data-spm=\".*?\">(?<x>.*?)<div class=\"propAttrs\""))
            {
                GetChildCats(catinfo, catPage);
                catinfo.HasChild = true;
            }
            else
                catinfo.HasChild = false;
            catinfo.ClassCrumble = catinfo.ClassCrumble.TrimEnd(',');

            var oldCatInfo = HasBindClasslist.Find(c => c.ClassId == catinfo.ClassId);
            if (oldCatInfo==null)
            {
                catinfo.HasChild = true;
                HasBindClasslist.Add(catinfo);
                shopClasslist.Add(catinfo);
            }
            else
            {
                oldCatInfo.Urlinfo = catinfo.Urlinfo;
                oldCatInfo.ClassId = catinfo.ClassId;
                oldCatInfo.ClassName = catinfo.ClassName;
                oldCatInfo.TotalProduct = catinfo.TotalProduct;
                oldCatInfo.ParentUrl = catinfo.ParentUrl;
                oldCatInfo.ParentClass = catinfo.ParentClass;
                oldCatInfo.ParentUrl = catinfo.ParentUrl;
                oldCatInfo.UpdateTime = DateTime.Now;
                new SiteClassBll().UpdateSiteCat(oldCatInfo);
            }
          


            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }
        }


        public void LoadBand()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();
            HasBindBandlist = new SiteClassBandDb().FindBySiteId(Baseinfo.SiteId);
            HasSiteBandlist = new SiteBandDb().FindBySiteId(Baseinfo.SiteId);
            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                if (string.IsNullOrEmpty(HasBindClasslist[i].ClassId))
                    continue;
                SaveBand(HasBindClasslist[i]);
                Thread.Sleep(1000 * 10);
            }
        }

        private void SaveBand(SiteClassInfo siteClassInfo)
        {

            string brandUrl = string.Format("http://list.tmall.com/ajax/allBrandShowForGaiBan.htm?cat={0}", siteClassInfo.ClassId);

            string page = HtmlAnalysis.Gethtmlcode(brandUrl);
            if (string.IsNullOrEmpty(page))
                return;
            page = page.Replace("\r", "").Replace("\n", "").Trim();
            if (page == "")
                return;


            var list = RegGroupCollection(page, "\\{(?<x>.*?)\\}");
            if (list == null)
                return;
            List<SiteClassBand> catBands = new List<SiteClassBand>();
            List<SiteBandInfo> siteBand = new List<SiteBandInfo>();
           
            for(int i=0;i<list.Count;i++)
            {
                string templist = list[i].ToString();
                string url = RegGroupsX<string>(templist, "\"href\":\"(?<x>.*?)\"");
                if (string.IsNullOrEmpty(url))
                    continue;
                url = url.Replace("&amp;", "&");
                if (!url.Contains("http://"))
                {
                    url = "http://list.tmall.com/search_product.htm" + url;
                }
                string bandId = RegGroupsX<string>(templist, "brand=(?<x>\\d+)");
                string disName = RegGroupsX<string>(templist, "\"title\":\"(?<x>.*?)\"");
                string img = RegGroupsX<string>(templist, "\"img\":\"(?<x>.*?)\"");
                string cnName = "";
                string enName = "";
                string key = Baseinfo.SiteId + "_" + siteClassInfo.ClassId + "_" + bandId;
                string key1 = Baseinfo.SiteId + "_" + bandId;
                string[] names = disName.Split('/');
                foreach (string obj in names)
                {
                    if (regIsMatch(obj, @"[\u4e00-\u9fa5]"))
                        cnName = obj;
                    else
                        enName = obj;
                }

                SiteClassBand tempBand = new SiteClassBand
                {
                    ImgUrl=img,
                    UniqueKey = key,
                    DisplayName = disName,
                    CnName=cnName,
                    EnName = enName,
                    CommentCount=0,
                    ProductCount=0,
                    Urlinfo=url,
                    SiteBandId = bandId,
                    SiteClassId= siteClassInfo.ClassId,
                    SiteId=Baseinfo.SiteId,
                    IsHid=false,
                    UpdateDate=DateTime.Now,
                    CreateDate =DateTime.Now
                };
                if (!HasBindBandlist.Exists(p => p.UniqueKey == key))
                {
                    HasBindBandlist.Add(tempBand);
                    catBands.Add(tempBand);
                }
                if (!HasSiteBandlist.Exists(p => p.UniqueKey == key1))
                {
                    SiteBandInfo tempsband = new SiteBandInfo
                    {
                        CatArea="",
                        EnName=tempBand.EnName,
                        ImgUrl=tempBand.ImgUrl,
                        Introduction="",
                        IsHid=false,
                        Remark="",
                        TotalComments=0,
                        TotalProduts=0,
                        UniqueKey = key1,
                        SiteId=tempBand.SiteId,
                        SiteBandId=tempBand.SiteBandId,
                        DisplayName=tempBand.DisplayName,
                        CnName=tempBand.CnName,
                        CreateDate=DateTime.Now,
                        UpdateDate=DateTime.Now
                    };

                    HasSiteBandlist.Add(tempsband);
                    siteBand.Add(tempsband);
                }

            }
            try
            {
                new SiteClassBandDb().Save(catBands);
                new SiteBandDb().Save(siteBand);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }




        }
    }
}
