using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;

namespace Mode
{
    /// <summary>
    /// ip代理信息
    /// </summary>
    public class HostProxy
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Country { get; set; }
        public string Area { get; set; }
        [Index(Unique = true)]
        public string IpAddress { get; set; }
        public string IpPort { get; set; }
        public string HttpType { get; set; }
        public double ConnectionTime { get; set; }
        public double Speed { get; set; }
        public string Niming { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Available { get; set; }

        public bool IsDel { get; set; }
        public DateTime VolidTime { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 总测试次数
        /// </summary>
        public int VolidTotalCount { get; set; }
        /// <summary>
        /// 失败次数
        /// </summary>
        public int VolidFaildCount { get; set; }
        /// <summary>
        /// 数据来源
        /// </summary>
        public string DataSource { get; set; }

    }
}
