using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webznm.news
{
    public partial class RankingDetial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
                if (Page.RouteData.Values["num"] != null)
                {
                    Response.Write(Page.RouteData.Values["num"]);
                }
                if (Page.RouteData.Values["title"] != null)
                {
                    Response.Write(Page.RouteData.Values["title"]);
                }
                Literal1.Text = Page.RouteData.Values["num"].ToString();
            //}
        }
    }
}