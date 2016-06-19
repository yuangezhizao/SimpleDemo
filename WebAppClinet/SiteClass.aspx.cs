using System;
using BLL;

namespace WebAppClinet
{
    
    public partial class SiteClass : System.Web.UI.Page
    {
        public string siteData;
        protected void Page_Load(object sender, EventArgs e)
        {
        
            siteData = "''";
            if (!this.Page.IsPostBack)
            {
               // siteData = "[{text: 'Parent 1',nodes:[{text: 'Parent 2'}]},{text: 'Parent 1'},{text: 'Parent 1'}]";
                LoadSitsCat();
                if (Request.QueryString["sid"] != null)
                {
                    int sid;
                    if (int.TryParse(Request.QueryString["sid"], out sid))
                    {
                        if (ddlParen.Items.FindByValue(sid.ToString()) != null)
                        {
                            ddlParen.Items.FindByValue(sid.ToString()).Selected = true;
                        }
                        Unbing(sid);
                        siteData = new SiteClassBll().GetSiteCatJson(sid);
                    }
                }
            }
        }

        private void Unbing(int sid)
        {
            int catid;
            if (int.TryParse(Request.QueryString["catid"], out catid))
            {
                //new SiteClassBll().UnBingCat(catid);
                Response.Redirect("SiteClass.aspx?sid=" + sid);
            }
        }



        private void LoadSitsCat()
         {
             var list = new SiteInfoBll().GetAllSite();
             ddlParen.DataSource = list;
             ddlParen.DataTextField = "SiteName";
             ddlParen.DataValueField = "SiteId";
             ddlParen.DataBind();
             //siteData = new SiteClassBll().GetSiteCatJson(1);
         }

    
    }

   


}