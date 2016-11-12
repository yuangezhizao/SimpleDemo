using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons;
using DataBase.Stock;
using Mode;

namespace BLL.Sprider.Stock
{
    public class BondInfoBll
    {
        public void AddBondInfo(List<BondInfo> list)
        {
            try
            {
                new BondInfoDB().AddBondInfo(list);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }
        }

        public List<BondInfo> GetAllBondinfo()
        {
            return new BondInfoDB().GetAllinfo();
        }
    }
}
