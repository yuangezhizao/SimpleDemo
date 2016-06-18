using System.Collections.Generic;

namespace SpriderProxy.Analysis.Gome
{
    public class GroupPurchaseEF
    {
        public int total_count{ get; set; }

        public List<group_purchase> promo_items { get; set; }
    }
    /// <summary>
    /// 团购信息
    /// </summary>
    public class group_purchase
    {

        public string sku_id { get; set; }
        public string sku_name { get; set; }

        public string start_date { get; set; }
        public string end_date { get; set; }
        public string update_date { get; set; }
        public string promo_id { get; set; }
        public string promo_name { get; set; }
        public decimal promo_price { get; set; }
        public string category_id { get; set; }
        public string product_id { get; set; }
        public string band { get; set; }
        public string product_url { get; set; }
        public string product_url_wap { get; set; }
        public string picture_url { get; set; }
        public string update_status { get; set; }
       
   
    }
}
