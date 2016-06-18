using System;
using ServiceStack.DataAnnotations;
namespace Mode
{
   public class WbUser
    {
        /// <summary>
        /// 用户唯一id，与name相对应,
        /// </summary>
        public string NameTwb { get; set; }
        /// <summary>
        /// 用户唯一id，与name相对应,
        /// </summary>
       public string OpenidTwb { get; set; }
       
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickTwb { get; set; }

        /// <summary>
        /// 头像url
        /// </summary>
        public string HeadTwb { get; set; }

        /// <summary>
        /// 用户性别，1-男，2-女，0-未填写
        /// </summary>
        public string SexTwb { get; set; }

        /// <summary>
        /// 用户发表微博时的所在地
        /// </summary>
        public string LocationTwb { get; set; }

        /// <summary>
        ///  国家码
        /// </summary>
        public string CountryCodeTwb { get; set; }

        /// <summary>
        ///  省份码
        /// </summary>
        public string ProvinceCodeTwb { get; set; }

          /// <summary>
        ///  城市码
        /// </summary>
        public string CityCodeTwb { get; set; }

        /// <summary>
        ///  听众数
        /// </summary>
        public int FansnumTwb { get; set; }

        /// <summary>
        ///  收听数
        /// </summary>
        public int IdolnumTwb { get; set; }

        //    tweet : 用户最近发的一条微博
        //    {
        //        text : 微博内容,
        //        from : 来源,
        //        id : 微博id,
        //        timestamp : 微博时间戳
        //    },
        //    fansnum : 听众数,
        //    idolnum : 偶像数,
        //    isidol : 是否我的偶像，0-不是，1-是,
        //    isvip : 是否名人用户,
        //    tag : 用户标签
        //    {
        //        id : 标签id,
        //        name : 标签名
        //    }
        //}
       

        public string HttpsHead { get; set; }

        public string IsFansTwb { get; set; }

        public string IsidolTwb { get; set; }

        public string IsVip { get; set; }

        public string TagTwb { get; set; }

        public string FromTwb { get; set; }
       [PrimaryKey]
        public string IdTwb { get; set; }

        public string TextTwb { get; set; }

        public string TimestampTwb { get; set; }

        public string IsrealnameTwb { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 微博类型
        /// </summary>
        public string WbType { get; set; }

        public string AccessToken { get; set; }
       /// <summary>
       /// 允许私信
       /// </summary>
        public bool Allowactmsg { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsUsed { get; set; }
       /// <summary>
       /// 创建时间
       /// </summary>
        public string Createdat { get; set; }
        /// <summary>
        /// 等级 数字越大等级越高 
        /// </summary>
        public int Grade{ get; set; }
    }
}
