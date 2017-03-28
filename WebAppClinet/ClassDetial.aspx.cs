using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BLL;
using Commons;
using Mode;

namespace WebAppClinet
{
    public partial class ClassDetial : System.Web.UI.Page
    {   
        //ddd
        protected StringBuilder ParaCatInfo = new StringBuilder();
        protected string SiteData;
        protected string SiteCatName;
        protected int CatId;
        protected int ParCatId;
        ///修改分类绑定
        protected string oldCatName;
        protected string oldSort;
        protected string oldSeo;
        protected string oldHide;

        private static List<ClassInfo> catList;
        private const string Mode = "<option value=\"{0}\">{1}</option>";
        private const string Mode1 = "<option selected='selected' value=\"{0}\">{1}</option>";
        private const string Mode2 = "<li><a href='ClassList.aspx'>分类管理</a></li><li class=\"active\">{0}</li>";

        protected string addform = "style='display:none'";
        protected string bathform = "style='display:none'";
        protected string updateform = "style='display:none'";
        private List<SiteClassInfo> AllSiteCat;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["bathid"] != null)
                {
                    bathform = "";
                    Master.breadcrumb = string.Format(Mode2, "批量添加分类");
                    bathAddCat(Request.QueryString["bathid"]);
                }
                else if (Request.QueryString["catId"] != null)
                {
                    updateform = "";
                    int.TryParse(Request.QueryString["catId"], out CatId);
                    Master.breadcrumb = string.Format(Mode2, "修改分类");
                    UpdateCat(CatId);
                    BingOldCat(CatId);
                }
                else if (Request.QueryString["delId"] != null)
                {
                    int delId;
                    if (!int.TryParse(Request.QueryString["delId"], out delId))
                        return;
                    DelCat(delId);
                }
                else
                {
                    addform = "";
                    Master.breadcrumb = string.Format(Mode2, "添加分类");
                    addCat();
                }
                loadCatinfo();
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

        private void UpdateCat(int catId)
        {
   
            int parentId;
            if (!int.TryParse(Request.Form["parCatName"], out parentId))
                return;
            int catSort;
            int.TryParse(Request.Form["catSort"], out catSort);
              
            ClassInfoBll catbll = new ClassInfoBll();
            var oldCat = new ClassInfoBll().getCat(catId);
            if (oldCat == null)
                return;
            oldCat.UpdateTime = DateTime.Now;
         
            oldCat.SpellWord = new WordCenter().GetPyString(oldCat.CatName);
            oldCat.SEOWords = Request.Form["seoKey"];
            oldCat.Sort = catSort;
            if (oldCat.ParentId == parentId && oldCat.CatName == Request.Form["catName"])
            {
                catbll.UpdateCat(oldCat);
                return;
            }

            oldCat.CatName = Request.Form["catName"];
            if (parentId != 0)
            {
                ClassInfo parCat = catbll.getCat(parentId);
                oldCat.ParentId = parCat.Id;
                oldCat.Level = parCat.Level + 1;
                oldCat.ParentName = parCat.CatName;
                if (!string.IsNullOrEmpty(parCat.CatCrumbleIds))
                {
                    oldCat.CatCrumbleIds = parCat.CatCrumbleIds + "," + parCat.Id;
                    oldCat.CatCrumbleNames = parCat.CatCrumbleNames + "," + parCat.CatName;
                }
                else
                {
                    oldCat.CatCrumbleIds = parCat.Id.ToString(CultureInfo.InvariantCulture);
                    oldCat.CatCrumbleNames = parCat.CatName;
                }
                if (!parCat.HasChild)
                {
                    parCat.HasChild = true;
                    catbll.UpdateCat(parCat);
                }
            }
            else
            {
                oldCat.ParentId = 0;
                oldCat.Level = 1;
                oldCat.ParentName = "";
                oldCat.CatCrumbleIds = "";
                oldCat.CatCrumbleNames = "";
            }
            catbll.UpdateCat(oldCat);

            if (oldCat.HasChild)
            {
                UpdateChildCat(oldCat);
            }


        }

