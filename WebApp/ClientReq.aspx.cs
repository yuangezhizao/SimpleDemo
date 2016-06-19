using ServiceStack;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class ClientReq : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/api";
            //var client = new JsonServiceClient(baseUrl);
            ////var authResponse = client.Post<AuthenticateResponse>("/auth/mmbOAuth");
            //var authResponse = client.Post<AuthenticateResponse>("/auth/mmbOAuth");
         
            //var requestContxt = HttpContext.Current.ToRequest();
         
            //((IHttpResponse)requestContxt.Response).Cookies.AddSessionCookie(SessionFeature.SessionId, authResponse.SessionId);
        
        }

      
    }
}