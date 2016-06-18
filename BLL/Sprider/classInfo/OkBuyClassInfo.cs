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
    public class OkBuyClassInfo : OkBuy, ISiteClassBLL
    {
        public OkBuyClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(33);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.okbuy.com/");

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<span id=\"allcat\">(?<x>.*?)<a href=\"http://www.okbuy.com/mobile\"");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>http://www.okbuy.com/\\w*/\\d+)\" title=\"(?<y>.*?)\">");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (string.IsNullOrEmpty(tempName))
                    continue;

                string tempid = RegGroupsX<string>(tempUrl, "^http://www.okbuy.com/\\w*/(?<x>\\d+)$|^http://www.okbuy.com/(?<x>\\w*)$");
                if (!ValidCatId(tempid))
                    continue;
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
            string cromb = RegGroupsX<string>(page, "id='baseName' title='好乐买'>好乐买 </a>(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "href='(?<x>.*?)' title=(\"|')(?<y>.*?)(\"|')");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName="";
            string parentId = "";
            foreach (Match item in plist)
            {
                if(item.ToString().Contains(siteClassInfo.ClassName))
                    continue;
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                parentId = RegGroupsX<string>(parentUrl, "^http://www.okbuy.com/\\w*/(?<x>\\d+)$|^http://www.okbuy.com/(?<x>\\w*)$");
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

            string brotherCat = RegGroupsX<string>(page, "<!--BEGIN sub_category list-->(?<x>.*?)<!--END sub_category list-->");
            if (brotherCat != null)
            {
                var blist = RegGroupCollection(brotherCat, "<a title=\"(?<y>.*?)\\((?<z>\\d+)\\)\" href=\"(?<x>.*?)\" name=\"category\">");
                if (blist != null)
                {
                    foreach (Match item in blist)
                    {
                        string burl = item.Groups["x"].Value;
                        string bName = item.Groups["y"].Value;

                        string bId = RegGroupsX<string>(burl, "^http://www.okbuy.com/\\w*/(?<x>\\d+)$|^http://www.okbuy.com/(?<x>\\w*)$");
                        if (!HasBindClasslist.Exists(c => c.ClassId == bId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = "",
                                ParentName = "",
                                ClassName = bName,
                                ClassId = bId,
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
                                Urlinfo = burl,
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


            if (brotherCat != null )
                siteClassInfo.HasChild = true;
            else
                siteClassInfo.HasChild = false;


            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "共找到 <b>(?<x>\\d+)</b> 个商品");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
