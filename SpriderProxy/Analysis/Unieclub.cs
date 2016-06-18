namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 品致会 127
    /// http://www.unieclub.com/
    /// </summary>
   public class Unieclub:BaseSiteInfo
    {
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 3 || catId.Length > 20)
                return false;
            return true;

        }
    }
}
