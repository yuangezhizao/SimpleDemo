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
    public class SuningClassInfo : Suning, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }

        public SuningClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(6);
        }


        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string url = "http://www.suning.com/emall/pgv_10052_10051_1_.html";
            string page =HtmlAnalysis.Gethtmlcode(url);
            string content = RegGroupsX<string>(page, "target=\"_blank\">手机/数码/配件</a></h3>(?<x>.*?)<h3 class=\"sName\"><a name=\"sFloor1\" href=\"http://book.suning.com/\" target=\"_blank\">图书</a></h3>");
            var list = RegGroupCollection(content, "<a class=\"searchCity\" id=\"\\d+\" href=\"(?<x>.*?)\" title=\"(?<y>.*?)\" target=\"_blank\">");
            foreach (Match item in list)
            {
                string tempUrl = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempid =RegGroupsX<string>(tempUrl,"http://list.suning.com/0-(?<x>\\d+)-0");
                if (!HasBindClasslist.Exists(c => c.ClassId == tempid) && ValidCatId(tempid))
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

            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                if (HasBindClasslist[i].Id < 637691)
                    continue;
                int classid = 0;
                
                try
                {
                    if (!int.TryParse(HasBindClasslist[i].ClassId, out classid))
                        continue;
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
            string url = string.Format("http://list.suning.com/0-{0}-0-0-0-9017.html", siteClassInfo.ClassId);
            string pageinfo = HtmlAnalysis.Gethtmlcode(url);
            if (pageinfo.Contains("<div class=\"no-result-page\">对不起，没有找到符合条件的商品，试试修改筛选条件或关键词重新查找</div>"))
            {
                siteClassInfo.IsDel = true;
                new SiteClassBll().UpdateSiteCat(siteClassInfo);
                return;
            }
            string cromp = RegGroupsX<string>(pageinfo, "<div class=\"breadNavBg\">(?<x>.*?)</h1>");
            if (string.IsNullOrEmpty(cromp)) return;

            string proName = RegGroupsX<string>(cromp, "<span>(?<x>.*?)</span>");
            if (string.IsNullOrEmpty(proName))
            {
                proName = RegGroupsX<string>(pageinfo, "<div class=\"searchKeyT\"><b>(?<x>.*?)</b>");
                if (string.IsNullOrEmpty(proName))
                    return;
            }


            var caplist = RegGroupCollection(cromp, "<a title=\"(?<y>.*?)\" href=\"(?<x>.*?)\">|<a id=\"\\d+\" href=\"(?<x>.*?)\">(?<y>.*?)</a>|<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            string parentUrl="";
            string parentName="";
            string parentid="";
            string classCrumble = "";
            if(caplist!=null)
                for (int i = 0; i < caplist.Count; i++)
                {
                    parentUrl = caplist[i].Groups["x"].Value;
                    parentName = caplist[i].Groups["y"].Value;
                    if (parentName == "首页")
                        continue;
                    parentid = RegGroupsX<string>(parentUrl, "list.suning.com/\\d+-(?<x>\\d*)-\\d+.html|ci=(?<x>\\d*)");
                    if (parentid != null)
                        classCrumble += parentid + ",";
                }
            if (parentid == null)
                parentid = "";
            classCrumble = classCrumble.TrimEnd(',');
   
            int total = RegGroupsX<int>(pageinfo, "搜索到<i>(?<x>\\d+)</i>件相关商品");
            SiteClassInfo cat = new SiteClassInfo
            {
                ParentUrl=parentUrl,
                ParentClass = parentid,
                ParentName=parentName,
                TotalProduct=total,
                Urlinfo=url,
                ClassId=siteClassInfo.ClassId,
                UpdateTime=DateTime.Now,
                IsDel=false,
                BindClassId= siteClassInfo.BindClassId,
                BindClassName="",
                Id=siteClassInfo.Id,
                HasChild=siteClassInfo.HasChild,
                IsBind=siteClassInfo.IsBind,
                IsHide = false,
                ClassName=proName,
                SiteId=Baseinfo.SiteId,
                ClassCrumble=classCrumble,
                CreateDate = DateTime.Now
            };
            if (HasBindClasslist.Exists(p => p.ClassId == cat.ClassId))
            {
                siteClassInfo.Urlinfo = cat.Urlinfo;
                siteClassInfo.ClassName = cat.ClassName;
                siteClassInfo.TotalProduct = cat.TotalProduct;
                siteClassInfo.ParentUrl = cat.ParentUrl;

                if (!string.IsNullOrEmpty(cat.ParentClass))
                {
                    siteClassInfo.ParentClass = cat.ParentClass;
                    siteClassInfo.ParentName = cat.ParentName;
                    siteClassInfo.ParentUrl = cat.ParentUrl;
                }
                siteClassInfo.UpdateTime = DateTime.Now;
                new SiteClassBll().UpdateSiteCat(siteClassInfo);

            }
            else
            {
                HasBindClasslist.Add(cat);
                shopClasslist.Add(cat);
            }


            string navBar = RegGroupsX<string>(pageinfo, "<div class=\"navBar\">(?<x>.*?)<div class=\"proList");



            var list = RegGroupCollection(navBar, "<a  name=\".*?\" id=\"(?<x>\\d+)\" href=\"(?<y>.*?)\" title=\"(?<z>.*?)\">");

            if (list == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                string id = list[i].Groups["x"].Value;
                if (string.IsNullOrEmpty(id))
                    continue;
                if (!HasBindClasslist.Exists(p => p.ClassId == id))
                {
                    // add
                    string temurl = string.Format("http://list.suning.com/0-{0}-0.html", id);
                    GetNode(temurl);
                }
            }
            if (shopClasslist.Count > 0)
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }


        }


        private void GetNode(string url)
        {
            string classid = RegGroupsX<string>(url, "list.suning.com/\\d+-(?<x>\\d*)-\\d+.html|ci=(?<x>\\d*)");
            if (!ValidCatId(classid))
            {
                LogServer.WriteLog("ClassId:" + classid + "验证失败\turl:" + url, "AddClassError");
                return;
            }
            if (HasBindClasslist.Exists(p => p.ClassId == classid))
                return;
            string pageinfo = HtmlAnalysis.Gethtmlcode(url);
            string cromp = RegGroupsX<string>(pageinfo, "<div class=\"breadNavBg\">(?<x>.*?)</h1>");
            if (string.IsNullOrEmpty(cromp)) return;

            string proName = RegGroupsX<string>(cromp, "<span>(?<x>.*?)</span>");
            if (string.IsNullOrEmpty(proName))
                return;

            var caplist = RegGroupCollection(cromp, "<a title=\"(?<y>.*?)\" href=\"(?<x>.*?)\">");
            string parentUrl = "";
            string parentName = "";
            string parentid = "";
            string classCrumble = "";
            if (caplist != null)
                for (int i = 0; i < caplist.Count; i++)
                {
                    if (i == 0)
                        continue;
                    parentUrl = caplist[i].Groups["x"].Value;
                    parentName = caplist[i].Groups["y"].Value;
                    parentid = RegGroupsX<string>(parentUrl, "list.suning.com/\\d+-(?<x>\\d*)-\\d+.html|ci=(?<x>\\d*)");
                    classCrumble += parentid + ",";
                }
            classCrumble = classCrumble.TrimEnd(',');

            int total = RegGroupsX<int>(pageinfo, "搜索到<i>(?<x>\\d+)</i>件相关商品");
            SiteClassInfo cat = new SiteClassInfo
            {
                ParentUrl = parentUrl,
                ParentClass = parentid,
                ParentName = parentName,
                TotalProduct = total,
                Urlinfo = url,
                ClassId = classid,
                UpdateTime = DateTime.Now,
                IsDel = false,
                BindClassId =0,
                BindClassName = "",
                HasChild = false,
                IsBind = false,
                IsHide =false,
                ClassName = proName,
                SiteId = Baseinfo.SiteId,
                ClassCrumble = classCrumble,

                CreateDate =DateTime.Now
            };
            HasBindClasslist.Add(cat);
            shopClasslist.Add(cat);

        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
