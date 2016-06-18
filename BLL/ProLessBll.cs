using DataBase;
using Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class ProLessBll
    {
        public List<ProLessInfo> GetProLessList()
        {
          return  new ProLessDB().GetProLessList();
        }
    }
}
