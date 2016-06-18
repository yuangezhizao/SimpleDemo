
namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 聚美优品
    /// http://bj.jumei.com/
    /// </summary>
   public class JuMei : BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length > 6)
                return false;
            int id;
            return int.TryParse(catId, out id);
          
        }
    }
}
