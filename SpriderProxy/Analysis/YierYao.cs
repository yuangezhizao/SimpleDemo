
namespace SpriderProxy.Analysis
{

    /// <summary>
    /// http://www.12yao.com/
    /// 十二药网
    /// </summary>
    public class YierYao : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            int result;
            return int.TryParse(catId, out result);
        }
    }
}
