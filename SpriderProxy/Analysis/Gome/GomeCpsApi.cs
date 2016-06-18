/*
 * AppKey：60439208807 
 * AppSecret：7EC9A4FB779F08C613BF06B686C09879 
 */
using System;
using System.Collections.Generic;

namespace SpriderProxy.Analysis.Gome
{

    public class GomeCpsApi : BaseSiteInfo
    {
        public string CpsAppkey { get; set; }

        public  string CpsAppSecret { get; set; }

        public  string CpsDomain { get; set; }
        /// <summary>
        /// json xml 默认为 xml
        /// </summary>
        public string Format { get; set; }



        public string CategoryId { get; set; }

        public string ProductId { get; set; }
        
        public string SkuId { get; set; }
        /// <summary>
        /// 当前页号 默认为1
        /// </summary>
        public int PageNo { get; set; }
        /// <summary>
        /// 每页记录数 默认为1000
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 是否有库存 0 没有 1有 null 为全部
        /// </summary>
        public int? StockStatus { get; set; }

        public DateTime UpdateStartDate { get; set; }
        

        public GomeCpsApi()
        {
            Format = "json";
            //CpsAppkey = "60439208807";
            //CpsAppSecret = "7EC9A4FB779F08C613BF06B686C09879";

            CpsAppkey = "716167";
            CpsAppSecret = "8BDBCA2F3E1619C59083060F1FD3F0BF";
            CpsDomain = "http://open.gome.com.cn/interface/cooperate/gateway?app_key=60439208807&app_secret=7EC9A4FB779F08C613BF06B686C09879&api_name=";
            PageNo = 1;
            PageSize = 1000;
        }

        private Dictionary<string, string> ApiName
        {
            get
            {
                var mathType = new Dictionary<string, string>
                {
                    {"categorys", "gome.categorys.get"},//获取所有类目接口
                    {"items", "gome.items.page.get"},//全量获取产品信息
                    {"UpdateItems", "gome.items.maintain.page.get"},//分页得到商品信息SKU(增量)
                    {"orders", "gome.orders.occur.get"},//分页得到订单发生数
                    {"validorders", "gome.orders.valid.get"},//分页得到订单有效数
                    {"grouppurchase", "gome.grouppurchase.page.get"},//分页得到团购信息
                    {"limitbuyAll", "gome.limitbuy.page.get"},//分页得到抢购信息(全量)
                    {"limitbuyTime", "gome.limitbuy.maintain.page.get"}//分页得到抢购信息(增量)
                };
                return mathType;
            }
        
        }
        /// <summary>
        /// 全量获取分类信息的请求地址
        /// </summary>
        /// <returns></returns>
        public string GetCategorysurl()
        {
            string reqUrl = CpsDomain + ApiName["categorys"];
            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            return reqUrl;
        }
        /// <summary>
        /// 全量获取产品信息的请求地址
        /// </summary>
        /// <returns></returns>
        public  string GetAllItemsUrl()
        {
            string reqUrl = CpsDomain + ApiName["items"];

            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                reqUrl += "&category_id=" + CategoryId;
            }
            if (!string.IsNullOrEmpty(ProductId))
            {
                reqUrl += "&product_id=" + ProductId;
            }
            if (!string.IsNullOrEmpty(SkuId))
            {
                reqUrl += "&sku_id=" + SkuId;
            }
            if (PageNo>1)
            {
                reqUrl += "&page_no=" + PageNo;
            }
            reqUrl += "&page_size=" + PageSize;

            if (StockStatus!=null)
            {
                reqUrl += "&stock_status=" + StockStatus;
            }
            
            return reqUrl;
        }

        /// <summary>
        /// 分页得到商品信息SKU(增量)
        /// </summary>
        /// <returns></returns>
        public string GetUpdateItemsUrl()
        {
            string reqUrl = CpsDomain + ApiName["UpdateItems"];

            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                reqUrl += "&category_id=" + CategoryId;
            }
            if (!string.IsNullOrEmpty(ProductId))
            {
                reqUrl += "&product_id=" + ProductId;
            }
            if (!string.IsNullOrEmpty(SkuId))
            {
                reqUrl += "&sku_id=" + SkuId;
            }
            if (PageNo > 1)
            {
                reqUrl += "&page_no=" + PageNo;
            }
            reqUrl += "&page_size=" + PageSize;

            if (StockStatus != null)
            {
                reqUrl += "&stock_status=" + StockStatus;
            }

            if (UpdateStartDate != DateTime.MinValue)
            {
                reqUrl += "&update_start_date=" + UpdateStartDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return reqUrl;
        }
        /// <summary>
        /// 获取团购信息
        /// </summary>
        /// <returns></returns>
        public string GetGroupPurchase()
        {
            string reqUrl = CpsDomain + ApiName["grouppurchase"];

            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                reqUrl += "&category_id=" + CategoryId;
            }
            if (!string.IsNullOrEmpty(ProductId))
            {
                reqUrl += "&product_id=" + ProductId;
            }
            if (!string.IsNullOrEmpty(SkuId))
            {
                reqUrl += "&sku_id=" + SkuId;
            }
            if (PageNo > 1)
            {
                reqUrl += "&page_no=" + PageNo;
            }
            reqUrl += "&page_size=" + PageSize;

            return reqUrl;
        }

        /// <summary>
        /// 分页得到抢购信息(全量)
        /// </summary>
        /// <returns></returns>
        public string GetLimitbuyUrl()
        {
            string reqUrl = CpsDomain + ApiName["limitbuyAll"];

            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                reqUrl += "&category_id=" + CategoryId;
            }
            if (!string.IsNullOrEmpty(ProductId))
            {
                reqUrl += "&product_id=" + ProductId;
            }
            if (!string.IsNullOrEmpty(SkuId))
            {
                reqUrl += "&sku_id=" + SkuId;
            }
            if (PageNo > 1)
            {
                reqUrl += "&page_no=" + PageNo;
            }
            reqUrl += "&page_size=" + PageSize;
       
            return reqUrl;
        }
        /// <summary>
        /// 分页得到抢购信息(增量)
        /// </summary>
        /// <returns></returns>
        public string GetLimitbuyTimeUrl()
        {
            string reqUrl = CpsDomain + ApiName["limitbuyTime"];

            if (!string.IsNullOrEmpty(Format))
            {
                reqUrl += "&format=" + Format;
            }
            if (!string.IsNullOrEmpty(CategoryId))
            {
                reqUrl += "&category_id=" + CategoryId;
            }
            if (!string.IsNullOrEmpty(ProductId))
            {
                reqUrl += "&product_id=" + ProductId;
            }
            if (!string.IsNullOrEmpty(SkuId))
            {
                reqUrl += "&sku_id=" + SkuId;
            }
            if (PageNo > 1)
            {
                reqUrl += "&page_no=" + PageNo;
            }
            if (UpdateStartDate != DateTime.MinValue)
            {
                reqUrl += "&update_start_date=" + UpdateStartDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            reqUrl += "&page_size=" + PageSize;
       
            return reqUrl;
        }



        public override bool ValidCatId(string catId)
        {
            throw new NotImplementedException();
        }
    }
}
