using System;
using System.Text.RegularExpressions;
namespace SpriderProxy
{
    public abstract class ValidateBase
    {
        private const RegexOptions Ro = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        /// <summary>
        /// 商城分类id 验证
        /// </summary>
        /// <param name="catId">分类id</param>
        /// <returns></returns>
        abstract public bool ValidCatId(string catId);

        /// <summary>
        /// 商城分类验证
        /// </summary>
        /// <param name="catName">分类</param>
        /// <returns></returns>
        abstract public bool ValidCatName(string catName);

        /// <summary>
        /// 产品名称验证
        /// </summary>
        /// <param name="itemName"> 产品名称</param>
        /// <returns></returns>
        abstract public bool ValidItemName(string itemName);
        /// <summary>
        /// 产品url验证
        /// </summary>
        /// <param name="itemurl"> 产品url</param>
        /// <returns></returns>
        abstract public bool ValidItemurl(string itemurl);

        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="regInfo">正则信息 必须带有 "x" </param>
        /// <param name="text">源数据</param>
        /// <returns>-1 表示未匹配到</returns>
        public static T RegGroupsX<T>(string text, string regInfo) where T : IConvertible
        {
            return RegGroups<T>(text, regInfo, "x");
        }

        public static T RegGroups<T>(string text, string regInfo, string paramInfo) where T : IConvertible
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(regInfo) || string.IsNullOrEmpty(paramInfo))
                return default(T);
            try
            {
                if (!Regex.IsMatch(text, regInfo, Ro))
                {
                    return default(T);
                }
                var result = Regex.Match(text, regInfo, Ro).Groups[paramInfo].Value;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public bool regIsMatch(string text, string regInfo)
        {
            return Regex.IsMatch(text, regInfo, Ro);
        }

        public static MatchCollection RegGroupCollection(string connten, string regText)
        {
            if (string.IsNullOrEmpty(connten))
                return null;
            try
            {
                if (!Regex.IsMatch(connten, regText, Ro))
                    return null;
                return Regex.Matches(connten, regText, Ro);
            }
            catch (Exception)
            {

                return null;
            }

        }
    }
}
