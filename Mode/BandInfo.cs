using System;
using MongoDB.Bson;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class BandInfo
    {
        public BsonObjectId _id { get; set; }
        /// <summary>
        /// 品牌中文名称
        /// </summary>
        public string DisplayName { get; set; }
        public string CnName { get; set; }
        public string EnName { get; set; }
        /// <summary>
        /// 公司网站
        /// </summary>
        public string Domain { get; set; }
        public string LogoImg { get; set; }
        /// <summary>
        /// 正则匹配词
        /// </summary>
        public string RegexWords { get; set; }

        public string Phone { get; set; }
        /// <summary>
        /// 公司简介
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        ///  公司名称
        /// </summary>
        public string Company { get; set; }

        public string Address { get; set; }
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsHid { get; set; }

    }
}
