namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 新蛋网
    /// http://www.newegg.cn/
    /// </summary>
    public class NewEgg : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int id;
            if (!int.TryParse(catId, out id))
                return false;
            return true;
        }
    }
}
