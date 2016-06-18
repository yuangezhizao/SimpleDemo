using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class SaveSpiderErrorBll
    {
        public void SaveSpiderError(SpiderError error)
        {
            new ErrorInfoDB().SaveSpiderError(error);
        }
    }
}
