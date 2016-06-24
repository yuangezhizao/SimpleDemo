using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ApiConfig
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppSecret { get; set; }
        public string RequestUrl { get; set; }
        public string Token { get; set; }
        public string CallBackUrl { get; set; }
        public string Summary { get; set; }
        /// <summary>
        /// token 有效期
        /// </summary>
        public DateTime TokenValidity { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
