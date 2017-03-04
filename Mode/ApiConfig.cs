using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class ApiConfig
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string ApiId { get; set; }
        public string ApiSupplier { get; set; }
        public string ApiUserName { get; set; }
        public string ApiUserPwd { get; set; }
        public string ApiSecret { get; set; }
        public string RequestUrl { get; set; }
        public string Token { get; set; }
        public string CallBackUrl { get; set; }
        public string Summary { get; set; }

        /// <summary>
        ///     token 有效期
        /// </summary>
        public DateTime TokenValidity { get; set; }

        public DateTime UpdateTime { get; set; }
        public DateTime CreateDate { get; set; }
    }
}