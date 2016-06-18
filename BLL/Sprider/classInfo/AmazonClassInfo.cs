using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Commons;
using DataBase;
using Mode;
using SpriderProxy.Analysis;

namespace BLL.Sprider.classInfo
{
    public class AmazonClassInfo : Amazon, ISiteClassBLL
    {
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        const string CatListMode = "http://www.amazon.cn/b/ref=?ie=UTF8&node={0}";
    


        public AmazonClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(4);
        }
        private List<string> catUrls = new List<string>();
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.amazon.cn/gp/site-directory");
      
            string tempPage = RegGroupsX<string>(directoryHtml, "<div id=\"cat4\" class=\"a-row a-spacing-small a-spacing-top-medium\">(?<x>.*?)<script>");

            var list = RegGroupCollection(tempPage, "href=\"(?<x>.*?)\"|href='(?<x>.*?)'");

            for (int i = 0; i < list.Count; i++)
            {
                string tempurl = list[i].Groups["x"].Value.Replace("&amp;", "&");
                if (!tempPage.Contains("http://www.amazon.cn/"))
                    tempurl = "http://www.amazon.cn/" + tempurl;
         
                if (catUrls.Contains(tempurl))
                    continue;
                catUrls.Add(tempurl);
            }

            for (int i = 0; i < catUrls.Count; i++)
            {
                string tempurl = catUrls[i];

                if (!tempurl.Contains("rh=n%3A"))
                {
                    string tempid = RegGroupsX<string>(catUrls[i],
                        "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)|rh=n%3A(?<x>\\d+)%2Cp_\\d");
                    if (string.IsNullOrEmpty(tempid))
                        continue;

                    tempurl = string.Format(CatListMode, tempid);
                    GetAmazonNode(tempurl);

                }
                else if(tempurl.IndexOf('&')>0)
                {
                    var catIdList = RegGroupCollection(tempurl, "(?<x>\\d{8,10})");

                    for (int j = 0; j < catIdList.Count; j++)
                    {
                        string id = catIdList[j].Value;
                        tempurl = string.Format(CatListMode, id);
                        GetAmazonNode(tempurl);
                    }

                }

                if (shopClasslist.Count > 0)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
                }

            }

        }

        private void GetAmazonNode(string url)
        {

            string pageinfo = HtmlAnalysis.Gethtmlcode(url);
            if (pageinfo.Contains("ResponseUri:http://www.amazon.cn/") || pageinfo.Contains("没有找到任何与"))
                return;

            string classList = RegGroupsX<string>(pageinfo, "<div class=\"sbDepartmentLabel\">(?<x>.*?)<form id=\"bottomSearchForm\"|<div class=\"categoryRefinementsSection\">(?<x>.*?)<div class=\"shoppingEngineSectionHeaders\"");
            if (classList == null)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + url, "AddClassError");
                return;
            }
            var classInfo = RegGroupCollection(classList, "<span class=\"deptLevel\\d+( deptCurrent)?\">(?<x>.*?)</span>|<li.*?>(?<x>.*?)</li>");
            if (classInfo == null || classInfo.Count == 0)
                return;

            SiteClassInfo catInfo = new SiteClassInfo();
            catInfo.ClassName = RegGroupsX<string>(classList, "deptCurrent\">(?<x>.*?)</span>|<strong>(?<x>.*?)</strong>");
            if (!ValidCatName(catInfo.ClassName))
                return;
            string count =
              RegGroupsX<string>(pageinfo,
                  "<span>显示(?<x>.*?)个结果|<span>显示所有(?<x>.*?)个结果</span>|共 (?<x>\\d+) 个结果|共(?<x>.*?)条|<div id=\"resultCount\" class=\"toTheEdge searchListHeader\">\n(?<x>.*?) 条结果</div>");
            if (count != null)
            {
                int procount;
                int.TryParse(count.Replace(" ",""), out procount);
                catInfo.TotalProduct = procount;
            }
            catInfo.Urlinfo = url;
            catInfo.ClassId = RegGroupsX<string>(url, "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)|rh=n%3A(?<x>\\d+)%2Cp_\\d|lp_(?<x>\\d+)_");
            if (!ValidCatId(catInfo.ClassId))
            {
                LogServer.WriteLog("ClassId:" + catInfo.ClassId + "验证失败\turl:" + url, "AddClassError");
                return;
            }
         
            catInfo.IsHide = false;
            catInfo.UpdateTime = DateTime.Now;
            catInfo.CreateDate = DateTime.Now;
            catInfo.IsBind = false;
            catInfo.IsDel = false;
            catInfo.SiteId = Baseinfo.SiteId;


            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";
            string currentName = "";

            for (int i = 0; i < classInfo.Count; i++)
            {
               
                var item = classInfo[i].ToString();
                string tempUrl = RegGroupsX<string>(item, "href=\"(?<x>.*?)\"");
                string tempid = "";
                if (tempUrl != null)
                {
                    tempUrl = "http://www.amazon.cn/" + tempUrl.Replace("&amp;", "&");
                    tempid = RegGroupsX<string>(tempUrl, "3A(?<x>\\d+)\\&bbn");
                    if (tempid == null)
                        tempid = RegGroupsX<string>(item, "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)|rh=n%3A(?<x>\\d+)%2Cp_\\d|lp_(?<x>\\d+)_");
                }
                else
                    continue;

                if (!ValidCatId(tempid))
                {
                    LogServer.WriteLog("ClassId:" + catInfo.ClassId + "验证失败\turl:" + url, "AddClassError");
                    continue;
                }

                string tempName = RegGroupsX<string>(item, "&lt;&nbsp;(?<x>.*?)</a>|rnid=\\d+\">(?<x>.*?)<span class=\"gray\">|<span class=\"refinementLink\">(?<x>.*?)</span>|</span><span>(?<x>.*?)</span></a>");
                if (string.IsNullOrEmpty(tempName))
                {
                    currentName = RegGroupsX<string>(classList, "deptCurrent\">(?<x>.*?)</span>|<strong>(?<x>.*?)</strong>"); //RegGroupsX<string>(item, "<strong>(?<x>.*?)</strong>");
                    if (currentName == null )
                    {
                        LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误2", "AddClassError");
                        return;
                    }
                  
                    if (!HasBindClasslist.Exists(p => p.ClassId == catInfo.ClassId))
                    {
                        if (i < classInfo.Count - 1)
                            catInfo.HasChild = true;
                        else
                            catInfo.HasChild = false;
                        catInfo.ParentClass = pcatId;
                        catInfo.ParentName = pcatName;
                        catInfo.ParentUrl = pcatUrl;
                        catInfo.ClassCrumble = classCrumble.TrimEnd(',');
                        HasBindClasslist.Add(catInfo);
                        shopClasslist.Add(catInfo);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tempid)&&!HasBindClasslist.Exists(p => p.ClassId == tempid))
                    {
                        if (!catUrls.Contains(tempUrl))
                            catUrls.Add(tempUrl);
                    }

                }
                if (currentName == "")
                {
                    pcatUrl = tempUrl;
                    pcatName = tempName;
                    pcatId = tempid;//RegGroupsX<string>(pcatUrl, "node=(?<x>\\d*)|nodeId=(?<x>\\d*)");
                    classCrumble += pcatId + ",";
                }
            }


        }

        public void UpdateSiteCat()
        {
            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();
            HtmlAnalysis html = new HtmlAnalysis();
            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                try
                {
                    UpdateCat(html,HasBindClasslist[i]);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }
        }

        /// <summary>
        /// 更新分类，有问题需优化
        /// </summary>
        /// <param name="siteClassInfo"></param>
        private void UpdateCat( HtmlAnalysis html,SiteClassInfo siteClassInfo)
        {
            string url =string.Format(CatListMode,siteClassInfo.ClassId);


            
            //html.RequestAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //html.RequestHeaders.Add("x-amz-id-1", "12SNRGFVN7F3GN9TZQ6A");
            //html.RequestHeaders.Add("x-amz-id-2", "HHlJ6ZQcJqqihTf2uhlh7xPqV4kx2CyUKZOeVXInz/IdAUgpQDfMInz6tG5ia6VKvlmZXjpDTPz 2WwRGyFE9A==");
            //html.RequestHeaders.Add("X-Frame-Options", "SAMEORIGIN");
            //html.RequestHeaders.Add("x-ua-compatible", "IE=edge");
            //html.RequestHeaders.Add("Cookie", "x-wl-uid=1ABOedPNZYW+nIQbl1b30JM+0+4U4bA6yQoNrlasdwDpl1IqkNfHVhbzaL5/qqOVMsWtwsc2i3RM=; amznacsleftnav-99427612=1,2; amznacsleftnav-100213812=1; s_nr=1440467727734-Repeat; s_vnum=1869792751635%26vn%3D4; s_dslv=1440467727736; 5SnMamzvowels.time.4=1440664276731; 5SnMamzvowels.time.0=1441181856266; amznacsleftnav-99204312=1; amznacsleftnav-100100372=1; 5SnMamzvowels.time.1=1441508587108; 5SnMamzvowels.time.2=1441523642117; 5SnMamzvowels.pos=4; 5SnMamzvowels.time.3=1442287567488; session-token=\"1GYzzhtBqbos2Ftk+KCgTA9f5yRCkSZjj6lDEmlXHPNRua8TAmIztTkzk9RooM3be2CPBhr21h404qe4Gy3e+0zCarS/5xlXuC6Xy1C2GeGy0sFr8nGgWp1iGD+VjAuyOrdQH+mqy1Ie9/ELizvIqfF5PmoPqTy4vp92B/sNMX8xqOeHkYqdWAWHB6mJW0TAgqRgYhTgIGt8CBKxi/8Bhg==\"; x-amz-captcha-1=1444963265190387; x-amz-captcha-2=GcvGzFcFHl5jmwSO4uF/Zg==; UserPref=T/z6Om64hhzqQdB81xsMYv0BMLVePVAtO8NUr8jzM/2BocRpX2Zd+SKZAUfRAIURECn/REj5CtQscHUNwkr+LIJr0IOKTpqjXQwnd1ojeIeEJ8uB0tDMjvJhI2wjoBG5yoZuWZdEO7fGaBCDJwMi5qmump/VskQ6wtpK0Dcid8ozeNIhZvSdzwcFNYxsFxYYiS+aPd+0ZpXUn74/4YD4RAH+s8YPC06fOt1R3/XVpBPVhRq+7YhjPlQAg1RFaH02WYtSbZJV9d5sAmAJzCyK9T1yOKwuHCDsnBjPHo8FLlOptd49AkRa4xXApgrufn7Hhqecje92i3QU9X6cShPi1QIX93mkef+uArPXUMQYU7kgkvSjVkjfyYlhOQUSpDgg1j9G/PRUrY+Eb7hezUw1lvlzgb1p4nQnipeJK63vuJJCeRyfc/r4lLNdDBv3NnHV; session-id-time=2082729601l; session-id=479-8884718-9124366; ubid-acbcn=479-6055231-7810538; csm-hit=0MG3ENSCMXYNMQY80BH8+s-0MG3ENSCMXYNMQY80BH8|1446517791183");
            string pageinfo = html.HttpRequest(url);
            //string pageinfo = HtmlAnalysis.Gethtmlcode(url, "utf8", false);

            if (pageinfo.Contains("没有找到任何与") || pageinfo.Contains("<h2 id=\"s-result-count\" class=\"a-size-base a-text-normal\"><b>&nbsp;</b>\n                    </h2></div>\n        </div><div class=\"a-column a-span4 a-text-right a-spacing-none a-span-last\"><div class=\"a-row a-spacing-micro a-spacing-top-micro\"><div class=\"s-last-column\">\n                    </div>\n            </div></div></div></div><script type=\"text/javascript\">\n    \n    function viewCompleteImageLoaded(image, time, resultCount, shouldUseCSMScopes) {\n         if (image) {\n           image.onload = image.onerror = image.onabort = null;\n         }\n\n         amzn.sx.utils.jsDepMgr.when('clickToViewLogger', 'viewCompleteImageLoaded',\n           function(clickToViewLogger) {\n            if (typeof ctvcL == 'undefined') {ctvcL = new clickToViewLogger.ClickToViewCompleteLogger(\"false\");}\n            ctvcL.iL(image, time, resultCount, shouldUseCSMScopes);\n           }\n         );\n    };\n</script>\n<div class=\"img_header hdr noborder\" id=\"bottomBar\">\n    <div id=\"pagn\" class=\"pagnHy\" >\n            <br clear=\"all\" />"))
            {
                new SiteClassBll().delClass(siteClassInfo);
                return;
            }
            if (pageinfo.Contains("ResponseUri:http://www.amazon.cn/") || pageinfo.Contains("没有找到任何与") || pageinfo.Contains("请输入您在下方看到的字符"))
                return;

            string crumble = RegGroupsX<string>(pageinfo, "<div class=\"s-first-column\">(?<x>.*?)<span id=\"breadcrumbSearchSeperator\">|<h2 id=\"s-result-count\"(?<x>.*?)</h2>");
            if (crumble == null)
            {
                url = url.Replace("page=1", "page=2");
                pageinfo = HtmlAnalysis.Gethtmlcode(url);
                crumble = RegGroupsX<string>(pageinfo, "<div class=\"s-first-column\">(?<x>.*?)<span id=\"breadcrumbSearchSeperator\">|<h2 id=\"s-result-count\"(?<x>.*?)</h2>");
                if (crumble == null)
                {
                    LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + url, "AddClassError");
                    return;
                }
            }

            string currentName = RegGroupsX<string>(crumble, "<span class=\"a-color-state a-text-bold\">(?<x>.*?)</span>|<strong>(?<x>.*?)$");

            if (!ValidCatName(currentName))
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓名称匹配错误\turl:" + url, "AddClassError");
                return;
            }
            var crumlist = RegGroupCollection(crumble, "<a class=\"a-link-normal a-color-base a-text-bold a-text-normal\" href=\"(?<x>.*?)\">(?<y>.*?)</a>|<a href=\"(?<x>.*?)\">(?<y>.*?)</a>");
            //if(crumlist==null)
            //{
            //    LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误2\turl:" + url, "AddClassError");
            //    return;
            //}
            string pcatUrl = "";
            string pcatName = "";
            string pcatId = "";
            string classCrumble = "";

            if (crumlist!=null)
            foreach (Match match in crumlist)
            {
                pcatUrl = match.Groups["x"].Value;
                if (string.IsNullOrEmpty(pcatUrl))
                    continue;
                if (!pcatUrl.Contains("http"))
                    pcatUrl = "http://www.amazon.cn" + pcatUrl;

                var pcatList = RegGroupCollection(pcatUrl, "%3A(?<x>\\d+)");
                if (pcatList != null)
                {
                    foreach (Match item in pcatList)
                    {
                        pcatId = item.Groups["x"].Value;
                        if (!string.IsNullOrEmpty(pcatId) && !HasBindClasslist.Exists(p => p.ClassId == pcatId))
                        {
                            GetAmazonNode(string.Format(CatListMode, pcatId));
                        }

                    }
                }
                else
                {
                    pcatId = RegGroupsX<string>(pcatUrl,
                        "3A(?<x>\\d+)\\&bbn|/s/ref=sr_hi_\\d+\\?rh=n%3A(?<x>\\d+)|rh=n%3A(?<x>\\d+)\\&amp");

                    if (pcatId == null)
                        pcatId = RegGroupsX<string>(pcatUrl, "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)");
                }
                if (!string.IsNullOrEmpty(pcatId))
                {
                    classCrumble += pcatId + ",";
                }
                pcatName = WordCenter.FilterHtml(match.Groups["y"].Value);

            }


            SiteClassInfo catInfo = new SiteClassInfo();
            catInfo.ClassName = currentName;
            catInfo.ParentName = pcatName;
            catInfo.ParentClass = pcatId;
            catInfo.ParentUrl = pcatUrl;
            catInfo.ClassCrumble = classCrumble;
            string count =
              RegGroupsX<string>(pageinfo,
                  "<span>显示(?<x>.*?)个结果|<span>显示所有(?<x>.*?)个结果</span>|共(?<x>.*?)条|<div id=\"resultCount\" class=\"toTheEdge searchListHeader\">\n(?<x>.*?) 条结果</div>");
            if (count != null)
            {
                int procount;
                int.TryParse(count.Replace(" ", ""), out procount);
                catInfo.TotalProduct = procount;
            }
            catInfo.Urlinfo = url;
            catInfo.ClassId = RegGroupsX<string>(url, "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)|rh=n%3A(?<x>\\d+)%2Cp_\\d|ref=lp_(?<x>\\d+)_pg");
            if (!ValidCatId(catInfo.ClassId))
            {
                LogServer.WriteLog("ClassId:" + catInfo.ClassId + "验证失败\turl:" + url, "AddClassError");
                return;
            }
            if (siteClassInfo.ClassId != catInfo.ClassId)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "抓取分类id不一致 old:" + siteClassInfo.ClassId+"new:"+ catInfo.ClassId);
                return;
            }
            //更新当前分类
            siteClassInfo.Urlinfo = catInfo.Urlinfo;
            siteClassInfo.ClassId = catInfo.ClassId;
            siteClassInfo.ClassName = catInfo.ClassName;
            siteClassInfo.TotalProduct = catInfo.TotalProduct;
            siteClassInfo.ParentUrl = catInfo.ParentUrl;
            siteClassInfo.ParentClass = catInfo.ParentClass;
            siteClassInfo.ParentUrl = catInfo.ParentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            new SiteClassBll().UpdateSiteCat(siteClassInfo);

            string classList;
            if (pageinfo.Contains("data-typeid=\"n\""))
            {
                classList = RegGroupsX<string>(pageinfo, "<ul id=\"ref_\\d+\" data-typeid=\"n\"(?<x>.*?)</ul>");
            }
            else
            {
                classList = RegGroupsX<string>(pageinfo, "<div class=\"sbDepartmentLabel\">(?<x>.*?)<form id=\"bottomSearchForm\"|<div class=\"categoryRefinementsSection\">(?<x>.*?)<div class=\"shoppingEngineSectionHeaders\">");
            }
            if (classList == null)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误1\turl:" + url, "AddClassError");
                return;
            }
            var classInfo = RegGroupCollection(classList, "<span class=\"deptLevel\\d+( deptCurrent)?\">(?<x>.*?)</span>|<li.*?>(?<x>.*?)</li>");
            if (classInfo == null || classInfo.Count == 0)
                return;
            for (int i = 0; i < classInfo.Count; i++)
            {

                var item = classInfo[i].ToString();
                string tempUrl = RegGroupsX<string>(item, "href=\"(?<x>.*?)\"");


                var pcatList = RegGroupCollection(tempUrl, "n%3A(?<x>\\d{7,16})");
                if (pcatList != null)
                {
                    foreach (Match obj in pcatList)
                    {
                        var tempcatId = obj.Groups["x"].Value;
                        if (!string.IsNullOrEmpty(pcatId) && !HasBindClasslist.Exists(p => p.ClassId == tempcatId))
                        {
                            GetAmazonNode(string.Format(CatListMode, tempcatId));
                        }

                    }
                }
                else
                {
                    string tempid = "";
                    if (tempUrl != null)
                    {
                        tempUrl = "http://www.amazon.cn" + tempUrl.Replace("&amp;", "&");
                        tempid = RegGroupsX<string>(tempUrl, "3A(?<x>\\d{7,16})\\&bbn");
                        if (tempid == null)
                            tempid = RegGroupsX<string>(item, "node=(?<x>\\d+)|nodeId=(?<x>\\d+)|bbn=(?<x>\\d+)");
                    }
                    if (!string.IsNullOrEmpty(tempid) && !HasBindClasslist.Exists(p => p.ClassId == tempid))
                    {
                        GetAmazonNode(tempUrl);
                    }
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
