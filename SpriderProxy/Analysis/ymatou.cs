namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.ymatou.com/
    /// 洋码头
    /// </summary>
    public class Ymatou : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length > 50)
                return false;
            return true;
        }
    }
}
