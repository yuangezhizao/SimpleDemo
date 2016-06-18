using System.Linq;
using System.Collections.Generic;
using DataBase;
using Mode;

namespace BLL
{
    public class SiteClassBll
    {
        //public List<Mode.SiteClassInfo> HasBindClasslist { get; set; }

        public List<SiteClassInfo> GetClassInfo(int siteId)
        {
            return new SiteClassInfoDB().getAllSiteCatInfo(siteId);
        }
      /// <summary>
      /// 将shop_class_info数据导入到 SiteClass
      /// </summary>
        public void SaveTommbData()
        {
            var site =new SiteClassInfoDB();
            var knowList = site.getAllSiteCatInfo(3);
            var oldList = site.getmmbSiteClass(3);

            var mmbSite = new mmbSiteClassInfoDB();
     

            List<SiteClassInfo> tempList = new List<SiteClassInfo>();

            for (int i = 0; i < oldList.Count; i++)
            {
                if (knowList.Exists(p => p.ClassId == oldList[i].ClassId))
                    continue;
                tempList.Add(oldList[i]);
                if (tempList.Count >= 100)
                {
                    mmbSite.AddSiteClass(tempList);
                    tempList.Clear();
                }
            }

            mmbSite.AddSiteClass(tempList);

        }

        public void UpdatemmbsiteClass()
        {
            int[] siteid = {1,4,8,13,6,11,3};
            for (int i = 0; i < siteid.Length; i++)
            {
                UpdatemmbsiteClass(siteid[i]);
            }
        }

        /// <summary>
        /// 将新抓取的分类 siteClass 保存到 慢慢买 shop_class_info
        /// </summary>
        public void UpdatemmbsiteClass(int siteid)
        {
            var site = new SiteClassInfoDB();
            var knowList = site.getmmbSiteClass(siteid);
            var oldList = site.getAllSiteCatInfo(siteid);
            if (siteid == 190)
            {
                UpdatemmbsiteClass(siteid, "50514008");
                oldList = _tempcatList;
            }

            var mmbSite = new mmbSiteClassInfoDB();


            List<SiteClassInfo> tempList = new List<SiteClassInfo>();

            for (int i = 0; i < oldList.Count; i++)
            {
                //if (oldList[i].UpdateTime.AddDays(3) < System.DateTime.Now)
                //    continue;
                //int classid;
                //if (siteid != 13&&!int.TryParse(oldList[i].ClassId, out classid))
                //    continue;
                if (siteid == 11 && !string.IsNullOrEmpty(oldList[i].ParentClass))
                    oldList[i].ClassId = oldList[i].ParentClass + "t" + oldList[i].ClassId;
                else if (siteid == 8)
                {
                    oldList[i].ClassId = oldList[i].ClassId.Replace("cat", "");
                    if (!string.IsNullOrEmpty(oldList[i].ParentClass))
                        oldList[i].ParentClass = oldList[i].ParentClass.Replace("cat", "");
                }
                //else if (siteid == 13)
                //{
                //    string tempcatid = oldList[i].ClassId.Substring(0, oldList[i].ClassId.IndexOf('-'));
                //    if (knowList.Exists(p => p.ClassId == tempcatid))
                //        continue;
                //}
                else if (siteid == 42)
                {

                    if (knowList.Exists(p => p.ClassId == "0-" + oldList[i].ClassId))
                        continue;
                    oldList[i].ClassId = "100-" + oldList[i].ClassId;
                    if (!string.IsNullOrEmpty(oldList[i].ParentClass))
                    {
                        oldList[i].ParentClass = "100-" + oldList[i].ParentClass;
                    }
                    if (knowList.Exists(p => p.ClassId == oldList[i].ClassId))
                        continue;
                }
                else if (siteid == 43)
                {
                    if (oldList[i].ClassId.Contains('_'))
                    {
                        oldList[i].ClassId = oldList[i].ClassId.Substring(oldList[i].ClassId.IndexOf('_') + 1);
                    }
                    if (oldList[i].ParentClass.Contains('_'))
                    {
                        oldList[i].ParentClass = oldList[i].ParentClass.Substring(oldList[i].ParentClass.IndexOf('_') + 1);
                    }
                }

                else if (siteid == 1)
                {
                    //if (!oldList[i].ClassId.Contains(',') && !oldList[i].ClassId.Contains('-'))
                    //    continue;
                    oldList[i].ClassId = oldList[i].ClassId.Replace(',', '-');
                    if (!string.IsNullOrEmpty(oldList[i].ParentClass))
                        oldList[i].ParentClass = oldList[i].ParentClass.Replace(',', '-');
                }

                if (knowList.Exists(p => p.ClassId == oldList[i].ClassId))
                    continue;
                tempList.Add(oldList[i]);
                if (tempList.Count >= 100)
                {
                    mmbSite.addmmbSIteClass(tempList);
                    tempList.Clear();
                }
            }
            if (tempList.Count > 0)
                mmbSite.addmmbSIteClass(tempList);

        }

