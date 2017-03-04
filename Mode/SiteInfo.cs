using ServiceStack.DataAnnotations;

namespace Mode
{
    /// <summary>
    ///     商城信息
    /// </summary>
    public class SiteInfo
    {
        /// <summary>
        ///     商城id
        /// </summary>
        [PrimaryKey]
        public int SiteId { get; set; }

        /// <summary>
        ///     商城名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        ///     商城logo
        /// </summary>
        public string SiteLogo { get; set; }

        /// <summary>
        ///     域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        ///     商城小logo
        /// </summary>
        public string smallLogo { get; set; }
    }
}