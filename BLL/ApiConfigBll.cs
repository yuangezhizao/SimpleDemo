using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
   public class ApiConfigBll
    {
       public void SaveApiConfig(ApiConfig api)
       {
            new ApiConfigDB().SaveApiConfig(api);
       }

       public List<ApiConfig> GetAllConfig()
       {
           return new ApiConfigDB().GetAllConfig();
       }
    }
}
