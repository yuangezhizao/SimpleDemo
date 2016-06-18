namespace SpriderProxy.Analysis.Gome
{
    /// <summary>
    /// 产品信息
    /// </summary>
    public class ItemEF
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string sku_id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string sku_name { get; set; }

        /// <summary>
        /// 类目ID
        /// </summary>
        public string category_id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string product_id { get; set; }

        /// <summary>
        /// 经营方式(0联营1自营)
        /// </summary>
        public string operating_model { get; set; }

        /// <summary>
        /// 直销价格
        /// </summary>
        public decimal list_price { get; set; }

        /// <summary>
        /// 折扣价格(折扣时间有效时，价格取该字段)
        /// </summary>
        public decimal sale_price { get; set; }

        /// <summary>
        /// 包装列表 产品清单
        /// </summary>
        public string packing_list { get; set; }

        /// <summary>
        /// 服务描述
        /// </summary>
        public string service_desc { get; set; }

        /// <summary>
        /// 图片地址(逗号分隔)
        /// </summary>
        public string picture_url { get; set; }

        /// <summary>
        /// 库存状态(0无库存1有库存)
        /// </summary>
        public bool stock_status { get; set; }

        /// <summary>
        /// 商品内页
        /// </summary>
        public string product_url { get; set; }

        /// <summary>
        /// 商品页面(手机端)
        /// </summary>
        public string product_url_wap { get; set; }

        /// <summary>
        /// 折扣开始时间
        /// </summary>
        public string on_sale_start_date { get; set; }

        /// <summary>
        /// 折扣结束时间
        /// </summary>
        public string on_sale_end_date { get; set; }

        /// <summary>
        /// 新日期
        /// </summary>
        public string update_date { get; set; }

        /// <summary>
        /// String	品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// 促销语
        /// </summary>
        public string promo_desc { get; set; }



    }
}
