using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class RegProListBll
    {
        public List<RegProListInfo> GetRegProList()
        {
            return new RegProListDB().GetRegProList();
        }
    }
}
