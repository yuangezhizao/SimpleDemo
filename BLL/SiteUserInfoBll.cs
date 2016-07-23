using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class SiteUserInfoBll
    {
        public void addUser(List<SiteUserInfo> users)
        {
            new SiteUserInfoDB().addUser(users);
        }

        public List<SiteUserInfo> GetAllUser()
        {
            return new SiteUserInfoDB().GetAllUser();
        }
    }
}
