namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.baoshuiguoji.com/
    /// 保税国际
    /// </summary>
    public class Baoshuiguoji : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int cid;
            if (!int.TryParse(catId, out cid))
                return false;
            return true;
        }
    }
}