        private List<SiteClassInfo> _tempcatList;
        private List<SiteClassInfo> _catList;
        private void UpdatemmbsiteClass(int siteid, string catid)
        {
            if (_catList == null)
            {
                var site = new SiteClassInfoDB();
                _catList = site.getAllSiteCatInfo(10);
            }
            if (_tempcatList == null)
                _tempcatList = new List<SiteClassInfo>();
            foreach (SiteClassInfo siteClassInfo in _catList.Where(c => c.ParentClass == catid))
            {
                siteClassInfo.SiteId = siteid;
                _tempcatList.Add(siteClassInfo);
                if (_catList.Exists(c => c.ParentClass == siteClassInfo.ClassId))
                {
                    UpdatemmbsiteClass(siteid, siteClassInfo.ClassId);
                }
            }




        }


        public void delClass(SiteClassInfo siteclass)
        {
            SiteClassInfoDB db = new SiteClassInfoDB();
            siteclass.IsDel = true;
            db.SetIsDel(siteclass);
        }
        public void UpdateCat(SiteClassInfo siteclass)
        {
             new SiteClassInfoDB().UpdateSiteClass(siteclass);
        }

        public void UnBindCat(int classId)
        {
            new SiteClassInfoDB().UnBindCat(classId);
        }

        List<SiteClassInfo> _list = new List<SiteClassInfo>();
        List<SiteClassInfo> _lessSiteClass = new List<SiteClassInfo>();
        public string GetSiteCatJson(int siteId, int type = 0)
        {
          
            List<BstrapTreeView> node = new List<BstrapTreeView>();
            _list = new SiteClassBll().GetClassInfo(siteId);
            _lessSiteClass = new SiteClassBll().GetClassInfo(siteId);

            if (siteId == 11)
                node = BingNodeByName("", type);
            else
                node = BingNode("", type);
            if (_lessSiteClass.Count > 0)
            {

                List<BstrapTreeView> tempnode = new List<BstrapTreeView>();
                foreach (SiteClassInfo item in _lessSiteClass)
                {
                    BstrapTreeView tempview = new BstrapTreeView{ text=item.ClassName,href=item.Urlinfo};
                    tempview.tags = new List<string>();
                    tempview.tags.Add("<span class=\"badge\">" + item.TotalProduct + "</span>");
                    tempnode.Add(tempview);
                }

                node.Add(new BstrapTreeView { text = "未绑定的分类", href = "#", nodes = tempnode });
            }

            return ServiceStack.Text.JsonSerializer.SerializeToString(node);
        }
        
        public SiteClassInfo GetCatById(int id)
        {
           return new SiteClassInfoDB().GetSiteCatById(id);
        }

        public string BingCat(int id,int catid)
        {
            var siteCat = GetCatById(id);
            if (siteCat == null)
                return "没找到商城分类 id:" + id;
            var cat = new ClassInfoBll().getCat(catid);
            if (cat == null)
                return "没找到分类 catid:" + catid;
            siteCat.BindClassId = catid;
            siteCat.BindClassName = cat.CatName;
            siteCat.IsBind = true;
            var flag = new SiteClassInfoDB().BingCatInfo(siteCat);
            if (flag)
                return "绑定成功";
            return "绑定失败";
        }

        private const string TagFormat = "<span class='glyphicon glyphicon-cog'></span> <a  style='color:#fff' target='_blank' href='ClassDetial.aspx?bathid={0}'>批量导入</a>";
        private const string TagFormatBind = "<span class='glyphicon glyphicon-cog'></span> <a  style='color:#fff' target='_blank' href='javascript:bindCat({0},\"{1}\")'>绑定</a>";


        public void UpdateSiteCat(SiteClassInfo cat)
        {
            if (string.IsNullOrEmpty(cat.ClassId))
                return;
           
            if (string.IsNullOrEmpty(cat.ClassName))
                return;

            new SiteClassInfoDB().updateSpiderOnly(cat);
        }

