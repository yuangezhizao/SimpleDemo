namespace SpriderProxy.Analysis
{
    /// <summary>
    /// 同康药房网
    /// http://www.tkyfw.com/
    /// </summary>
    public class Tkyfw : BaseSiteInfo
    {
        public override bool ValidCatId(string catid)
        {
            if (string.IsNullOrEmpty(catid))
                return false;
            if (catid.Length >4)
                return false;
            return true;
        }
   
    }
}
