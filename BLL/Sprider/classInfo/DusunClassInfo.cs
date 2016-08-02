using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataBase;
using Mode;
using SpriderProxy.Analysis;
using Commons;
namespace BLL.Sprider.classInfo
{
    public class DusunClassInfo:Dusun, ISiteClassBLL
    {
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        public DusunClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(293);
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }

        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);

           string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.dusun.com.cn/form_mobile/menu/index.jsp");
            // string directoryHtml=  DocumentServer.ReadFileInfo(@"E:\aaa.txt");
            GetCatInfo(directoryHtml);
        }

        private void GetCatInfo(string directoryHtml)
        {
            string area = RegGroupsX<string>(directoryHtml, "楼层json数据(?<x>.*?)初始化页面时显示最后一次点击的楼层分类");
            if (string.IsNullOrEmpty(area))
                return;
            var list = RegGroupCollection(area, "var floor\\d+ = (?<x>.*?);");
            if (list == null || list.Count == 0)
                return;
            foreach (Match match in list)
            {
                
               
                var item = match.Groups["x"].Value.Replace("'", "").Replace("+", "");
                if (item.Contains("floor4"))
                {
                    item = match.Groups["x"].Value.Replace("'", "\"").Replace("+", "");
                }


                var secitem = RegGroupCollection(item, "\"productCategory\":\\[(?<x>.*?)\\]");
                if(secitem==null||secitem.Count==0)
                    continue;

                foreach (Match secmth in secitem)
                {
                    string parentUrl = RegGroups<string>(secmth.Groups["x"].Value, "\\s*{\"url\":\"(?<x>.*?)\",\"name\":\"(?<y>.*?)\",\\s*\"productItems\"", "x");
                    if (string.IsNullOrEmpty(parentUrl))
                        continue;
                    string parentName = RegGroups<string>(secmth.Groups["x"].Value, "\\s*{\"url\":\"(?<x>.*?)\",\"name\":\"(?<y>.*?)\",\\s*\"productItems\"", "y");
                    if (string.IsNullOrEmpty(parentName))
                        continue;
                    string parentId = RegGroupsX<string>(parentUrl, "class-(?<x>\\d+)-");
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
                    string sonarea = RegGroupsX<string>(secmth.Groups["x"].Value, "\"productItem\":\\[(?<x>.*?)$");
                    var sonlist = RegGroupCollection(sonarea, "\"url\":\"(?<x>.*?)\",\"img\":\".*?\",\"name\":\"(?<y>.*?)\"");
                    if (sonlist == null || sonlist.Count == 0)
                        continue;

                    foreach (Match mch in sonlist)
                    {
                        string caturl = mch.Groups["x"].Value;
                        if (string.IsNullOrEmpty(caturl))
                            continue;
                        string catName = mch.Groups["y"].Value;
                        if (string.IsNullOrEmpty(catName))
                            continue;
                        if (catName == parentName)
                            continue;
                        string catid = RegGroupsX<string>(caturl, "class-(?<x>\\d+)-");
                        if (!HasBindClasslist.Exists(c => c.ClassId == catid))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = parentId,
                                ParentName = parentName,
                                ClassName = catName,
                                ClassId = catid,
                                ParentUrl = parentUrl,
                                IsDel = false,
                                IsBind = false,
                                IsHide = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = true,
                                ClassCrumble = "",
                                TotalProduct = 0,
                                SiteId = Baseinfo.SiteId,
                                Urlinfo = caturl,
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


        }
        public void UpdateSiteCat()
        {
            throw new NotImplementedException();
        }
    }
}
