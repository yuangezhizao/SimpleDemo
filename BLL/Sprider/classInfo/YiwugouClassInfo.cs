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
    public class YiwugouClassInfo : YiWuGou, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public YiwugouClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(2) ?? new SiteInfo { SiteId = 2 };
        }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string url = "http://www.yiwugou.com/shop_list/i_1.html";
            string pageinfo = HtmlAnalysis.Gethtmlcode(url);

            string catArea = RegGroupsX<string>(pageinfo,
                "<div class=\"nav-class-bord\" id=\"maket-id\">(?<x>.*?)<div class=\"nav-right\">");

            var building = RegGroupCollection(catArea, "<li(?<x>.*?)</li>");

            if (building == null)
                return;
            for (int i = 0; i < building.Count; i++)
            {
                var floor = building[i].Groups["x"].Value;
                var catlist = RegGroupCollection(floor, "<a(?<x>.*?)</a>");
                if (catlist == null)
                    continue;
                string parentid = "";
                string parentUrl = "";
                string parentName = "";
                for (int j = 0; j < catlist.Count; j++)
                {
                    string catinfo = catlist[j].Groups["x"].Value;
                    string caturl = RegGroupsX<string>(catinfo, "href=\"(?<x>.*?)\"");
                    if (string.IsNullOrEmpty(caturl))
                        continue;
                    caturl = "http://www.yiwugou.com" + caturl;
                    string catId = RegGroupsX<string>(caturl, "/product_list/i_1_(?<x>.*?).html");
                    
                    string catName = RegGroupsX<string>(catinfo, ">(?<x>.*?)$");
                    catName = WordCenter.FilterHtml(catName);
                    if (HasBindClasslist.Exists(c => c.ClassId == catId)||!ValidCatId(catId))
                    {
                        continue;
                    }

                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = parentUrl,
                        ParentClass = parentid,
                        ParentName = parentName,
                        TotalProduct = 0,
                        Urlinfo = caturl,
                        ClassId = catId,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = !catId.Contains("_"),
                        IsBind = false,
                        IsHide = false,
                        ClassName = catName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);

                    if (!catId.Contains("_"))
                    {
                        parentid = catId;
                        parentName = catName;
                        parentUrl = caturl;
                    }

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
            string page = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo.Replace("product_list", "shop_list"));

            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "<li>共找到商铺 <span class=\"fontred\">(?<x>\\d+)</span> 个</li>");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
