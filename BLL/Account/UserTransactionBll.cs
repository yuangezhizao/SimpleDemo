using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase.Account;
using Mode.account;

namespace BLL.Account
{
   public class UserTransactionBll
    {
        public void SaveTransaction(UserTransaction transaction)
        {
            try
            {
                new UserTransactionDb().SaveTransaction(transaction);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }

        }
        public UserTransaction GetById(int id)
        {
            try
            {
                return new UserTransactionDb().GetById(id);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }
        }
        public List<UserTransaction> GetTransactionByOrderid(int orderid)
        {
            try
            {
                return new UserTransactionDb().GetTransactionByOrderid(orderid);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
                return null;
            }
        }
        public void UpdateOrderInfo(UserTransaction transaction)
        {
            try
            {
                new UserTransactionDb().UpdateOrderInfo(transaction);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBError");
            }
        }
    }
}
