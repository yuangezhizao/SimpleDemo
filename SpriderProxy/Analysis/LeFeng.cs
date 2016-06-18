namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 乐峰网
    /// http://www.lefeng.com/
    /// </summary>
    public class LeFeng : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 4 || catId.Length > 20)
                return false;
            return true;
        }
    }
}
