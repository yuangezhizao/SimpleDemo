namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 唯品会
    /// http://www.vip.com/
    /// </summary>
    public class VipShop : BaseSiteInfo
    {
        /// <summary>
        /// 商城分类id 验证
        /// </summary>
        /// <param name="catId">分类id</param>
        /// <returns></returns>
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length != 8)
                return false;
            return true;
        }
    }
}
