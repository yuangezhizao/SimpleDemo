using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    /// <summary>
    /// 价格更新方案
    /// </summary>
    public class SpiderConfig
    {
        /// <summary>
        /// 方案编号
        /// </summary>
        [AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 商品类目以“,”分隔
        /// </summary>
        public string ClassInfoId { get; set; }
        /// <summary>
        /// 商品类目以“,”分隔
        /// </summary>
        public string ClassInfoName { get; set; }

        public int MaxPage { get; set; }
        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime StopTime { get; set; }
        /// <summary>
        /// 更新地区 0 全部，1北京，2上海，3广州
        /// </summary>
        public string AreaInfoId { get; set; }
        /// <summary>
        /// 商城编号以“,”分隔
        /// </summary>
        public string SiteInfoId { get; set; }

        /// <summary>
        /// 循环更新时间间隔
        /// </summary>
        public int TimeSpan { get; set; }

        public int Qzsort { get; set; }


        public int? SiteSort { get; set; }

        public int CaseType { get; set; }

        public bool IsAlive { get; set; }

        public string Detial { get; set; }

        public string TaskRemark { get; set; }
    }
}
