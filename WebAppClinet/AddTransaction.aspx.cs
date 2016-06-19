using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mode.account;
using Servers;

namespace WebAppClinet
{
    public partial class AddTransaction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserTransaction transaction = new UserTransaction
            {
                UserId = 1,
                NickName = "chennysnow",
                StockNo = "100001",
                StockName = "xxxx",
                StockCount = 100,
                SellType = 1,
                UnitPrice = 7,
                StopLoss = 6,
                StopProfit = 10,
                Transactionfees=5,
                CreateTime = DateTime.Now

            };
            transaction.Positions = transaction.StockCount*transaction.UnitPrice;
            UserAccountSystem system = new UserAccountSystem();
            system.AddNewTransaction(transaction);
        }
    }
}