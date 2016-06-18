namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 我买网
    /// http://www.womai.com/
    /// </summary>
    public class WoMai : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 6 || catId.Length > 15)
                return false;
            return true;
        }
    }
}
