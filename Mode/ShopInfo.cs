using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ShopInfo
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserMobile { get; set; }
        public string Userqq { get; set; }
        public string UserEmail { get; set; }
        /// <summary>
        /// 商铺编号
        /// </summary>
        public string ShopNo { get; set; }
        public string ShopUserID { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopSummary { get; set; }
        public string ShopUrl { get; set; }
        public string ShopLevel { get; set; }

        public string CatName { get; set; }
        public string CatId { get; set; }
        public string ParentName { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateDate { get; set; }
        

    }
}
