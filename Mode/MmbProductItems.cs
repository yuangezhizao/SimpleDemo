using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class MmbProductItems
    {
        [AutoIncrement]
        public int Id { get; set; }

        public int classid { get; set; }

        public int siteid { get; set; }

        public string spurl { get; set; }

        public string spprice { get; set; }

        public decimal? spMoney { get; set; }

        public string spname { get; set; }

        public string spkey { get; set; }

        public DateTime? date2 { get; set; }

        public bool notice { get; set; }

        public bool isdel { get; set; }

        public int spid { get; set; }

        public string sppic { get; set; }

        public bool? isSell { get; set; }

        public int? commentCount { get; set; }

        public string commentUrl { get; set; }

        public DateTime? commentUpdate { get; set; }

        public int? stateid { get; set; }

        public string spurl2 { get; set; }

        public string pingpai { get; set; }

        public int? pipeinum { get; set; }

        public string spkey2 { get; set; }

        public int? jiangjia { get; set; }

        public int? ppid { get; set; }

        public string s1 { get; set; }

        public string s2 { get; set; }

        public string shopID { get; set; }

        public string bigpic { get; set; }

        public string spbh { get; set; }

        public decimal? spMoney2 { get; set; }

        public bool? isUpdate { get; set; }

        public long? spbhInt { get; set; }

        public int? oldCommentCount { get; set; }

        public int? f1id { get; set; }

        public int? f2id { get; set; }

        public int? f3id { get; set; }

        public int? f4id { get; set; }

        public int? f5id { get; set; }

        public int? f6id { get; set; }

        public string youhui { get; set; }

        public decimal? oldprice { get; set; }

        public DateTime? updatetime { get; set; }

        public bool? isjiangjia { get; set; }

        public string siteclass { get; set; }

        public int? diqu { get; set; }

        public string searchfield { get; set; }

        public string commentHistory { get; set; }

        public int? monthComments { get; set; }

        public int? pageid { get; set; }

        public int? qzsort { get; set; }

        public int? ziying { get; set; }

        public int? pageSeat { get; set; }

        public decimal? minMoney { get; set; }

        public bool? IsYouXian { get; set; }

        public string areaPriceInfo { get; set; }

        public string hostIP { get; set; }

        public string CaseName { get; set; }

        public string monitor { get; set; }
    }
}
