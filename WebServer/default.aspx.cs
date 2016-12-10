using System;
using BLL;
using Commons;
using Mode;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {


                string type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "Token")
                    {
                        var url = new JdServer().GetJdCode();
                        Response.Redirect(url);
                    }
                }
            }
            else
            {

                string key = Request.Form["SearchTxt"];
                if (!string.IsNullOrEmpty(key))
                {
                    string result = new JdServer().GetSingleCode(key);
                    if (result == "商品不在推广中")
                    {
                        string url = key;
                        if (!key.Contains("http"))
                        {
                            url = "http://item.jd.com/" + key + ".html";
                        }

                        lblmsg.Text = "商品不在推广中 <a href='" + url + "'> 去购买 </a>";
                        return;
                    }
                    LogServer.WriteLog("key:" + key + "result:" + result, "Searchlog");
                    if (!string.IsNullOrEmpty(result))
                    {
                        Response.Redirect(result);
                    }

                }
            }
           
            
        }


    }
}