using System;

namespace WebServer
{
    public partial class api : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string code = Request.QueryString["code"];
                if (!string.IsNullOrEmpty(code))
                {
                    Response.Clear();
                    var flage=JdServer.GetAccessToken(code);
                    if(flage)
                        Response.Write("sucessed");
                    else
                        Response.Write("error");
                }
            }

           


        }
    }
}