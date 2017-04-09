using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class SmsHistory
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string CaseName { get; set; }
        public string Phone { get; set; }
        public string Summary { get; set; }
        public string MessageInfo { get; set; }
        public string SmsServer { get; set; }
        public string SmsUserName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}