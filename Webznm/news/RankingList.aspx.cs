using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webznm.news
{
    public partial class RankingList : System.Web.UI.Page
    {
        public string Domain { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            Domain = System.Configuration.ConfigurationManager.AppSettings["domian"];
        }
    }
}