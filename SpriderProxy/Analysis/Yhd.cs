using System.Collections.Generic;
using Commons;

namespace SpriderProxy.Analysis
{
    /// <summary>
    /// http://www.yhd.com.cn/
    /// 1号网
    /// </summary>
    public class Yhd : BaseSiteInfo
    {
        /// <summary>
        /// 商城分类id 验证
        /// </summary>
        /// <param name="catId">分类id</param>
        /// <returns></returns>
        public override bool ValidCatId(string catId)
        {
            if (string.IsNullOrEmpty(catId))
                return false;
            if (catId.Length < 7 || catId.Length > 15)
                return false;
            return true;
        }

        private const string commentUrl = "http://e.yhd.com/front-pe/productExperience/proExperienceAction!ajaxView_pe.do?product.id={0}&merchantId=1&filter.orderType=newest&pagenationVO.currentPage={1}";
        /// <summary>
        /// 获取评论地址
        /// </summary>
        /// <param name="catid">分类Id</param>
        /// <param name="pageid">页号</param>
        /// <returns></returns>
        public string getCommentUrl(string catid,int pageid)
        {
            return string.Format(commentUrl, catid, pageid);
        }

        public override string Gethtmlcode(string url)
        {
            url = url.Replace("/b/a-s1-v4-p1-price-d0-f0d-m1-rt0-pid-mid0-k/", "")+ "?/?tc=0.0.16.CatMenu_Search_100000024_138034.133&tp=504.1079.159.2.2.LIItAL5-10-Cf7cf&ti=VYG5";
            if (!HtmlAnalysis.Headers.ContainsKey("Cookie"))
                HtmlAnalysis.Headers.Add("Cookie",
                    "abtest=61; newUserFlag=1; guid=D1G6956R3B7EQA7VZEBW69KHN64QNMYR5PGJ; msessionid=2VE7JSMENUS3BR232TU6BBKFFNZXM6N94PT9; provinceId=6; gla=6.50_0_0; gc=153233751; search_keyword_history=; tma=40580330.17126609.1462582423588.1462582423588.1462582423588.1; tmd=3.40580330.17126609.1462582423588.; bfd_g=bb5f92f48bcfd0e3000023af0008f3cb5724720c; search_browse_history=46401719%2C30042648%2C50896847%2C52083124%2C954269%2C4049528%2C39467019%2C51162231%2C52047392%2C53421578; LIAMSoy_R561=x:0.29093|y:0.33490; wide_screen=1; JSESSIONID=508F6DD5B65C8CC85630FEA627EC5CAA");

            string page = base.Gethtmlcode(url);
            if (page.Contains("Error StatusCode"))
            {
                LogServer.WriteLog("网页抓取失败\t"+ page+"\t" + url, "DownLoadError");
                return "";
            }
            if (page.Contains("您的访问过于频繁，可能引起我们的误会"))
            {
                LogServer.WriteLog("网页抓取被屏蔽\t"+url,"DownLoadError");
                return "";
            }
            return page;
        }
    }
}
