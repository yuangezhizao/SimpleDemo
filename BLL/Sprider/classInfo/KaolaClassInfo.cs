using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis;
namespace BLL.Sprider.classInfo
{
    public class KaolaClassInfo : Kaola, ISiteClassBLL
    {
        public KaolaClassInfo()
        {
            Baseinfo = new SiteInfoDB().SiteById(241);
        }
        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        public void SaveAllSiteClass()
        {
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            string directoryHtml = HtmlAnalysis.Gethtmlcode("http://www.kaola.com/getFrontCategory.html");

            var catinfo = JObject.Parse(directoryHtml);

            var list= catinfo["frontCategoryList"];
            foreach (var fitem in list)
            {
                string categoryId = fitem["categoryId"].Value<string>();
                string categoryName = fitem["categoryName"].Value<string>();
                string url = $"http://www.kaola.com/category/{categoryId}.html";
                if (!HasBindClasslist.Exists(c => c.ClassId == categoryId))
                {
                    SiteClassInfo iteminfo = new SiteClassInfo
                    {
                        ParentClass = "",
                        ParentName = "",
                        ClassName = categoryName,
                        ClassId = categoryId,
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
                        Urlinfo = url,
                        UpdateTime = DateTime.Now,
                        CreateDate = DateTime.Now
                    };
                    HasBindClasslist.Add(iteminfo);
                    shopClasslist.Add(iteminfo);
                }


                var firstChild = fitem["childrenNodeList"];
                if(firstChild==null)
                    continue;
                foreach (var fchild in firstChild)
                {
                    string fcategoryId = fchild["categoryId"].Value<string>();
                    string fcategoryName = fchild["categoryName"].Value<string>();
                    string furl = $"http://www.kaola.com/category/{fcategoryId}.html";
                    if (!HasBindClasslist.Exists(c => c.ClassId == fcategoryId))
                    {
                        SiteClassInfo iteminfo = new SiteClassInfo
                        {
                            ParentClass = categoryId,
                            ParentName = categoryName,
                            ClassName = fcategoryName,
                            ClassId = fcategoryId,
                            ParentUrl =url,
                            IsDel = false,
                            IsBind = false,
                            IsHide = false,
                            BindClassId = 0,
                            BindClassName = "",
                            HasChild = true,
                            ClassCrumble = "",
                            TotalProduct = 0,
                            SiteId = Baseinfo.SiteId,
                            Urlinfo = furl,
                            UpdateTime = DateTime.Now,
                            CreateDate = DateTime.Now
                        };
                        HasBindClasslist.Add(iteminfo);
                        shopClasslist.Add(iteminfo);
                    }

                    var secChild = fchild["childrenNodeList"];
                    if (secChild == null)
                        continue;
                    foreach (var sitem in secChild)
                    {
                        if(sitem["categoryId"] ==null)
                            continue;
                        string scategoryId = sitem["categoryId"].Value<string>();
                        string sfcategoryName = sitem["categoryName"].Value<string>();
                        string sfurl = $"http://www.kaola.com/category/{fcategoryId}/{scategoryId}.html";
                        if (!HasBindClasslist.Exists(c => c.ClassId == scategoryId))
                        {
                            SiteClassInfo iteminfo = new SiteClassInfo
                            {
                                ParentClass = fcategoryId,
                                ParentName = fcategoryName,
                                ClassName = sfcategoryName,
                                ClassId = fcategoryId + "/"+ scategoryId,
                                ParentUrl = furl,
                                IsDel = false,
                                IsBind = false,
                                IsHide = false,
                                BindClassId = 0,
                                BindClassName = "",
                                HasChild = false,
                                ClassCrumble = "",
                                TotalProduct = 0,
                                SiteId = Baseinfo.SiteId,
                                Urlinfo = sfurl,
                                UpdateTime = DateTime.Now,
                                CreateDate = DateTime.Now
                            };
                            HasBindClasslist.Add(iteminfo);
                            shopClasslist.Add(iteminfo);
                        }

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
            throw new NotImplementedException();
        }

        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
