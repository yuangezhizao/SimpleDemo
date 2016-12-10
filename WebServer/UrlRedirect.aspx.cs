using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebServer
{
    public partial class UrlRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string result="";
                string skuid = Request.QueryString["skuid"];
                if (!string.IsNullOrEmpty(skuid) && skuid.Length > 5 && skuid.Length < 20)
                {
                    int tempid;
                    if (int.TryParse(skuid, out tempid))
                    {
                        result = new JdServer().GetSingleCode(skuid);
                    }
                    if (result == "商品不在推广中")
                    {
                        Response.Redirect("http://item.jd.com/"+ skuid + ".html");
                        return;
                    }
                }
                else
                {
                    string requrl = Request.QueryString["url"];
                    string id = Regex.Match(requrl, "(<x>//d{6,15})", RegexOptions.Singleline).Groups["x"].Value;
                    int tempid;
                    if (int.TryParse(skuid, out tempid))
                    {
                        result = new JdServer().GetSingleCode(id);
                    }
                    if (result == "商品不在推广中")
                    {
                        Response.Redirect(requrl);
                        return;
                    }

                }
           
                if (!string.IsNullOrEmpty(result))
                {
                    Response.Redirect(result);
                }



            }
        }
    }
}