using System.Collections.Generic;

namespace SpriderProxy.Analysis.Gome
{
    public class PromoResponseEF
    {
        public int total_count{ get; set; }

        public List<promo_items> promo_items { get; set; }
    }

    public class promo_items
    {
        public int promo_id { get; set; }
        public string promo_name { get; set; }
        public string sku_id { get; set; }
        public string sku_name { get; set; }
        public string category_id { get; set; }
        public string product_id { get; set; }
        public decimal promo_price { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string picture_url { get; set; }
        public string update_date { get; set; }
        public string update_status { get; set; }
        public string product_url { get; set; }
        public string product_url_wap { get; set; }
    }
}
