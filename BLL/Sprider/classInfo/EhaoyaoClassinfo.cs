using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class EhaoyaoClassinfo : Ehaoyao, ISiteClassBLL
    {
        public EhaoyaoClassinfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(264);
        }
        private string domain = "http://www.ehaoyao.com/";
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.ehaoyao.com/");

            GetCatInfo(directoryHtml);
        }


        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div id=\"category_common\" class=\"category-common\">(?<x>.*?)</ul>");
            if (catArea == null)
                return;
        

            var list = RegGroupCollection(catArea, "<a   target=\"_blank\" href=\"(?<x>.*?)\" >(?<y>.*?)<i>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
           
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid = RegGroupsX<string>(tempUrl, "/products/c\\d+-s(?<x>\\d+).html");
                if(!ValidCatId(tempid))
                    continue;
                string parentid = RegGroupsX<string>(tempUrl, "c(?<x>\\d+)");
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = parentid,
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
                        Urlinfo = tempUrl,
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
            string cromb = RegGroupsX<string>(page, "<div class=\"breadCrumb\">(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a(?<x>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
                string itemcon = item.Groups["x"].Value;
                if(itemcon.Contains("首页")  )
                    continue;
               string tempId = RegGroupsX<string>(itemcon, "-s?(?<x>\\d+).html");// item.Groups["x"].Value;
                                                                             //if (!ValidCatId(tempId))
                                                                             //     continue;
                if (tempId == siteClassInfo.ClassId || itemcon.Contains("清空筛选条件") || itemcon.Contains("<h1>"))
                    break;
                parentId = tempId;
                parentName = RegGroupsX<string>(itemcon, ">(?<x>.*?)$");// item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }

                parentUrl = RegGroupsX<string>(itemcon, "href=\"(?<x>.*?)\"");
                if (parentUrl.IndexOf("http://www.ehaoyao.com") == -1)
                {
                    parentUrl = "http://www.ehaoyao.com" + parentUrl;
                }
                if (!HasBindClasslist.Exists(c => c.ClassId == parentId))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = parentName,
                        ClassId = parentId,
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
                        Urlinfo = parentUrl,
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }

            }
            
            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "共<span class=\"font_green count\">(?<x>\\d+)</span>件商品");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

            string childrenCat = RegGroupsX<string>(page, "<ul id=\"sort\">(?<x>.*?)</ul>");

            var childrenCats = RegGroupCollection(childrenCat, "<a title=\"(?<x>.*?)\"\\s*href=\"(?<y>.*?)\"");
            if(childrenCats==null||childrenCats.Count==0)
                return;
            foreach (Match cat in childrenCats)
            {
                string cName = cat.Groups["x"].Value;
                string cUrl = cat.Groups["y"].Value;
                if(string.IsNullOrEmpty(cName)||string.IsNullOrEmpty(cUrl))
                    continue;
                if (cUrl.IndexOf("http://www.ehaoyao.com") == -1)
                {
                    cUrl = "http://www.ehaoyao.com" + cUrl;
                }
                string cCatid= RegGroupsX<string>(cUrl, "-s(?<x>\\d+).html");
                string pCatid = RegGroupsX<string>(cUrl, "/products/c(?<x>\\d+)");

                if (!HasBindClasslist.Exists(c => c.ClassId == cCatid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = pCatid,
                        ParentName = "",
                        ClassName = cName,
                        ClassId = cCatid,
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
                        Urlinfo = cUrl,
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
