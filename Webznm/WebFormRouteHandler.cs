
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

namespace Webznm
{
    public class WebFormRouteHandler : IRouteHandler
    {
        public string VirtualPath { get; private set; }

        public WebFormRouteHandler(string virtualPath)
        {
            this.VirtualPath = virtualPath;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            HttpContext context = HttpContext.Current;
            if (requestContext.RouteData.Values["key"] != null
                && !string.IsNullOrEmpty(requestContext.RouteData.Values["key"].ToString().Trim()))
            {
                string ProductID = HttpUtility.HtmlEncode(requestContext.RouteData.Values["key"].ToString().ToLower());
                context.Items.Add("key", ProductID);
            }
            if (!string.IsNullOrEmpty(VirtualPath))
            {
                var page = BuildManager.CreateInstanceFromVirtualPath(VirtualPath, typeof(Page)) as IHttpHandler;
                return page;
            }
            else
            {
                return BuildManager.CreateInstanceFromVirtualPath("/Default.aspx", typeof(Page)) as IHttpHandler;
            }

        }
    }
}
