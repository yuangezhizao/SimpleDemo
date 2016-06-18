namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 老板电器
    /// http://www.robam.com/
    /// </summary>
    public class ShopRobam : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int id;
            return int.TryParse(catId, out id);
        }
    }
}
