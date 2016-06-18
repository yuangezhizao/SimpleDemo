using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Sprider.ProList.Api
{
   public interface IApiProList
   {
       /// <summary>
       /// 全量获取商城产品信息
       /// </summary>
       void GetAllProducts();

       void AddNewProducts();
   }
}
