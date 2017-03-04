using System;
using MongoDB.Bson;

namespace Mode
{
    public class SiteBandInfo
    {
        public BsonObjectId _id { get; set; }

        public int SiteId { get; set; }

        public string SiteBandId { get; set; }

        public string DisplayName { get; set; }

        public string CnName { get; set; }

        public string EnName { get; set; }

        public string ImgUrl { get; set; }

        public string Introduction { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsHid { get; set; }

        public string CatArea { get; set; }

        public int TotalProduts { get; set; }

        public int TotalComments { get; set; }

        public string Remark { get; set; }

        public string UniqueKey { get; set; }
    }
}