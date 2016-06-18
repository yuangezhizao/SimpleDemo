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
    public class TaoXieClass : Taoxie, ISiteClassBLL
    {

        private List<SiteClassInfo> HasBindClasslist { get; set; }



        public TaoXieClass()
        {
            Baseinfo = new SiteInfoDB().SiteById(30);
        }
        public void SaveAllSiteClass()
        {
            List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://m.taoxie.com/category.html");


            var catList = RegGroupCollection(directoryHtml, "<div class=\"actit\"><img src=\"static/mobile/images/userfile/cat-(?<x>.*?).jpg\">(?<y>.*?)</div>");
            if (catList == null)
                return;
            foreach (Match item in catList)
            {
                string parId = item.Groups["x"].Value;
                string proName = WordCenter.FilterHtml(item.Groups["y"].Value);
                string url =string.Format("http://m.taoxie.com/category/category_ajaxpage?parentId={0}",parId);
                string childPage = HtmlAnalysis.Gethtmlcode(url);

                var list = RegGroupCollection(childPage, "<a href=\"/products\\?categoryId=(?<y>.*?)\">(?<x>.*?)</a>");
                foreach (Match child in list)
                {
                    string itemId = child.Groups["y"].Value;
                    string itemName = WordCenter.FilterHtml(child.Groups["x"].Value);

                    if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(itemName))
                        continue;
                    if (!ValidCatId(itemId) || HasBindClasslist.Exists(c => c.ClassId == itemId))
                        continue;
                    SiteClassInfo catInfo = new SiteClassInfo
                    {
                        ClassName = itemName,
                        ClassId = itemId,
                        SiteId = Baseinfo.SiteId,
                        CreateDate = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsHide = false,
                        ParentUrl = "",
                        ParentName = proName,
                        ClassCrumble = "",
                        IsDel = false,
                        ParentClass = parId,
                        Urlinfo = string.Format("http://m.taoxie.com/products?categoryId={0}", itemId),
                        HasChild = false,
                        IsBind = false
                    };
                    HasBindClasslist.Add(catInfo);
                    shopClasslist.Add(catInfo);
                }

                if (!ValidCatId(parId) || HasBindClasslist.Exists(c => c.ClassId == parId))
                    continue;

                SiteClassInfo parcatInfo = new SiteClassInfo
                {
                    ClassName = proName,
                    ClassId = parId,
                    SiteId = Baseinfo.SiteId,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsHide = false,
                    ParentUrl = "",
                    ParentName = "",
                    ClassCrumble = "",
                    IsDel = false,
                    ParentClass = "",
                    Urlinfo = string.Format("http://m.taoxie.com/products?categoryId={0}", parId),
                    HasChild = false,
                    IsBind = false
                };
                HasBindClasslist.Add(parcatInfo);
                shopClasslist.Add(parcatInfo);
           
  
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
            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            foreach (SiteClassInfo cat in HasBindClasslist)
            {
                try
                {
                    UpdateNode(cat);
                }
                catch (Exception ex)
                {
                    LogServer.WriteLog(ex);
                }
            }

        }

        private void UpdateNode(SiteClassInfo item)
        {
            string catPage = HtmlAnalysis.Gethtmlcode(item.Urlinfo);
            int proTotal = RegGroupsX<int>(catPage, "共有商品 <em>(?<x>.*?)</em> 个");
           
            var oldCatInfo = HasBindClasslist.Find(c => c.ClassId == item.ClassId);
            if (oldCatInfo == null)
                return;
            if (proTotal > 0)
            {
                oldCatInfo.TotalProduct = proTotal;
                oldCatInfo.UpdateTime = DateTime.Now;
                new SiteClassBll().UpdateSiteCat(oldCatInfo);
            }
        }

        public void LoadBand()
        {
           
        }
    }
}
