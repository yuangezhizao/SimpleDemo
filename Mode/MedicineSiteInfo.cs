using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class MedicineSiteInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string CertificateNo { get; set; }
        public string ServerArea { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Gerent { get; set; }
        public string Domian { get; set; }
        public string SiteIp { get; set; }
        public string SiteName { get; set; }
        public string Province { get; set; }
        public DateTime CreateTime { get; set; }

        public DateTime ReleaseTime { get; set; }

        public DateTime ValidityDate { get; set; }
        public string PostNo { get; set; }

        public string Remark { get; set; }
        public bool Usefull { get; set; }
    }
}