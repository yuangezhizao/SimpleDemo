namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 康爱多
    /// http://www.360kad.com/
    /// </summary>
    public class Kad : BaseSiteInfo
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
