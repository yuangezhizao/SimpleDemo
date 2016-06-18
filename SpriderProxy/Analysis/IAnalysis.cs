using System.Collections.Generic;
using Mode;

namespace SpriderProxy.Analysis
{
    interface IAnalysis
    {
        List<SiteClassInfo> GetChildClassByUrl(string url);

        string GetSiteClassIdByUrl(string url);
    }
}
