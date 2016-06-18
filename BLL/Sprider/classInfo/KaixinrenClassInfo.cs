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
    public class KaixinrenClassInfo : Kaixinren, ISiteClassBLL
    {
       public KaixinrenClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(265);
        }
       //              http://www.360kxr.com/Category/GetListByParentId/0
       private string domain = "http://www.360kxr.com";
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode(domain);

            GetCatInfo(directoryHtml);
        }


        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "全部商品分类</h2>(?<x>.*?)<div class=\"nav-right\">");
            if (catArea == null)
                return;
            //catArea = catArea.Replace("\t", "").Replace("\r", "").Replace("\n", "");

            var list = RegGroupCollection(catArea, "href=('|\")(?<x>.*?)('|\")\\s*>(?<y>.*?)</a>");

            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid = RegGroupsX<string>(tempUrl, "category/(?<x>\\d+)-");
                if (ValidCatId(tempid) &&!HasBindClasslist.Exists(c => c.ClassId == tempid))
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
                    if (HasBindClasslist[i].ParentName=="")
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
            string cromb = RegGroupsX<string>(page, "您现在的位置：</span>(?<x>.*?)</div>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a class=\"\" href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {
               
                parentUrl = item.Groups["x"].Value;
                parentName = item.Groups["y"].Value;
                if(parentName=="首页")
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }
                if (parentName == "")
                { parentUrl = ""; continue; }
                parentId = RegGroupsX<string>(parentUrl, "category/(?<x>\\d+)-");
                if (!ValidCatId(parentId))
                {
                    parentId = RegGroupsX<string>(parentUrl, "/(?<x>.*?).html");
                    if (string.IsNullOrEmpty(parentId))
                        continue;
                }
                
                parentUrl = string.Format(domain+"{0}", parentUrl);
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


            var templist = RegGroupCollection(page, "getJSON\\(\"(?<x>.*?)\"");
            for (int i = 0; i < templist.Count; i++)
            {
                var caturl = templist[i].Groups["x"].Value;
                string temppage = HtmlAnalysis.Gethtmlcode(domain+ caturl);
                var catlist = RegGroupCollection(temppage, "n_(?<x>.*?)\"EntityState");
                if (catlist == null)
                    continue;
                foreach (Match item in catlist)
                {
                    string cat = item.Groups["x"].Value;

                    string catid = RegGroupsX<string>(cat, "id\":(?<x>\\d+),");
                    string catName = RegGroupsX<string>(cat, "\"n_name\":\"(?<x>.*?)\"");
                    string catpid = RegGroupsX<string>(cat, "\"parentid\":(?<x>.*?),");
                    string tempurl = string.Format("http://www.360kxr.com/category/{0}-0-2-1-15-1.html", catid);
                    if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                    {
                        SiteClassInfo iteminfo = new SiteClassInfo
                        {
                            ParentClass = catpid,
                            ParentName = "",
                            ClassName = catName,
                            ClassId = catid,
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

            }


           
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }



            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "<div class=\"goods-total\">共<b>(?<x>\\d+)</b>个商品</div>");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