        private void UpdateChildCat(ClassInfo oldCat)
        {
            ClassInfoBll catbll = new ClassInfoBll();
            var list = catbll.GetChildCatById(oldCat.Id);
            foreach (var cat in list)
            {
                cat.Level = oldCat.Level + 1;
                cat.ParentName = oldCat.CatName;
                if (!string.IsNullOrEmpty(oldCat.CatCrumbleIds))
                {
                    cat.CatCrumbleNames = oldCat.CatCrumbleNames + "," + oldCat.CatName;
                    cat.CatCrumbleIds = oldCat.CatCrumbleIds + "," + oldCat.Id;
                }
                else
                {
                    cat.CatCrumbleIds = oldCat.Id.ToString(CultureInfo.InvariantCulture);
                    cat.CatCrumbleNames = oldCat.CatName;
                }
                cat.UpdateTime = DateTime.Now;
                catbll.UpdateCat(cat);
                if (cat.HasChild)
                {
                    UpdateChildCat(cat);
                }
            }
    
        }

        private void BingOldCat(int catId)
        {
            var oldCat = new ClassInfoBll().getCat(catId);
            if (oldCat == null)
                return;
       
            oldCatName = oldCat.CatName;
            oldSeo = oldCat.SEOWords;
            oldSort = oldCat.Sort == 0 ? "" : oldCat.Sort.ToString(CultureInfo.InvariantCulture);
            ParCatId = oldCat.ParentId;
            if (oldCat.IsHide)
            {
                oldHide = "checked='checked'";
            }
        }
        /// <summary>
        /// 批量添加分类
        /// </summary>
        /// <param name="catid"></param>
        private void bathAddCat(string catid)
        {
            SiteClassBll bll = new SiteClassBll();
            
            int siteClassId;
            int parentId;
            SiteClassInfo siteCat = new SiteClassInfo();
            if (int.TryParse(catid, out siteClassId))
            {
                siteCat = bll.GetCatById(siteClassId);
                SiteCatName = siteCat.ClassName.Replace(" ","");
            }
            if (!int.TryParse(Request.Form["parCatName"], out parentId))
                return;
            #region 添加分类
            ClassInfo cat = new ClassInfo();
            cat.CatName = SiteCatName;
            cat.SpellWord = WordCenter.GetShortPinyin(cat.CatName);
            cat.SEOWords = "";
            cat.Sort = 0;
            cat.CreateDate = DateTime.Now;
            cat.UpdateTime = DateTime.Now;
            cat.HasChild = false;
            ClassInfoBll catbll = new ClassInfoBll();
            if (parentId != 0)
            {
                ClassInfo parCat = catbll.getCat(parentId);
                cat.ParentId = parCat.Id;
                cat.Level = parCat.Level + 1;
                cat.ParentName = parCat.CatName;
                if (!string.IsNullOrEmpty(parCat.CatCrumbleIds))
                {
                    cat.CatCrumbleIds = parCat.CatCrumbleIds + "," + parCat.Id;
                    cat.CatCrumbleNames = parCat.CatCrumbleNames + "," + parCat.CatName;
                }
                else
                {
                    cat.CatCrumbleIds = parCat.Id.ToString(CultureInfo.InvariantCulture);
                    cat.CatCrumbleNames = parCat.CatName;
                }
                if (!parCat.HasChild)
                {
                    parCat.HasChild = true;
                    catbll.UpdateCat(parCat);
                }
            }
            else
            {
                cat.ParentId = 0;
                cat.Level = 1;
                cat.ParentName = "";
                cat.CatCrumbleIds = "";
                cat.CatCrumbleNames = "";
            }
            cat.Id = catbll.AddCat(cat);  
            #endregion
            #region 添加子分类

            AllSiteCat = bll.GetClassInfo(siteCat.SiteId);

            addChildCat(siteCat.ClassId, cat);

            #endregion
        }
        /// <summary>
        /// 下拉框分类绑定
        /// </summary>
        private void loadCatinfo()
        {
            catList = new ClassInfoBll().GetAllCatInfo();
            BingNode(0);
        }
        /// <summary>
        /// 下拉框分类绑定
        /// </summary>
        /// <param name="pid"></param>
        private void  BingNode(int pid)
        {
            SiteData = new ClassInfoBll().GetSiteCatJson();
            var parList = catList.Where(p => p.ParentId == pid);
     
            foreach (var item in parList)
            {
                string level = "";
                for (int i = 1; i < item.Level; i++)
                {
                    level += "&nbsp; &nbsp; &nbsp;";
                }
                if (item.Id == ParCatId && ParCatId > 0)
                    ParaCatInfo.Append(string.Format(Mode1, item.Id, level + item.CatName));
                else
                    ParaCatInfo.Append(string.Format(Mode, item.Id, level + item.CatName));
                if (item.HasChild)
                    BingNode(item.Id);
            }

        }
        /// <summary>
        /// 添加分类
        /// </summary>
        private void addCat()
        {
            if (Request.Form.Count < 3 || string.IsNullOrEmpty(Request.Form["catName"]) ||
                string.IsNullOrEmpty(Request.Form["parCatName"]))
                return;
           
            int parentId;
            if (!int.TryParse(Request.Form["parCatName"], out parentId))
                return;

            int catSort;
            int.TryParse(Request.Form["catSort"], out catSort);

            ClassInfo cat = new ClassInfo();
            cat.CatName = Request.Form["catName"];
            cat.SpellWord = new WordCenter().GetPyString(cat.CatName);
            cat.SEOWords = Request.Form["seoKey"];
            cat.Sort = catSort;
            cat.CreateDate = DateTime.Now;
            cat.UpdateTime = DateTime.Now;
            cat.HasChild = false;
            cat.IsDel = false;
            ClassInfoBll catbll = new ClassInfoBll();
            if (parentId != 0)
            {
                ClassInfo parCat = catbll.getCat(parentId);
                cat.ParentId = parCat.Id;
                cat.Level = parCat.Level + 1;
                cat.ParentName = parCat.CatName;
                if (!string.IsNullOrEmpty(parCat.CatCrumbleIds))
                {
                    cat.CatCrumbleIds = parCat.CatCrumbleIds + "," + parCat.Id;
                    cat.CatCrumbleNames = parCat.CatCrumbleNames + "," + parCat.CatName;
                }
                else
                {
                    cat.CatCrumbleIds = parCat.Id.ToString(CultureInfo.InvariantCulture);
                    cat.CatCrumbleNames = parCat.CatName;
                }
                if (!parCat.HasChild)
                {
                    parCat.HasChild = true;
                    catbll.UpdateCat(parCat);
                }
            }
            else
            {
                cat.ParentId = 0;
                cat.Level = 1;
                cat.ParentName = "";
                cat.CatCrumbleIds = "";
                cat.CatCrumbleNames = "";
            }
            catbll.AddCat(cat);
        }

