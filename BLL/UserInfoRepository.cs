using System.Collections.Generic;
using Mode;
using DataBase;

namespace BLL
{
    public class UserInfoRepository
    {
        public List<UserInfo> GetAllUserInfo()
        {
            return new UserInfoDb().GetAllUserInfo();
        }

        public bool UpdateUserNameById(int userId, string userName)
        {
            UserInfo user = new UserInfoDb().GetUserById(userId);
            user.Name = userName;
            return new UserInfoDb().UpdateUserById(user);
        }

        public bool AddUserInfo(UserInfo user)
        {
            return new UserInfoDb().AddUserInfo(user);
        }
    }
}
