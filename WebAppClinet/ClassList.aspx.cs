using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;

namespace WebAppClinet
{
    public partial class ClassList : System.Web.UI.Page
    {
        protected string siteData;
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.breadcrumb = "<li class=\"active\">分类管理</li>";
            siteData = new ClassInfoBll().GetSiteCatJson();

            if (!IsPostBack && Request.QueryString["delId"] != null)
            {
                int delId;
                if (!int.TryParse(Request.QueryString["delId"], out delId))
                    return;
                DelCat(delId);
            }
        }
        private void DelCat(int catid)
        {
            ClassInfoBll catbll = new ClassInfoBll();
            var cat = new ClassInfoBll().getCat(catid);
            if (cat == null)
                return;

            catbll.DelCat(cat);
        }

    }
}