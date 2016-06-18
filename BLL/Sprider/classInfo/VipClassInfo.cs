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
    /// <summary>
    /// 唯品会
    /// </summary>
    public class VipClassInfo : VipShop, ISiteClassBLL
    {
        public VipClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(36);
        }
        protected List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://category.vip.com/","utf8",false);

            GetVipCatInfo(directoryHtml);
        }

        private void GetVipCatInfo(string directoryHtml)
        {
            var catjson = RegGroupsX<string>(directoryHtml, "//楼层左侧连接(?<x>.*?)</script>");
            var list = RegGroupCollection(catjson, "\"(?<x>.*?)\"");
            if (list == null)
                return;
            string paraurl = "";
            string paraname = "";
            string paraId = "";
 
            foreach (Match item in list)
            {
                string con = item.Groups["x"].Value;
                string[] tempitemlist = con.Split(',');
                if (tempitemlist.Count() < 2)
                    continue;
                if (tempitemlist[0] == tempitemlist[2])
                {
                    paraurl = ""; paraname = ""; paraId = "";
                }
                string catid = RegGroupsX<string>(tempitemlist[1], "q=\\d+\\|(?<x>\\d+)\\&");
                
                if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo { 
                        ParentClass = paraId, 
                        ParentName = paraname,
                        ClassName=tempitemlist[0],
                        ClassId=catid,
                        ParentUrl=paraurl,
                        IsDel=false,
                        IsBind=false,
                        IsHide=false,
                        BindClassId=0,
                        BindClassName="",
                        HasChild=con.Contains("q=2"),
                        ClassCrumble=paraId,
                        TotalProduct=0,
                        SiteId=Baseinfo.SiteId,
                        Urlinfo= string.Format("http://category.vip.com/search-2-0-1.html?{0}", tempitemlist[1]),
                        UpdateTime=DateTime.Now,
                        CreateDate=DateTime.Now
                    };
                    if (con.Contains("q=2"))
                    {
                        paraurl = string.Format("http://category.vip.com/search-2-0-1.html?{0}", tempitemlist[1]);
                        paraname = tempitemlist[0];
                        paraId = RegGroupsX<string>(paraurl, "q=\\d+\\|(?<x>\\d+)\\&");
                    }
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }



    

                    //getVipSecChild(tempUrl, iteminfo);
                }
                if (shopClasslist.Count > 0)
                {
                    new SiteClassInfoDB().AddSiteClass(shopClasslist);
                    shopClasslist.Clear();
                }



            }
            //MessageBox.Show("抓取完毕");
        //}


        public void UpdateSiteCat()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

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
            string cromb = RegGroupsX<string>(pageinfo, "<!-- 面包屑 -->(?<x>.*?)<!-- 面包屑 end -->");
            var cromblist = RegGroupCollection(cromb, "<p class=\"cat-site-text\">(?<x>.*?)</p>");
            if (cromblist == null)
            {
                cromblist = RegGroupCollection(cromb, "<a(?<x>.*?)</a>");
                
            }
            string paraurl = "";
            string paraname = "";
            string paraId = "";
            for (int i = 0; i < cromblist.Count; i++)
            {
                string text = cromblist[i].Groups["x"].Value;
                string tempName = RegGroupsX<string>(text, ">(?<x>.*?)</a>");
                if (tempName == null)
                {
                    tempName = RegGroupsX<string>(text, ">(?<x>.*?)$");
                }
                if (tempName == null|| tempName.Contains("商品分类"))
                    continue;
                tempName = tempName.Trim();
                string tempUrl = RegGroupsX<string>(text, "href=\"(?<x>.*?)\"");
                string tempid = RegGroupsX<string>(tempUrl, "q=\\d+\\|(?<x>.*?)\\|");


                if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = paraId,
                        ParentName = paraname,
                        ClassName = tempName,
                        ClassId = tempid,
                        ParentUrl = paraurl,
                        IsDel = false,
                        IsBind = false,
                        IsHide = false,
                        BindClassId = 0,
                        BindClassName = "",
                        HasChild = tempUrl.Contains("q=2") || tempUrl.Contains("q=1"),
                        ClassCrumble = "",
                        TotalProduct = 0,
                        SiteId = Baseinfo.SiteId,
                        Urlinfo = string.Format("http://category.vip.com/{0}", tempUrl.TrimStart('/')),
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }
                if (cromblist.Count - 1 == i)
                {
                    siteClassInfo.ParentClass = paraId;
                    siteClassInfo.ParentName = paraname;
                    siteClassInfo.ParentUrl = paraurl;
                    siteClassInfo.UpdateTime = DateTime.Now;
                    siteClassInfo.TotalProduct = RegGroupsX<int>(pageinfo, "count = (?<x>\\d+)");
                    siteClassInfo.ClassName = tempName;
                    new mmbSiteClassInfoDB().UpdateSiteClass(siteClassInfo);
                }
                else
                {
                    paraurl = string.Format("http://category.vip.com/{0}", tempUrl.TrimStart('/'));
                    paraname = tempName;
                    paraId = tempid;
                }


            }

            var childCat = RegGroupsX<string>(pageinfo, "<div class=\"oper-sec-lit f-clearfix J_content J_ctHeight\">(?<x>.*?)</div>");
            if (childCat != null)
            {
                var tempList = RegGroupCollection(childCat, "<a(?<x>.*?)>");
                foreach (Match item in tempList)
                {
                    string catinfo = item.Value;
                    string tempid = RegGroupsX<string>(catinfo, "q=\\d+\\|(?<x>.*?)\\|");
                    if (tempid == null)
                        continue;
                    if (!HasBindClasslist.Exists(c => c.ClassId == tempid))
                    {
                        string tempUrl = RegGroupsX<string>(catinfo, "href=\"(?<x>.*?)\"");
                        string tempName = RegGroupsX<string>(catinfo, "title=\"(?<x>.*?)\"");
                        SiteClassInfo iteminfo = new SiteClassInfo
                        {
                            ParentClass = siteClassInfo.ClassId,
                            ParentName = siteClassInfo.ClassName,
                            ClassName = tempName,
                            ClassId = tempid,
                            ParentUrl = siteClassInfo.Urlinfo,
                            IsDel = false,
                            IsBind = false,
                            IsHide = false,
                            BindClassId = 0,
                            BindClassName = "",
                            HasChild = tempUrl.Contains("q=2") || tempUrl.Contains("q=1"),
                            ClassCrumble = "",
                            TotalProduct = 0,
                            SiteId = Baseinfo.SiteId,
                            Urlinfo = string.Format("http://category.vip.com/{0}", tempUrl.TrimStart('/')),
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


           
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }

    }
}
