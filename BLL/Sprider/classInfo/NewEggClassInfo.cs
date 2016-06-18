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
    public class NewEggClassInfo : NewEgg, ISiteClassBLL
    {
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        public NewEggClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(9);
        }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.newegg.cn/CategoryList.htm");
            string catArea = RegGroupsX<string>(directoryHtml, "<div class=\"gridC1\">(?<x>.*?)<div id=\"footer\">");

            var firstCatList = RegGroupCollection(catArea, "<li class='pd\\d+'><a href='#pd(?<x>.*?)' title='(?<y>.*?)'>");
            foreach (Match item in firstCatList)
            {
                string pid = item.Groups["x"].Value;
                string pname = item.Groups["y"].Value;
                if (!ValidCatId(pid) || HasBindClasslist.Exists(c => c.ClassId == pid))
                    continue;
                SiteClassInfo catInfo = new SiteClassInfo
                {
                    ClassName = pname,
                    ClassId = pid,
                    Urlinfo = "",
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "",
                    ClassCrumble = "",
                    IsDel = false,
                    ParentClass = "",
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

            var secCatList = RegGroupCollection(catArea, "<em>\r\n\t\t\t\t\t\t\t\t\t\t    <a href='(?<x>.*?)' target='_blank' title='(?<y>.*?)'>");
            foreach (Match item in secCatList)
            {

                string url = item.Groups["x"].Value;
                string pid = RegGroupsX<string>(url, "http://www.newegg.cn/SubCategory/(?<x>\\d+).htm");
                string pname = item.Groups["y"].Value;
                if (!ValidCatId(pid) || HasBindClasslist.Exists(c => c.ClassId == pid))
                    continue;
                SiteClassInfo catInfo = new SiteClassInfo
                {
                    ClassName = pname,
                    ClassId = pid,
                    Urlinfo = "",
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "",
                    ClassCrumble = "",
                    IsDel = false,
                    ParentClass = "",
                    HasChild = false,
                    IsBind = false
                };
                HasBindClasslist.Add(catInfo);
                shopClasslist.Add(catInfo);
                if (shopClasslist.Count > 99)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
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
                     if (HasBindClasslist[i].HasChild)
                         continue;
                    UpdateNode(HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
         

        }

        private void UpdateNode(SiteClassInfo siteClassInfo)
        {
            string directoryHtml = HtmlAnalysis.Gethtmlcode(string.Format("http://www.newegg.cn/SubCategory/{0}.htm",siteClassInfo.ClassId));
            if (directoryHtml.Contains("该类别不存在"))
                return;

            string crumb = RegGroupsX<string>(directoryHtml, "<div id=\"crumb\" class=\"crumb_\\d+\">(?<x>.*?)<div id=\"wrap\">");
            string paraurl = RegGroupsX<string>(crumb, "</span><a title=\".*?\" href=\"(?<x>.*?)\">");
            string paraName = RegGroupsX<string>(crumb, "</span><a title=\"(?<x>.*?)\" href=");
            string catName = RegGroupsX<string>(crumb, "<strong>(?<x>.*?)</strong>");
            var paraInfo = HasBindClasslist.Find(c => c.ClassName == paraName);
            if (paraInfo != null)
            {
                siteClassInfo.ParentClass = paraInfo.ClassId;
                if (string.IsNullOrEmpty(paraInfo.Urlinfo))
                {
                    paraInfo.Urlinfo = paraurl;
                    new SiteClassBll().UpdateSiteCat(paraInfo);
                }
            }
            siteClassInfo.ClassName = catName;
            siteClassInfo.ParentUrl = paraurl;
            siteClassInfo.Urlinfo = string.Format("http://www.newegg.cn/SubCategory/{0}.htm", siteClassInfo.ClassId);
            siteClassInfo.ParentName = paraName;
            siteClassInfo.TotalProduct = RegGroupsX<int>(directoryHtml, "共(?<x>\\d+)个产品");
            siteClassInfo.UpdateTime = DateTime.Now;
            new SiteClassBll().UpdateSiteCat(siteClassInfo);

            string col_sub = RegGroupsX<string>(directoryHtml, "<div class=\"col_sub\">(?<x>.*?)(<div class=\"mt10 hotrank oneWeek\">|<h3> 蛋友热评</h3>|<h3>投票</h3>)");
            if (string.IsNullOrEmpty(col_sub))
                return;
            col_sub = col_sub.Replace("&#47;", "/").Replace("&#58;", ":");
            var catlist = RegGroupCollection(col_sub, "<a href=\"(?<y>.*?)\">(?<x>.*?)</a>");
            if (catlist == null)
                return;
            foreach (Match match in catlist)
            {
                string tempurl = match.Groups["y"].Value;
                string tempName = match.Groups["x"].Value;
                string tempid = RegGroupsX<string>(tempurl, "http://www.newegg.cn/SubCategory/(?<x>.*?).htm");
                if (tempid == null)
                    continue;
                if (HasBindClasslist.Exists(c => c.ClassId == tempid))
                    continue;
                SiteClassInfo catInfo = new SiteClassInfo
                {
                    ClassName = tempName,
                    ClassId = tempid,
                    Urlinfo = tempurl,
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "",
                    ClassCrumble = "",
                    IsDel = false,
                    ParentClass = "",
                    HasChild = false,
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

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
