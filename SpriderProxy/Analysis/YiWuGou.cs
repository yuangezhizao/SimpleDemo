namespace SpriderProxy.Analysis
{
    public class YiWuGou : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 3 || catId.Length > 15)
                return false;
            return true;
        }
    }
}
