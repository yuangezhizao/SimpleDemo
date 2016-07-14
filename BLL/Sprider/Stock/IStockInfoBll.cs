using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Sprider.Stock
{
    public interface IStockInfoBll
    {
        void GetALlStockInfo();
        void DayReport();
    }
}