        /// <summary>
        /// 添加商城对应的子分类
        /// </summary>
        /// <param name="siteCatId"></param>
        /// <param name="parCat"></param>
        private void addChildCat(string siteCatId, ClassInfo parCat)
        {
            var ChildCat = AllSiteCat.Where(p => p.ParentClass == siteCatId);
            foreach (var child in ChildCat)
            {
                ClassInfo cat = new ClassInfo();
                cat.CatName = child.ClassName.Replace(" ","");
                cat.SpellWord = WordCenter.GetShortPinyin(cat.CatName);
                cat.SEOWords = "";
                cat.Sort = 0;
                cat.CreateDate = DateTime.Now;
                cat.UpdateTime = DateTime.Now;
                cat.HasChild = false;
                ClassInfoBll catbll = new ClassInfoBll();
                if (parCat != null)
                {
                    cat.ParentId = parCat.Id;
                    cat.Level = parCat.Level + 1;
                    cat.ParentName = parCat.CatName;
                    if (!string.IsNullOrEmpty(parCat.CatCrumbleIds))
                    {
                        cat.CatCrumbleIds = parCat.CatCrumbleIds + "," + parCat.Id;
                        cat.CatCrumbleNames = parCat.CatCrumbleNames + "," + parCat.CatName;
                    }
                    else
                    {
                        cat.CatCrumbleIds = parCat.Id.ToString(CultureInfo.InvariantCulture);
                        cat.CatCrumbleNames = parCat.CatName;
                    }
                    if (!parCat.HasChild)
                    {
                        parCat.HasChild = true;
                        catbll.UpdateCat(parCat);
                    }
                    cat.Id = int.Parse(catbll.AddCat(cat).ToString());
                    if (AllSiteCat.Exists(p => p.ParentClass == child.ClassId))
                    {
                        addChildCat(child.ClassId, cat);
                    }
                }
            }

        }
    }
}