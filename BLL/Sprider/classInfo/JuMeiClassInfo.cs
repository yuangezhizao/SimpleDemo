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
    public class JuMeiClassInfo : JuMei, ISiteClassBLL
    {

        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

     
        public JuMeiClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(52);
        }
        public void SaveAllSiteClass()
        {
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string popHtml = HtmlAnalysis.Gethtmlcode("http://search.jumei.com/?filter=0-31-1");

            string catArea = RegGroupsX<string>(popHtml,
                "<div class=\"filter_attrs\" id=\"filter_cat\">(?<x>.*?)<label class=\"filter_tit_wide\">已选分类");

            var catList = RegGroupCollection(catArea, "cat=(?<x>\\d+)\">(?<y>.*?)</a>");
            if (catList == null)
                return;
            foreach (Match item in catList)
            {
                string tempid = item.Groups["x"].Value;
                string tempName = WordCenter.FilterHtml(item.Groups["y"].Value);

                if (!ValidCatId(tempid) || HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    continue;
                }


                SiteClassInfo catInfo = new SiteClassInfo
                {
                    ClassName = tempName,
                    ClassId = tempid,
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "名品特卖",
                    ClassCrumble = "",
                    IsDel = false,

                    ParentClass = "",
                    Urlinfo = "http://pop.jumei.com/list?filter=0-11-1&cat=" + tempid,

                    HasChild = true,
                    IsBind = false
                };
                HasBindClasslist.Add(catInfo);
                shopClasslist.Add(catInfo);
  
            }


            string searchHtml = HtmlAnalysis.Gethtmlcode("http://search.jumei.com/");

            catArea = RegGroupsX<string>(searchHtml,
              "id=\"filter_cat\">(?<x>.*?)<label class=\"filter_tit_wide\">已选分类");

             catList = RegGroupCollection(catArea, "cat=(?<x>\\d+)\">(?<y>.*?)</a>");
            if (catList == null)
                return;

            foreach (Match item in catList)
            {
                string tempid = item.Groups["x"].Value;
                string tempName = WordCenter.FilterHtml(item.Groups["y"].Value);

                if (!ValidCatId(tempid) || HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    continue;
                }


                SiteClassInfo catInfo = new SiteClassInfo
                {
                    ClassName = tempName,
                    ClassId = tempid,
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "化妆品",
                    ClassCrumble = "",
                    IsDel = false,

                    ParentClass = "",
                    Urlinfo = "http://search.jumei.com/?filter=0-11-1&search=&cat=" + tempid,

                    HasChild = true,
                    IsBind = false
                };
                HasBindClasslist.Add(catInfo);
                shopClasslist.Add(catInfo);

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
                    if (HasBindClasslist[i].UpdateTime.ToString("yyyy-MM-dd") != "2000-01-01")
                        UpdateNode(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }

        private void UpdateNode(SiteClassInfo item)
        {
            string catPage = HtmlAnalysis.Gethtmlcode(item.Urlinfo);
            if (catPage.Contains("抱歉，没有找到相关的产品"))
                return;
            string crumb = RegGroupsX<string>(catPage, "<\\!-- 面包屑 start -->(?<x>.*?)<\\!-- 面包屑 end -->|<div class=\"selected_panel\">(?<x>.*?)<div class=\"hover_mask\">");
            if (crumb == null)
                return;
            var crumblist = RegGroupCollection(crumb, "<a href=\"(?<x>.*?)\"( target=\"_blank\")?>(?<y>.*?)</a>");
            string parenturl = "";
            string parentName = "";
            string parentid = "";

            foreach (Match catinfo in crumblist)
            {
                if (catinfo.ToString().Contains("聚美优品") || catinfo.ToString().Contains(item.ClassName))
                    continue;
                if (catinfo.ToString().Contains("化妆品首页"))
                {
                    parentName = "化妆品";
                    parentid = "hzp";
                }
                else
                {
                    parenturl = catinfo.Groups["x"].Value;
                    parentName = catinfo.Groups["y"].Value;
                    parentid = RegGroupsX<string>(parenturl, "cat=(?<x>\\d+)") ?? "";
                    if (parentName == "名品特卖")
                        parentid = "mptm";
                }
            }


        
            int proTotal = RegGroupsX<int>(catPage, "共<span>(?<x>.*?)</span>个商品");

            string catName = RegGroupsX<string>(catPage, "<div class=\"filter_choosed_item\">分类：<span>(?<x>.*?)<a");
            catName = WordCenter.FilterHtml(catName);
       
            string childCatArea = RegGroupsX<string>(catPage, "<ul class=\"pop_list\">(?<x>.*?)</ul>");

            var childList = RegGroupCollection(childCatArea, "href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (proTotal > 0)
            {
                if (ValidCatId(parentid))
                    item.ParentClass = parentid;
                if (string.IsNullOrEmpty(parentName))
                    item.ParentName = parentName;
                item.ParentUrl = parenturl;
                item.TotalProduct = proTotal;
                item.UpdateTime = DateTime.Now;
                if (string.IsNullOrEmpty(catName))
                item.ClassName = catName;
                if (childList == null || childList.Count < 2)
                    item.HasChild = false;
                else
                    item.HasChild = true;
                 new SiteClassBll().UpdateSiteCat(item);
            }
      
            if (childList == null)
                return;
            foreach (Match catinfo in childList)
            {
                string tempurl = catinfo.Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempurl, "cat=(?<x>\\d+)");
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    string tempName = catinfo.Groups["y"].Value;
                    if (!string.IsNullOrEmpty(tempName))
                        tempName = WordCenter.FilterHtml(tempName);

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
                        HasChild = true,
                        ClassCrumble = "",
                        TotalProduct = 0,
                        SiteId = Baseinfo.SiteId,
                        Urlinfo = tempurl,
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
           
        }
    }
}
