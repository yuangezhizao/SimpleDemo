using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase.Account;
using Mode.account;

namespace BLL.Account
{
   public class UserAccountBll
    {
        public void SaveAccount(UserAccount account)
        {
            try
            {
                new UserAccountDb().SaveAccount(account);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }
        public UserAccount GetById(int id)
        {
            try
            {
                return new UserAccountDb().GetById(id);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }
        }
        public void UpdateAccount(UserAccount account)
        {
            try
            {
                 new UserAccountDb().UpdateAccount(account);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }
    }
}
