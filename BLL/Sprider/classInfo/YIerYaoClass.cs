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
    public class YIerYaoClass : YierYao, ISiteClassBLL
    {

        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public YIerYaoClass()
        {
           Baseinfo = new SiteInfoDB().SiteById(243);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string classurl = "http://www.12yao.com/sitemap.html";
            string page = HtmlAnalysis.Gethtmlcode(classurl);
             
            string content = RegGroupsX<string>(page, "<div class=\"linkall\">(?<x>.*?)最新文章");
            var list = content.Split(new string[] { "<h2>" }, StringSplitOptions.RemoveEmptyEntries);
            //var list = RegGroupCollection(content, "<h2>(?<x>.*?)<h2>");
            for (int i = 0; i < list.Length; i++)
            {
                string area1 = list[i];
                if (area1.Length < 10)
                    continue;
                string firstNode = RegGroupsX<string>(area1, "^(?<x>.*?)</h2>");
                string firstUrl = RegGroupsX<string>(firstNode, "href=\"(?<x>.*?)\"");
                string firstName = RegGroupsX<string>(firstNode, "target=\"_blank\">(?<x>.*?)</a>");
                string firstClassid = i.ToString();
                if (!HasBindClasslist.Exists(p => p.ClassId == firstClassid))
                {
                    SiteClassInfo cat = new SiteClassInfo
                    {
                        ParentUrl = "",
                        ParentClass = "",
                        ParentName = "",
                        TotalProduct = 0,
                        Urlinfo = firstUrl,
                        ClassId = firstClassid,
                        UpdateTime = DateTime.Now,
                        IsDel = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = true,
                        IsBind = false,
                        IsHide = false,
                        ClassName = firstName,
                        SiteId = Baseinfo.SiteId,
                        ClassCrumble = "",

                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(cat);
                    shopClasslist.Add(cat);
                }

                var list2 = RegGroupCollection(area1, "<dl >(?<x>.*?)</dl>");

                for (int j = 0; j < list2.Count; j++)
                {
                    string area2 = list2[j].Groups["x"].Value;
                    string SecNode = RegGroupsX<string>(area2, "<dt>(?<x>.*?)</dt>");
                    string SecUrl = RegGroupsX<string>(SecNode, "href=\"(?<x>.*?)\"");
                    string SecName = RegGroupsX<string>(SecNode, "target=\"_blank\">(?<x>.*?)</a>");
                    string SecClassid = RegGroupsX<string>(SecUrl, "http://www.12yao.com/product/(?<x>\\d+\\-\\d+)\\-0");
                    if (!HasBindClasslist.Exists(p => p.ClassId == SecClassid))
                    {
                        SiteClassInfo Seccat = new SiteClassInfo
                        {
                            ParentUrl = firstUrl,
                            ParentClass =firstClassid,
                            ParentName = firstName,
                            TotalProduct = 0,
                            Urlinfo = SecUrl,
                            ClassId = SecClassid,
                            UpdateTime = DateTime.Now,
                            IsDel = false,
                            BindClassId = 0,
                            BindClassName = "",
                            HasChild = true,
                            IsBind = false,
                            IsHide = false,
                            ClassName = SecName,
                            SiteId = Baseinfo.SiteId,
                            ClassCrumble = firstClassid,
                            CreateDate = DateTime.Now
                        };
                        HasBindClasslist.Add(Seccat);
                        shopClasslist.Add(Seccat);
                    }

                    var threeList = RegGroupCollection(area2, "<dd><a href=\"(?<x>.*?)\">(?<y>.*?)</a></dd>");
                    if (threeList == null)
                        continue;

                    for (int k = 0; k < threeList.Count; k++)
                    {
                        string threeUrl = threeList[k].Groups["x"].Value;
                        string threeName = threeList[k].Groups["y"].Value;
                        string threeClassid = RegGroupsX<string>(threeUrl, "http://www.12yao.com/product/(?<x>\\d+\\-\\d+\\-\\d+)\\-0");

                        if (!HasBindClasslist.Exists(p => p.ClassId == threeClassid))
                        {
                            SiteClassInfo threecat = new SiteClassInfo
                            {
                                ParentUrl = SecUrl,
                                ParentClass = SecClassid,
                                ParentName = SecName,
                                TotalProduct = 0,
                                Urlinfo = threeUrl,
                                ClassId = threeClassid,
                                UpdateTime = DateTime.Now,
                                IsDel = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = false,
                                IsBind = false,
                                IsHide = false,
                                ClassName = threeName,
                                SiteId = Baseinfo.SiteId,
                                ClassCrumble = firstClassid + "," + SecClassid,
                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(threecat);
                            shopClasslist.Add(threecat);
                        }
                     
                    }
                    if (shopClasslist.Count > 50)
                    {
                        new SiteClassInfoDB().AddSiteClass(shopClasslist);
                        shopClasslist.Clear();
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
            string pageinfo = HtmlAnalysis.Gethtmlcode(siteClassInfo.Urlinfo);
            siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "找到相关商品 <em>(?<x>\\d+)</em> 件");
            siteClassInfo.UpdateTime = DateTime.Now;
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
