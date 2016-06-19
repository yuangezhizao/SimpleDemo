using System;
using BLL;

namespace WebAppClinet
{
    public partial class BindCat : System.Web.UI.Page
    {
        protected string SiteData;
        protected string CatData;
        protected string Msg;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.breadcrumb = "<li><a href='ClassList.aspx'>分类管理</a></li><li class=\"active\">分类绑定</li>";
            SiteData = "''";
            CatData = "''";
            if (!IsPostBack)
            {
                LoadSitsCat();
                if (Request.QueryString["sid"] != null)
                {
                    int sid;
                    BingCat();
                    if (int.TryParse(Request.QueryString["sid"], out sid))
                    {
                        if (ddlParen.Items.FindByValue(sid.ToString()) != null)
                        {
                            ddlParen.Items.FindByValue(sid.ToString()).Selected = true;
                        }
                        SiteData = new SiteClassBll().GetSiteCatJson(sid,1);
                       
                    }

                }
              

                CatData = new ClassInfoBll().GetSiteCatJson(1);
            }
        }

        private void LoadSitsCat()
        {
            var list = new SiteInfoBll().GetAllSite();
            ddlParen.DataSource = list;
            ddlParen.DataTextField = "SiteName";
            ddlParen.DataValueField = "SiteId";
            ddlParen.DataBind();
        }

        private const string StrMode = " <div class='alert alert-success'><i class=\"fa fa-check\"></i>{0}</div>";

        private void BingCat()
        {
            int siteCatId ;
            if (!int.TryParse( Request.QueryString["SiteCat"],out siteCatId))
                return;
             int catid;
             if (int.TryParse(Request.QueryString["Catid"], out catid))
             {
                 Msg = string.Format(StrMode, new SiteClassBll().BingCat(siteCatId, catid));
             }

          
        }

    }
}