using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;


namespace BLL.Sprider.classInfo
{
    /// <summary>
    /// 老板电器分类管理
    /// </summary>
    public class ShopRobamClassInfo : ShopRobam, ISiteClassBLL
    {
       public ShopRobamClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(196);
           if (Baseinfo == null)
               Baseinfo = new SiteInfo {SiteId = 196};
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.shoprobam.com/");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"hover\" id=\"categorys\">.*?<div class=\"item fore1\">(?<x>.*?)<ul class=\"nav\">");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\" (class=\"sub\" )?target=\"_blank\">(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "scId=(?<x>\\d+)$");
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
                        Urlinfo = string.Format("http://www.shoprobam.com{0}", tempUrl),
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
            string cromb = RegGroupsX<string>(page, "<div class=\"path\">(?<x>.*?)</div>");
            string catName = RegGroupsX<string>(cromb, "<a href=\"javascript:void\\(0\\);\">(?<x>.*?) - 产品列表");
            if (cromb == null)
                return;
            string ProdAllList = HtmlAnalysis.HttpRequestFromPost("http://www.shoprobam.com/scProdAllList.do", "scId="+siteClassInfo.ClassId, "utf-8", false);
            string catpage = HtmlAnalysis.Gethtmlcode("http://www.shoprobam.com/ajaxSCleft.do");
            var catArea = RegGroupCollection(catpage, "<div class=\"side_box\">(?<x>.*?)</div>");
            if (catArea != null)
            {
                foreach (Match item in catArea)
                {
                    string parentName = RegGroupsX<string>(item.ToString(), "<h3> <b></b>(?<x>.*?)</h3>");
                    if (siteClassInfo.ClassName == parentName)
                        siteClassInfo.HasChild = true;
                    var paraCat = HasBindClasslist.FindLast(c => c.ClassName == parentName);
                    var childList = RegGroupCollection(item.ToString(), "<a href=\"/list/scProdPage.do\\?scId=(?<x>.*?)\">(?<y>.*?)</a>");
                    if (childList != null)
                    {
                        foreach (Match citem in childList)
                        {
                          
                            string tempName = citem.Groups["y"].Value;
                            string tempId = citem.Groups["x"].Value;

                            if (tempId == siteClassInfo.ClassId && paraCat!=null)
                            {
                                siteClassInfo.ParentClass = paraCat.ClassId;
                                siteClassInfo.ParentName = paraCat.ClassName;
                                siteClassInfo.ParentUrl = paraCat.Urlinfo;
                            }
                            if (!HasBindClasslist.Exists(c => c.ClassId == tempId))
                            {
                                SiteClassInfo iteminfo = new SiteClassInfo
                                {
                                    ParentClass = "",
                                    ParentName = "",
                                    ClassName = tempName,
                                    ClassId = tempId,
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
                                    Urlinfo = string.Format("http://www.shoprobam.com/list/scProdPage.do\\?scId={0}", tempId),
                                    UpdateTime = DateTime.Now,
                                    CreateDate = DateTime.Now
                                };
                                HasBindClasslist.Add(iteminfo);
                                shopClasslist.Add(iteminfo);
                            }



                        }
 
                    }
                }
            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

            siteClassInfo.ClassName = catName;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(ProdAllList, "共(?<x>\\d+)条记录");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
