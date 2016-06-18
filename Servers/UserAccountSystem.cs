using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.Account;
using Mode.account;

namespace Servers
{
    public class UserAccountSystem
    {
        public void SaveAccount(UserAccount account)
        {
            new UserAccountBll().SaveAccount(account);
        }
        public void AddNewTransaction( UserTransaction transaction)
        {
     
            OrderInfo order = new OrderInfo
            {
                NickName = transaction.NickName,
                UserId = transaction.UserId,
                StockNo = transaction.StockNo,
                StockName = transaction.StockName,
                TotalCount = transaction.StockCount,
                TotalAmount = transaction.Positions,
                OrderStatus = "已下单",
                Transactionfees=transaction.Transactionfees,
                CreateTime=DateTime.Now
            };
            new OrderBll().SaveOrderInfo(order);
            new UserTransactionBll().SaveTransaction(transaction);

        }
    }
}
