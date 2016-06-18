using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Commons;
using DataBase;
using Mode;
using NetDimension.Json.Linq;
using SpriderProxy.Analysis.Gome;
using System;
using System.Collections.Generic;

namespace BLL.Sprider.classInfo
{
    /// <summary>
    /// 国美在线
    /// </summary>
    public class GmClassInfo : Gome, ISiteClassBLL
    {

        public GmClassInfo()
        {
           
            Baseinfo = new SiteInfoDB().SiteById(8);
        }

        private List<SiteClassInfo> HasBindClasslist { get; set; }
        private List<SiteClassInfo> shopClasslist = new List<SiteClassInfo>();
        
        public void SaveAllSiteClass()
        {
            GomeCpsApi gmApi = new GomeCpsApi();
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            gmApi.Format = "json";
            string urlclass = gmApi.GetCategorysurl();

            string catResult = HtmlAnalysis.Gethtmlcode(urlclass);
            catResult = catResult.Substring("{\"categorys\":".Length);
            catResult = catResult.Substring(0, catResult.Length - 1);
            var catList = ServiceStack.Text.JsonSerializer.DeserializeFromString<List<CategoryEF>>(catResult);

            foreach (var cat in catList)
            {
                if (HasBindClasslist.Exists(p => p.ClassId == cat.category_id))
                    continue;

                if (!ValidCatId(cat.category_id))
                {
                    LogServer.WriteLog("ClassId:" + cat.category_id + "验证失败\turl:" + urlclass, "AddClassError");
                    return;
                }

                SiteClassInfo preclassinfo = new SiteClassInfo();
                preclassinfo.ClassName = cat.category_name;
                preclassinfo.ClassId = cat.category_id;
                preclassinfo.SiteId = Baseinfo.SiteId;
                preclassinfo.Urlinfo = "http://www.gome.com.cn/category/" + cat.category_id + ".html";
                if (cat.parent_id != "")
                    preclassinfo.ParentUrl = "http://www.gome.com.cn/category/" + cat.parent_id + ".html";
                if (cat.category_grade == "3")
                    preclassinfo.HasChild = false;
                else
                    preclassinfo.HasChild = true;
                preclassinfo.ParentClass = cat.parent_id;
                preclassinfo.IsDel = false;
                preclassinfo.CreateDate = DateTime.Now;
                preclassinfo.UpdateTime = DateTime.Now;
                HasBindClasslist.Add(preclassinfo);
                shopClasslist.Add(preclassinfo);
                if (shopClasslist.Count > 100)
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
            //getProBySkuid("1122440050");
            HasBindClasslist =
                new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId).OrderBy(p => p.UpdateTime).ToList();

            for (int i = 0; i < HasBindClasslist.Count; i++)
            {
                if (!HasBindClasslist[i].HasChild)
                    continue;
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

        /// <summary>
        /// 获取商城所有的产品
        /// </summary>
        public void LoadProducts()
        {
            LoadUpdateItems();
            return;
            HasBindClasslist = new SiteClassInfoDB().getAllSiteCatInfo(Baseinfo.SiteId);
            if (HasBindClasslist != null)
                foreach (var item in HasBindClasslist.FindAll(p => p.IsHide == false))
                {
                    SaveProByCatid(item.ClassId);
                }
        }

        /// <summary>
        /// 获取商城最近更新的产品
        /// </summary>
        public void LoadUpdateItems()
        {

            GomeCpsApi gmApi = new GomeCpsApi();
            gmApi.UpdateStartDate = DateTime.Now;
            gmApi.Format = "json";
   

            int proTotal = 0;

            do
            {
                string url = gmApi.GetUpdateItemsUrl();
                string catList = HtmlAnalysis.Gethtmlcode(url);
                if (proTotal == 0)
                    proTotal = RegGroupsX<int>(catList, "\"total_count\":(?<x>\\d+)");
                if (catList.IndexOf("items", StringComparison.Ordinal) < 0 || catList.IndexOf('}') == -1)
                {
                    LogServer.WriteLog("apiUrl:" + url + "\tcatList" + catList, "ApiError");
                    gmApi.PageNo++;
                    proTotal = proTotal - gmApi.PageSize;
                    continue;
                }
                catList = catList.Substring(catList.IndexOf("items", StringComparison.Ordinal) + 7).TrimEnd('}');
                var proList = ServiceStack.Text.JsonSerializer.DeserializeFromString<List<ItemEF>>(catList);
                AddProInfo(proList);
                gmApi.PageNo++;
                proTotal = proTotal - gmApi.PageSize;
            } while (proTotal > 0);
            gmApi.PageNo = 1;

        }

        //const string AsynSearchMoth = "http://www.gome.com.cn/p/asynSearch?module=catalog&parentId={1}&cateId={0}&brother=1";
        const string AsynSearchMoth = "http://search.gome.com.cn/cloud/asynSearch?module=catalog&cateId={0}&brother=0";
        
        /// <summary>
        /// 更新类别
        /// </summary>
        /// <param name="catinfo"></param>
        private void UpdateCat(SiteClassInfo catinfo)
        {
            string pageinfo = HtmlAnalysis.Gethtmlcode(catinfo.Urlinfo);
            catinfo.TotalProduct = RegGroupsX<int>(pageinfo, "共(?<x>\\d+)商品|共 <em id=\"searchTotalNumber\">(?<x>\\d+)</em> 个商品");
            if (catinfo.TotalProduct == 0)
                return;

    
            var tempar = HasBindClasslist.FirstOrDefault(c => c.ClassId == catinfo.ParentClass);
            if (tempar != null)
            {
                catinfo.ParentName = tempar.ClassName;
            }
            string tempCatUrl = string.Format(AsynSearchMoth, catinfo.ClassId);
            HtmlAnalysis request = new HtmlAnalysis { RequestMethod = "POST" };
            request.RequestUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0";
            request.RequestAccept = "application/json, text/javascript, */*; q=0.01";
            request.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string catTxt = "";
            int error = 0;
            do
            {
                catTxt = request.HttpRequest(tempCatUrl);
                if (!catTxt.Contains("success"))
                {
                    error++;
                    Thread.Sleep(5000);
                    LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误\r\n" + catTxt, "AddClassError");
                }
                else
                    error = 4;

            } while (error < 3);
            if (string.IsNullOrEmpty(catTxt) || !catTxt.Contains("success"))
                return;
            JObject obj = JObject.Parse(catTxt);
            if (obj == null || obj["response"] == null || obj["response"]["pageJson"] == null ||
                obj["response"]["pageJson"]["content"] == null || obj["response"]["pageJson"]["content"]["catObj"] == null)
            {
                LogServer.WriteLog(Baseinfo.SiteName + "分类抓取错误\r\n" + catTxt, "AddClassError");
                return;
            }

            if (obj["response"]["pageJson"]["content"]["catObj"]["brothers"] != null)
            {
             
                var temp = from item in obj["response"]["pageJson"]["content"]["catObj"]["brothers"]
                    where !HasBindClasslist.Exists(c => c.ClassId == (string) item["catId"])
                    select new SiteClassInfo
                    {
                        ClassId = (string) item["catId"],
                        ParentClass = catinfo.ParentClass ?? "",
                        ClassName = (string) item["catName"],
                        HasChild = true,
                        ParentName = catinfo.ParentName,
                        IsDel = false,
                        IsHide = false,
                        ParentUrl = catinfo.ParentUrl,
                        SiteId = catinfo.SiteId,
                        Urlinfo = string.Format("http://list.gome.com.cn/{0}.html", (string) item["catId"]),
                        CreateDate = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsBind = false
                    };
                var siteClassInfos = temp as SiteClassInfo[] ?? temp.ToArray();
                if (siteClassInfos.Any())
                {
                    shopClasslist.AddRange(siteClassInfos);
                    HasBindClasslist.AddRange(siteClassInfos);
                }
            }

            if (obj["response"]["pageJson"]["content"]["catObj"]["children"] != null)
            {
                var temp = from item in obj["response"]["pageJson"]["content"]["catObj"]["children"]
                           where !HasBindClasslist.Exists(c => c.ClassId == (string)item["catId"])
                           select new SiteClassInfo
                           {
                               ClassId = (string)item["catId"],
                               ClassCrumble = "",
                               BindClassId = 0,

                               ParentClass = catinfo.ClassId,
                               ClassName = (string)item["catName"],
                               HasChild = true,
                               ParentName = catinfo.ClassName,
                               IsDel = false,
                               IsHide = false,
                               ParentUrl = catinfo.Urlinfo,
                               SiteId = catinfo.SiteId,
                               Urlinfo = string.Format("http://list.gome.com.cn/{0}.html", (string)item["catId"]),
                               CreateDate = DateTime.Now,
                               UpdateTime = DateTime.Now,
                               IsBind = false
                           };
                var siteClassInfos = temp as SiteClassInfo[] ?? temp.ToArray();
                if (siteClassInfos.Any())
                {
                    shopClasslist.AddRange(siteClassInfos);
                    HasBindClasslist.AddRange(siteClassInfos);
                }
               
            }
            catinfo.HasChild = HasBindClasslist.Any(c => c.ParentClass == catinfo.ClassId);
            catinfo.IsDel = false;
            catinfo.UpdateTime = DateTime.Now;
            catinfo.ClassName = obj["response"]["pageJson"]["content"]["catObj"]["catName"].ToString();
            catinfo.ClassId = obj["response"]["pageJson"]["content"]["catObj"]["catId"].ToString();
            catinfo.ParentClass = obj["response"]["pageJson"]["content"]["catObj"]["parentId"].ToString();
            catinfo.Urlinfo = string.Format("http://list.gome.com.cn/{0}.html", catinfo.ClassId);
            if (catinfo.ParentClass == "homeStoreRootCategory")
            {
                catinfo.ParentClass = "";
            }
            new SiteClassBll().UpdateSiteCat(catinfo);

            if (shopClasslist.Any())
            {
                new SiteClassInfoDB().AddSiteClass(shopClasslist);
                shopClasslist.Clear();
            }

        }

        /// <summary>
        /// 根据分类全量获取产品
        /// </summary>
        /// <param name="catId"></param>
        private void SaveProByCatid(string catId)
        {
            GomeCpsApi gmApi = new GomeCpsApi();
            gmApi.CategoryId = catId;

            int proTotal = 0;

            do
            {
                string url = gmApi.GetAllItemsUrl();
                string catList = HtmlAnalysis.Gethtmlcode(url);
                if (proTotal == 0)
                    proTotal = RegGroupsX<int>(catList, "\"total_count\":(?<x>\\d+)");
                if (catList.IndexOf("items", StringComparison.Ordinal)<0||catList.IndexOf('}')==-1)
                {
                    LogServer.WriteLog("apiUrl:" + url + "\tcatList" + catList, "ApiError");
                    gmApi.PageNo++;
                    proTotal = proTotal - gmApi.PageSize;
                    continue;
                }
                catList = catList.Substring(catList.IndexOf("items", StringComparison.Ordinal)+7 ).TrimEnd('}');
                var proList = ServiceStack.Text.JsonSerializer.DeserializeFromString<List<ItemEF>>(catList);
                AddProInfo(proList);
                gmApi.PageNo++;
                proTotal = proTotal - gmApi.PageSize;
            } while (proTotal > 0);
            gmApi.PageNo = 1;

        }

        private void getProBySkuid(string skuid)
        {
            GomeCpsApi gmApi = new GomeCpsApi();
            gmApi.SkuId = skuid;
            string url = gmApi.GetAllItemsUrl();
            string catList = HtmlAnalysis.Gethtmlcode(url);
        }

        private void AddProInfo(List<ItemEF> proList)
        {
            if (proList == null || proList.Count == 0)
                return;

            List<SiteProInfo> list = new List<SiteProInfo>();

            foreach (ItemEF item in proList)
            {
                string smallpic = "";
                string otherpic = "";
                if (item.picture_url == null) 
                item.picture_url = "";
                var pictures = item.picture_url.Split(',');
                if (pictures.Length > 0 && item.picture_url != "")
                {
                    string bigpic = pictures[0];
                    smallpic = pictures[0].Replace("_800", "_160");
                    otherpic = item.picture_url.Replace(bigpic, "").TrimStart(',');
                }
                SiteProInfo tempPro = new SiteProInfo
                {
                    SpName=item.sku_name,
                    SpPrice = item.sale_price==0?item.list_price:item.sale_price,
                    ProUrl = item.product_url,
                    smallPic = smallpic,
                    Promotions = item.promo_desc,
                    spBrand = item.brand,
                    SingleDesc=item.service_desc,
                    IsSell=item.stock_status,
                    SiteCat = item.category_id,
                    CommenCount=0,
                    SiteProId=item.product_id,
                    CommentUrl=item.product_url,
                    BigPic = item.picture_url,
                    Otherpic = otherpic,
                    spSkuDes="",
                    SellType=item.operating_model=="1"?1:2,
                    ClassId = 0,
                    SiteId = Baseinfo.SiteId,
                    SiteSkuId = Baseinfo.SiteId + "|" + item.sku_id,
                    FloorPrice = item.sale_price == 0 ? item.list_price : item.sale_price,
                    UpdateTime=DateTime.Now,
                    CreateDate =DateTime.Now
                };
                list.Add(tempPro);
            }

            new SiteProInfoDB().AddSiteProInfo(list);



        }


        public void LoadBand()
        {
            throw new NotImplementedException();
        }
    }
}
