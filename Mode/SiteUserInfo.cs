using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;
namespace Mode
{
    public class SiteUserInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string Domain { get; set; }
        public string SiteName { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string PayPwd { get; set; }
        public string PhoneNum { get; set; }
        public string EmailName { get; set; }
        public string EmailPwd { get; set; }
        public string AddJdid { get; set; }
        public string AddJdCode { get; set; }
        public string AddJdDetial { get; set; }
        public string Eid { get; set; }
        public string Fp { get; set; }
        public string UserAgent { get; set; }
        public string LoginCookies { get; set; }

        public DateTime LoginUpdatetime { get; set; }

        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
    }
}