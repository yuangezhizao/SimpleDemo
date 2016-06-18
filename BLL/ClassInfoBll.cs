using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class ClassInfoBll
    {
        public int AddCat(ClassInfo cat)
        {
            return new ClassInfoDB().AddCatInfo(cat);
        }

        List<ClassInfo> _list = new List<ClassInfo>();

        public string GetSiteCatJson(int type=0)
        {
            List<BstrapTreeView> node = new List<BstrapTreeView>();
            _list = GetAllCatInfo();
            node = BingNode(0, type);
            return ServiceStack.Text.JsonSerializer.SerializeToString(node);
        }

        private const string Tagformat = "<span class='glyphicon glyphicon-cog'></span> <a  style='color:#fff' href='ClassDetial.aspx?catId={0}'>修改</a>";
        private const string Tagbindformat = "<span class='glyphicon glyphicon-cog'></span> <a  style='color:#fff' href='javascript:Bing({0})'>绑定</a>";
        private const string TagDelformat = "<i class=\"fa fa-times\"></i> <a  style='color:#fff' href='javascript:delCat({0})'>删除</a>";

        private List<BstrapTreeView> BingNode(int parentCladd,int type)
        {
            var parList = _list.Where(p => p.ParentId == parentCladd);
            List<BstrapTreeView> tree = new List<BstrapTreeView>();

            foreach (var item in parList)
            {
                List<string> tag = new List<string>();
                if (type == 1)
                    tag.Add(string.Format(Tagbindformat, item.Id));
                else
                {
                    tag.Add(string.Format(TagDelformat, item.Id));
                    tag.Add(string.Format(Tagformat, item.Id));
                }
                BstrapTreeView temp = new BstrapTreeView
                {
                    text = item.CatName,
                    tags = tag,
                    Extendible = item.Id.ToString(CultureInfo.InvariantCulture)
                };
                if (item.HasChild)
                {
                    temp.nodes = BingNode(item.Id, type);
                }
                //else if (_list.Exists(p => p.ParentId == item.Id))
                //{
                //    temp.nodes = BingNode(item.Id);
                //}
                tree.Add(temp);
            }
            return tree;
        }

        public List<ClassInfo> GetAllCatInfo()
        {
            var list = new ClassInfoDB().GetAllCatInfo();
            if (list == null)
                return null;
            return list.Where(p => !p.IsHide&& !p.IsDel).ToList();
        }

        public void UpdateCat(ClassInfo cat)
        {
            new ClassInfoDB().UpdateCatInfo(cat);
        }

        public ClassInfo getCat(int catId)
        {
           return new ClassInfoDB().getCat(catId);
        }
        public List<ClassInfo> GetChildCatById(int id)
        {
            return new ClassInfoDB().GetChildCatById(id);
        }

        /// <summary>
        /// 删除分类包括子分类
        /// </summary>
        /// <param name="cat"></param>
        public void DelCat(ClassInfo cat)
        {
            if (new ClassInfoDB().DelClass(cat))
            {
                //解除绑定
                new SiteClassBll().UnBindCat(cat.Id);
                if (cat.HasChild)
                {
                    DelChildCat(cat);
                }
            }
        }

        private void DelChildCat(ClassInfo oldCat)
        {
            ClassInfoBll catbll = new ClassInfoBll();
            var list = catbll.GetChildCatById(oldCat.Id);
            foreach (var cat in list)
            {
                new ClassInfoDB().DelClass(cat);
                //解除绑定
                new SiteClassBll().UnBindCat(cat.Id);
                if (cat.HasChild)
                {
                    DelChildCat(cat);
                }
            }
        }
    }
}
