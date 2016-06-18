using ServiceStack.DataAnnotations;

namespace Mode
{
    public class TweiboInfo
    {
        /// <summary>
        ///  微博内容
        /// </summary>
        public string TextTwb { get; set; }

        /// <summary>
        ///  原始内容
        /// </summary>
        public string OrigtextTwb { get; set; }

        /// <summary>
        ///  微博被转次数
        /// </summary>
        public int CountTwb { get; set; }

        /// <summary>
        ///  点评次数
        /// </summary>
        public int McountTwb { get; set; }

        /// <summary>
        ///  来源
        /// </summary>
        public int FromTwb { get; set; }

        /// <summary>
        ///   来源url,
        /// </summary>
        public int FromurlTwb { get; set; }

        /// <summary>
        ///  微博唯一id
        /// </summary>
        public string IdTwb { get; set; }

        /// <summary>
        /// 图片url列表
        /// </summary>
        public string ImageTwb { get; set; }

        /// <summary>
        /// 视频短片
        /// {
        ///    picurl : 缩略图,
        ///    player : 播放器地址,
        ///    realurl : 视频原地址,
        ///    shorturl : 视频的短url,
        ///    title : 视频标题
        /// }
        /// </summary>
        public string VideoTwb { get; set; }

        /// <summary>
        /// 音乐
        ///  //{
        ///  //author : 演唱者,
        ///  //url : 音频地址,
        ///  //title : 音频名字，歌名
        ///  //}
        /// </summary>
        public string MusicTwb { get; set; }

        /// <summary>
        /// 发表人帐户名
        /// </summary>
        public string NameTwb { get; set; }

        /// <summary>
        /// 用户唯一id，与name相对应,
        /// </summary>
        [PrimaryKey]
        public string OpenidTwb { get; set; }

        /// <summary>
        /// 发表人昵称
        /// </summary>
        public string NickTwb { get; set; }

        /// <summary>
        /// 是否自已发的的微博，0-不是，1-是,
        /// </summary>
        public bool SelfTwb { get; set; }

        /// <summary>
        /// 是否自已发的的微博，0-不是，1-是,
        /// </summary>
        public string TimestampTwb { get; set; }

        /// <summary>
        /// 博类型，1-原创发表，2-转载，3-私信，4-回复，5-空回，6-提及，7-评论,
        /// </summary>
        public int TypeTwb { get; set; }

        /// <summary>
        /// 发表者头像url
        /// </summary>
        public string HeadTwb { get; set; }


        /// <summary>
        /// 发表者所在地
        /// </summary>
        public string LocationTwb { get; set; }

        /// <summary>
        ///  国家码（其他时间线一样）
        /// </summary>
        public string CountryCodeTwb { get; set; }

        /// <summary>
        ///  省份码（其他时间线一样）
        /// </summary>
        public string ProvinceCodeTwb { get; set; }

        /// <summary>
        ///  城市码
        /// </summary>
        public string CityCodeTwb { get; set; }

        /// <summary>
        ///  是否微博认证用户，0-不是，1-是
        /// </summary>
        public bool IsvipTwb { get; set; }

        /// <summary>
        ///  发表者地理信息
        /// </summary>
        public bool GeoTwb { get; set; }

        /// <summary>
        ///   微博状态，0-正常，1-系统删除，2-审核中，3-用户删除，4-根删除,
        /// </summary>
        public int StatusTwb { get; set; }

        /// <summary>
        ///   心情图片url
        /// </summary>
        public string EmotionurlTwb { get; set; }

        /// <summary>
        ///   心情类型
        /// </summary>
        public string EmotiontypeTwb { get; set; }

        /// <summary>
        ///   当type=2时，source即为源tweet
        /// </summary>
        public int SourceTwb { get; set; }
    }

    //public class TweiboVideo

    //{
    //picurl : 缩略图,
    //player : 播放器地址,
    //realurl : 视频原地址,
    //shorturl : 视频的短url,
    //title : 视频标题
    //}
    //public class TweiboMusic : 
    //{
    //author : 演唱者,
    //url : 音频地址,
    //title : 音频名字，歌名
    //}
}
