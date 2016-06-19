
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Webznm
{
    public partial class Index : System.Web.UI.Page
    {
        public static string Domain;
        protected void Page_Load(object sender, EventArgs e)
        {
            //new SiteProInfoBll().getSiteProList();
            if(!IsPostBack)
            {
                if (string.IsNullOrEmpty(Domain))
                {
                    Domain = System.Configuration.ConfigurationManager.AppSettings["domian"];
                }
                BindCat();

            }
        }


        private void BindCat()
        {
            litCatmy.Text = "<ul>" + bingCatByParId(1) + "</ul>";
        
        }


        private string bingCatByParId(int pid)
        {
            string urlpath = Domain + "cat{0}/{1}.html";
            string catMode = "<li class=\" \"><a title='{1}' href=\"{0}\"><span class='menu-title'>{1}</span>{2}</a>{3}</li>";
            string catModeP = "<li class=\"parent dropdown-submenu \"><a class=\"dropdown-toggle\" data-toggle=\"dropdown\" title='{1}' href=\"{0}\"><span class=\"menu-title\">{1}</span>{2}</a>{3}</li>";

            var list = new ClassInfoBll().GetChildCatById(pid);

            StringBuilder res = new StringBuilder();
            foreach (var item in list)
            {
                string HasChild = item.HasChild ? "<b class=\"caret\"></b>" : "";
                string child = "";
                if (item.HasChild)
                {
                    var tempchild = new ClassInfoBll().GetChildCatById(item.Id);
                    StringBuilder childList = new StringBuilder();
                    foreach (var tempitem in tempchild)
                    {
                        string tempurl = string.Format(urlpath, item.Id, HttpUtility.UrlEncode(item.CatName));
                        childList.AppendFormat(catMode, tempurl, tempitem.CatName, "", "");
                    }
                    child = "<div class=\"dropdown-menu level2\"><div class=\"dropdown-menu-inner\"><div class=\"row\"><div class=\"col-sm-12 mega-col\" data-colwidth=\"12\" data-type=\"menu\"><div class=\"mega-col-inner\"><ul>" + childList.ToString() + "</ul></div></div></div></div></div>";

                }
                string purl = string.Format(urlpath, item.Id, HttpUtility.UrlEncode(item.CatName));
                res.Append(string.Format(catModeP, purl, item.CatName, HasChild, child));
            }


        //    <li class=" "><a href="http://demopavothemes.com/pav_styleshop/d/index.php?route=product/category&amp;path=25"><span class="menu-title">Musical Instruments</span></a></li>
            return res.ToString();
        

        }


    }
}