        /// <summary>
        /// 加载分类
        /// </summary>
        /// <param name="parentCladd"></param>
        /// <param name="type">1为绑定分类 </param>
        /// <returns></returns>
        private List<BstrapTreeView> BingNode(string parentCladd,int type)
        {
            var parList = _list.Where(p => p.ParentClass == parentCladd);
            List<BstrapTreeView> tree = new List<BstrapTreeView>();

            foreach (var item in parList)
            {

                List<string> tag = new List<string>();
                if (type == 1 && !item.IsBind)
                    tag.Add(string.Format(TagFormatBind, item.Id,item.ClassName));
                else
                {
                    if(item.IsBind)
                    {
                        string text = "";
                        if (string.IsNullOrEmpty(item.ClassName))
                            text = item.ClassId;
                        else
                            text = item.BindClassName;
                        tag.Add(string.Format("{0} <a  style='color:#fff' href='javascript:UnbindCat({1})'>取消绑定</a>",text,item.Id));
                    }
                }
                tag.Add(string.Format(TagFormat, item.Id));
                tag.Add("<span class=\"badge\">" + item.TotalProduct + "</span>");
                BstrapTreeView temp = new BstrapTreeView
                {
                    href = item.Urlinfo,
                    text = item.ClassName,
                    tags = tag,
                    Extendible = item.ClassId

                };
                if (item.ClassId != "")
                {
                    if (item.HasChild)
                    {
                        temp.nodes = BingNode(item.ClassId, type);
                    }
                    else if (_list.Exists(p => p.ParentClass == item.ClassId))
                    {
                        temp.nodes = BingNode(item.ClassId, type);
                    }
                    else if (item.ParentClass == "" && _list.Exists(p => p.ParentName == item.ClassName))
                    {
                        temp.nodes = BingNodeByName(item.ClassId, type);
                    }

                }
                else
                {
                    if (item.ParentClass == "" && _list.Exists(p => p.ParentName == item.ClassName))
                    {
                        temp.nodes = BingNodeByName(item.ClassId, type);
                    }
                }
                var tempcat = _lessSiteClass.FirstOrDefault(p => p.Id == item.Id);
                _lessSiteClass.Remove(tempcat);
                tree.Add(temp);
            }

            return tree;
        }

        private List<BstrapTreeView> BingNodeByName(string parentCladd,int type)
        {
            var parList = _list.Where(p => p.ParentName == parentCladd);
            List<BstrapTreeView> tree = new List<BstrapTreeView>();

            foreach (var item in parList)
            {
                List<string> tag = new List<string>();
                if (type == 1 && !item.IsBind)
                    tag.Add(string.Format(TagFormatBind, item.Id,item.ClassName));
                tag.Add(string.Format(TagFormat, item.Id));
                tag.Add("<span class=\"badge\">" + item.TotalProduct + "</span>");
                
              
                BstrapTreeView temp = new BstrapTreeView { text = item.ClassName, href = item.Urlinfo, Extendible = item.ClassId, tags = tag };
                if (item.ClassId == "" && item.ClassName != "")
                {
                    if (item.HasChild)
                    {
                        temp.nodes = BingNodeByName(item.ClassName, type);
                    }
                    else if (_list.Exists(p => p.ParentName == item.ClassName))
                    {
                        temp.nodes = BingNodeByName(item.ClassName, type);
                    }

                }
                else
                {
                    if (item.HasChild)
                    {
                        temp.nodes = BingNode(item.ClassId, type);
                    }
                    else if (_list.Exists(p => p.ParentClass == item.ClassId))
                    {
                        temp.nodes = BingNode(item.ClassId, type);
                    }
                    else if (_list.Exists(p => p.ParentName == item.ClassName))
                    {
                        temp.nodes = BingNodeByName(item.ClassName, type);
                    }
                }
                var tempcat = _lessSiteClass.FirstOrDefault(p => p.Id == item.Id);
                _lessSiteClass.Remove(tempcat);
                tree.Add(temp);
            }

            return tree;
        }


        public List<SiteClassInfo> GetBingCat(string catids,string siteids)
        {
            return new SiteClassInfoDB().GetBindCat("", "8");
        }


        public void UnBingCat(int catid)
        {
            var cat = GetCatById(catid);
            cat.IsBind = false;
            cat.BindClassId = 0;
            cat.BindClassName = "";
            UpdateCat(cat);
        }
    }

   public class BstrapTreeView
    {
        public string text { get; set; }
        public string icon { get; set; }
        public string color { get; set; }
        public string backColor { get; set; }
        public string href { get; set; }
        public List<string> tags { get; set; }
        public bool selectable { get; set; }
        public string Extendible { get; set; }

        public List<BstrapTreeView> nodes { get; set; }
    }

}