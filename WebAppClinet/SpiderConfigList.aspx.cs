using System;
using BLL;

namespace WebAppClinet
{
    public partial class SpiderConfigList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBing();
        }

        private void DataBing()
        {
            SpiderConfigBll spbll = new SpiderConfigBll();
            var list = spbll.LoadSpiderconfig();
            repConfigList.DataSource = list;
            repConfigList.DataBind();

        }
    }
}