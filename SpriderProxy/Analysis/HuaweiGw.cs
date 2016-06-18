namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 华为官方网站
    /// http://www.huawei.com/cn/
    /// </summary>
    public class HuaweiGw : BaseSiteInfo
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
