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
    /// 品致会分类管理 127
    /// </summary>
    public class UnieclubClassInfo: Unieclub, ISiteClassBLL
    {
        public UnieclubClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(127);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.unieclub.com/", "utf8", false);

            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<li class=\"level0 nav-1 level-top first parent\">(?<x>.*?)<div class=\"nav-quicklinks\">");
            if (catArea == null)
                return;

            var list = RegGroupCollection(catArea, "<a href=\"(?<x>.*?)\"( class=\"level-top\")?>\n<span>(?<y>.*?)</span>");
            string parentid = "";
            string parentName = "";
            string parentUrl = "";
            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                string tempid = RegGroupsX<string>(tempUrl, "http://www.unieclub.com/(?<x>.*?).html");
                string[] cat = tempid.Split('/');
                tempid = cat[0];

                if (cat.Length > 1)
                {
                    tempid = cat[cat.Length - 1];
                }
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = parentid,
                        ParentName = parentName,
                        ClassName = tempName,
                        ClassId = tempid,
                        ParentUrl = parentUrl,
                        IsDel = false,
                        IsBind = false,
                        IsHide = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = item.ToString().Contains("class=\"level-top\""),
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
                if (cat.Length == 1)
                {
                    parentUrl = tempUrl;
                    parentid = cat[0];
                    parentName = tempName;
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
            SaveAllSiteClass();
        }
        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
