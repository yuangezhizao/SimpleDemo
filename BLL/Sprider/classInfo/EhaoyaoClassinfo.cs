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

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.ehaoyao.com/search/");

            GetCatInfo(directoryHtml);
        }


        private void GetCatInfo(string directoryHtml)
        {
            string catArea = RegGroupsX<string>(directoryHtml,
                "<div class=\"category\">(?<x>.*?)<div class=\"contact green\">");
            if (catArea == null)
                return;
        

            var list = RegGroupCollection(catArea, "<a _id=\"(?<x>.*?)\" _name=\"(?<y>.*?)\"|<li _id=\"(?<x>.*?)\" _name=\"(?<y>.*?)\"");

            foreach (Match item in list)
            {
                string tempid = item.Groups["x"].Value;
                string tempName = item.Groups["y"].Value;
                if (!ValidCatId(tempid))
                    continue;
                if (!string.IsNullOrEmpty(tempName))
                {
                    tempName = tempName.Trim();
                }
                string tempUrl = string.Format("http://www.ehaoyao.com/search?cts={0}", tempid);
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
            string cromb = RegGroupsX<string>(page, " <ul id=\"catLv2\">(?<x>.*?)</ul>");
            if (cromb == null)
                return;
            var plist = RegGroupCollection(cromb, "<a _id=\"(?<x>.*?)\" _name=\"(?<y>.*?)\"|<li _id=\"(?<x>.*?)\" _name=\"(?<y>.*?)\"");
            if (plist == null)
                return;
            string parentUrl = "";
            string parentName = "";
            string parentId = "";
            foreach (Match item in plist)
            {


               string   tempId = item.Groups["x"].Value;
               if (!ValidCatId(tempId))
                    continue;
                if (tempId == siteClassInfo.ClassId)
                    break;
                parentId = tempId;
                parentName = item.Groups["y"].Value;
                if (!string.IsNullOrEmpty(parentName))
                {
                    parentName = parentName.Trim();
                }

                parentUrl = string.Format("http://www.ehaoyao.com/search?cts={0}", tempId);
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

            GetCatInfo(page);
       

            siteClassInfo.HasChild = HasBindClasslist.Exists(c => c.ParentClass == siteClassInfo.ClassId);
            siteClassInfo.ParentClass = parentId;
            siteClassInfo.ParentName = parentName;
            siteClassInfo.ParentUrl = parentUrl;
            siteClassInfo.UpdateTime = DateTime.Now;
            siteClassInfo.TotalProduct = RegGroupsX<int>(page, "相关商品(?<x>\\d+)个");
            new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);

        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
