using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using Mode;

namespace WebAppClinet
{
    public partial class SpiderProSet : System.Web.UI.Page
    {
    
        public List<ClassInfo> catList = new List<ClassInfo>() ;//分类绑定
        public StringBuilder SiteList = new StringBuilder();//商城绑定
        public StringBuilder AreaList = new StringBuilder();//地区绑定
        private bool isAllcat = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            catList = new ClassInfoBll().GetAllCatInfo();
            getAreaInfo();
            LoadSitsCat();
            if (!IsPostBack)
            {
                string[] caseType = { "分类更新", "商品列表", "评论更新", "图书更新", "产品内容" };
                for (int i = 0; i < caseType.Length; i++)
                {
                    ddlCaseType.Items.Add(new ListItem(caseType[i], (i + 1).ToString()));
                }
                ddlSort.Items.Add("默认排序");
                ddlSort.Items.Add("价格由低到高");
                txtStartTime.Text = DateTime.Now.Date.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");
                txtStopTime.Text = DateTime.Now.Date.AddHours(22).ToString("yyyy-MM-dd HH:mm:ss");
                catTree.Attributes.Add("onclick", "postBackByObject()");
                catTree.ExpandDepth = 0;
                BindTree(catTree.Nodes, 0);
            }
        }


        /// <summary>
        /// 绑定树控件（递归调用）
        /// </summary>
        /// <param name="tbn"></param>
        /// <param name="parentId"></param>
        private void BindTree(TreeNodeCollection tbn, int parentId)
        {
            TreeNode node;
            var parList = catList.Where(p => p.ParentId == parentId);
            foreach (var item in parList)
            {
                node = new TreeNode(item.CatName, item.Id.ToString());
                node.NavigateUrl = "CategoryDetial.aspx?Id=" + item.Id;
                node.Target = "centerFrame";
                node.Checked = true;
                tbn.Add(node);
                if (item.HasChild)
                {
                    BindTree(node.ChildNodes, item.Id);
                }
            }
        }

        /// <summary>
        /// 根据父节点状态设置子节点的状态
        /// </summary>
        /// <param name="parentNode"></param>
        private void SetChildChecked(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.ChildNodes)
            {
                node.Checked = parentNode.Checked;

                if (node.ChildNodes.Count > 0)
                {
                    SetChildChecked(node);
                }
            }
        }

        protected void catTree_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            SetChildChecked(e.Node);

            // 判断是否是根节点
            if (e.Node.Parent != null)
            {
                e.Node.Parent.Checked = e.Node.Checked;

            }
        }

        private void LoadSitsCat()
        {
            string fmode = "<option value='{0}'>{1}</option>";
            var list = new SiteInfoBll().GetAllSite();
            for (int i = 0; i < list.Count; i++)
            {
                SiteList.Append(string.Format(fmode, list[i].SiteId, list[i].SiteName));
            }
          
        }

        private void getAreaInfo()
        {
            string[] area = {"华北", "华东", "华南", "华中", "西南", "东北", "西北"};
            const string fmode = "<option value='{0}'>{1}</option>";

            for (int i = 0; i < area.Length; i++)
            {
                AreaList.Append(string.Format(fmode, i + 1, area[i]));
            }

        }


        private string getCatid(TreeNode treecon,string catid)
        {
            foreach (TreeNode item in treecon.ChildNodes)
            {
                if (item.ChildNodes.Count > 0)
                {
                   string tempcat= getCatid(item, "");
                   if (tempcat != "")
                   {
                       if (catid != "")
                           catid = catid + "," + tempcat;
                       else
                           catid = tempcat;
                   }
                }
                else if (item.Checked)
                {
                    catid += item.Value + ",";
                }
                else
                {
                    isAllcat = false;
                }
            }
            return catid;

        }


        protected void btnOk_Click(object sender, EventArgs e)
        {
            string sites = Request.Form["sites"];
            string areainfo = Request.Form["hidareas"];
            if (sites == null || areainfo == null)
                return;
            string classes = "";
            foreach (TreeNode item in catTree.Nodes)
            {
                string tempcat = getCatid(item, classes);
                if (tempcat != "")
                {
                    if (classes != "")
                        classes = classes + "," + tempcat;
                    else
                        classes = tempcat;
                }
            }
            if (isAllcat)
                classes = "all";
            SpiderConfig config = new SpiderConfig();
            config.TaskName = txtCatName.Text;
            config.ClassInfoId = classes;
            config.StartTime = DateTime.Parse(txtStartTime.Text);
            config.StopTime =DateTime.Parse(txtStopTime.Text);
            config.TimeSpan = int.Parse(txtTimeSpan.Text);
            config.AreaInfoId = Request.Form["hidareas"];
            config.ClassInfoName = "";
            config.AreaInfoId = areainfo == "" ? "all" : areainfo.TrimEnd(',');
            config.CaseType = int.Parse(ddlCaseType.SelectedValue);
            int maxpage;
            int.TryParse(txtMaxPage.Text, out maxpage);
            config.MaxPage = maxpage;
            int qzsort;
            int.TryParse(txtqz.Text, out qzsort);
            config.Qzsort = qzsort;
            config.SiteSort = ddlSort.SelectedIndex + 1;
            config.SiteInfoId = sites == "" ? "all" : sites.TrimEnd(',');
            config.IsAlive = chkType.Checked;
            config.Detial = txtRemark.Text;
            config.TaskRemark = "";
            new SpiderConfigBll().SaveSpiderConfig(config);
        }
        

    }
}