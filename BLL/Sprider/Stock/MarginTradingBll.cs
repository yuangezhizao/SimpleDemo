using System.Collections.Generic;
using DataBase.Stock;
using Mode;

namespace BLL.Sprider.Stock
{
    public class MarginTradingBll
    {
        public void AddMarginTrading(List<MarginTrading> list)
        {
            new MarginTradingDb().AddMarginTrading(list);
        }
        public MarginTrading GetNewestTrading(string code)
        {
           return new MarginTradingDb().GetNewestTrading(code);
        }
    }
}
