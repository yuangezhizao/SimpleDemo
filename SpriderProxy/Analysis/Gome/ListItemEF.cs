using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriderProxy.Analysis.Gome
{
    public class ListItemEF
    {
        public proItem[] products { get; set; }
        //public string recommend { get; set; }
        public PromosEF num { get; set; }
        //public string req { get; set; }
        //public string ip { get; set; }
        public GomeBar pageBar { get; set; }
    }

    public class GomeBar
    {
        public int totalCount { get; set; }
        public int totalPage { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }

    public class proItem
    {
        public string pid { get; set; }
        public string skuid { get; set; }
        public bool isMulti { get; set; }
        public bool XSearch { get; set; }
        public promoEF promo { get; set; }

        public bool isCoo8Product { get; set; }
        public skusEF skus { get; set; }
        public int evaluateCount { get; set; }
        public int shopPic { get; set; }
        public string sName { get; set; }
        public string shopPicUrl { get; set; }
        public string mUrl { get; set; }
        public int salesVolume { get; set; }
        public imageEF[] images { get; set; }
        public PromosEF[] allPromos { get; set; }
        public int stock { get; set; }
        public bool isGomeart { get; set; }
        public int pageNumber { get; set; }
        public bool isBigImg { get; set; }
        public string defCatId { get; set; }
        public int promoFlag { get; set; }
        public int productTag { get; set; }
        
    }

    public class skusEF
    {
        public string skuid { get; set; }
        public string name { get; set; }
        public bool onSale { get; set; }
        public string promoDesc { get; set; }
        public double price { get; set; }
        public string sImg { get; set; }
        public string sUrl { get; set; }
        public string color { get; set; }
        public int stock { get; set; }
        public string partGroup { get; set; }
        public string alt { get; set; }
        public string skuNo { get; set; }
        public string cityName { get; set; }
        public double ShowPrice { get; set; }
        public int gomelow { get; set; }
        
        
    }

    public class PromosEF
    {
        public int totalCount { get; set; }
        public int totalPage { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public bool XSearch { get; set; }
    }
    public class numEF
    {
        public string name { get; set; }
    }

    public class imageEF
    {
         public string pId { get; set; }
         public string skuid { get; set; }
         public string sImg { get; set; }
         public string sUrl { get; set; }
         public string color { get; set; }
    }
    public class promoEF
    {
        public bool pIMGEnabled { get; set; }
    }
}
