namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 顺电
    /// http://www.sundan.com/
    /// </summary>
   public class Sundian : BaseSiteInfo
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
