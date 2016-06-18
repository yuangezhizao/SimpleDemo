using Commons;
using Mode;

namespace SpriderProxy
{
    public abstract class BaseSiteInfo : ValidateBase
    {
        public BaseSiteInfo()
        {
            HtmlAnalysis = new HtmlAnalysis();
        }

        public  SiteInfo Baseinfo { get; set; }
        public HtmlAnalysis HtmlAnalysis { get; set; }
        public override bool ValidCatName(string catName)
        {
            if (string.IsNullOrEmpty(catName))
                return false;
            if (catName.Length > 50)
                return false;
            return true;
        }
        /// <summary>
        /// 产品名称验证
        /// </summary>
        /// <param name="catName"></param>
        /// <returns></returns>
        public override bool ValidItemName(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return false;
            if (itemName.Length > 80|| itemName.Length<2)
                return false;
            return true;
        }

        public override bool ValidItemurl(string itemurl)
        {
            if (string.IsNullOrEmpty(itemurl))
                return false;
            if (itemurl.Length > 80 || itemurl.Length < 2)
                return false;
            return true;
        }

        public virtual string Gethtmlcode(string url)
        { 
            return HtmlAnalysis.HttpRequest(url);
        }


    }
}
