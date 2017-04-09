using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class JdJosApi
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string AppKey { get; set; }
        public string AppSecret { get; set; }

        /// <summary>
        ///     授权动态令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     过期时间
        /// </summary>
        public DateTime ExpiresTime { get; set; }

        /// <summary>
        ///     授权时间
        /// </summary>
        public DateTime AccessTime { get; set; }

        /// <summary>
        ///     回调地址
        /// </summary>
        public string BackApiUrl { get; set; }

        public string UserNick { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}