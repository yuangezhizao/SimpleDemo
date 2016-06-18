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
    public class MiaClassInfo : Mia, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public MiaClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(261) ?? new SiteInfo { SiteId = 261 };
        }

        public void SaveAllSiteClass()
        {
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            //http://www.miyabaobei.com/
            string homePage = HtmlAnalysis.Gethtmlcode("http://www.miyabaobei.com/","utf8",false);
            string catArea = RegGroupsX<string>(homePage, "<!-- 分类开始 -->(?<x>.*?)<!-- 分类结束 -->");
            var catidList = RegGroupCollection(catArea, "href=\"/search/s\\?cat=(?<x>\\d+)");

            for (int i = 0; i < catidList.Count; i++)
            {
                var catid = catidList[i].Groups["x"].Value;
                if (!ValidCatId(catid) || HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    continue;
                }
                string tempcatUrl = string.Format("http://www.miyabaobei.com/search/s?cat={0}", catid);
                string catPage = HtmlAnalysis.Gethtmlcode(tempcatUrl);

                string crumbs = RegGroupsX<string>(catPage, "<div class=\"crumbs\">(?<x>.*?)</div>");
                string catName = RegGroupsX<string>(crumbs, "&gt;(?<x>.*?)$");

                catName = WordCenter.FilterHtml(catName);
                if (string.IsNullOrEmpty(catName))
                    continue;

                SiteClassInfo cat = new SiteClassInfo
                {
                    ParentUrl = "",
                    ParentClass = "",
                    ParentName = "",
                    TotalProduct = 0,
                    Urlinfo = tempcatUrl,
                    ClassId = catid,
                    UpdateTime = DateTime.Now,
                    IsDel = false,
                    BindClassId = 0,
                    BindClassName = "",
                    HasChild = false,
                    IsBind = false,
                    IsHide = false,
                    ClassName = catName,
                    SiteId = Baseinfo.SiteId,
                    ClassCrumble = "",

                    CreateDate = DateTime.Now
                };
                HasBindClasslist.Add(cat);
                shopClasslist.Add(cat);


            }

            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }


        }

        public void UpdateSiteCat()
        {
             //SaveAllSiteClass();

             HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

             for (int i = 0; i < HasBindClasslist.Count; i++)
             {
                 try
                 {
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
            string tempcatUrl = string.Format("http://www.miyabaobei.com/search/s?cat={0}", siteClassInfo.ClassId);
            string catPage = HtmlAnalysis.Gethtmlcode(tempcatUrl);

            string crumbs = RegGroupsX<string>(catPage, "<div class=\"crumbs\">(?<x>.*?)</div>");
            string catName = RegGroupsX<string>(crumbs, "&gt;(?<x>.*?)$");

            catName = WordCenter.FilterHtml(catName);
            if (string.IsNullOrEmpty(catName))
                return;
            siteClassInfo.TotalProduct= RegGroupsX<int>(catPage, "共计<em class=\"pink\">(?<x>\\d+)</em>个商品</div>");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
