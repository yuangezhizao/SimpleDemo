
namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 药房网
    /// http://www.yaofang.cn/
    /// </summary>
    public class Yaofangwang : BaseSiteInfo
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
