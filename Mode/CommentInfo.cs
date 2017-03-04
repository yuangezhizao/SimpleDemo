using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class CommentInfo
    {
        [AutoIncrement]
        public int Id { get; set; }

        public int Spid { get; set; }

        public int Bjid { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     用户评级
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        ///     用户ip地址
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        ///     晒单小图以";"分割
        /// </summary>
        public string ComSmallImg { get; set; }

        /// <summary>
        ///     晒单大图以";"分割
        /// </summary>
        public string CommBigImg { get; set; }

        /// <summary>
        ///     发布时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        ///     分类id
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        ///     品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        ///     品牌id
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>
        ///     商城id
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        ///     是否已删除
        /// </summary>
        public bool IsDel { get; set; }

        public DateTime CreateDate { get; set; }
    }
}