using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Webznm
{
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.Add("ProductDetail", new Route("Product/{key}.htm", new WebFormRouteHandler("/Products/ProductDetial.aspx")));
            //routes.Add("Productlist", new Route("Products/{class}.htm", new WebFormRouteHandler("/Products/ProductList.aspx")));
            routes.Add("NewsDetial", new Route("Ranking/{title}/{num}", new WebFormRouteHandler("/news/RankingDetial.aspx")));
            routes.Add("toplist", new Route("toplist", new WebFormRouteHandler("/news/RankingList.aspx")));
            routes.Add("Catcategory1", new Route("cat{a}/{b}", new WebFormRouteHandler("/Products.aspx")));
            routes.Add("Catcategory", new Route("cat{a}/{b}.html", new WebFormRouteHandler("/Products.aspx")));
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    "Default", // 路由名称
            //    "{controller}/{action}/{id}", // 带有参数的 URL
            //    new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            //);
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}