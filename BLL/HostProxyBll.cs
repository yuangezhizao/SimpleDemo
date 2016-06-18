using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
   public class HostProxyBll
    {
       public void SaveHostProxy(HostProxy host)
       {
           new HostProxyDb().SaveHostProxy(host);
       }

       public void SaveHostProxy(List<HostProxy> list)
       {
           HostProxyDb db = new HostProxyDb();
           List<HostProxy> temp = new List<HostProxy>();

           foreach (var item in list)
           {
               if (!db.Exist(item))
                   temp.Add(item);
           }
           if (temp.Any())
           new HostProxyDb().SaveHostProxy(temp);
       }

       public List<HostProxy> loadHostProxy()
       {
           return new HostProxyDb().LoadHostProxy();
       }

       public void UpdateHostProxy(HostProxy host)
       {
           new HostProxyDb().UpdateHostProxy(host);
       }

       public void DelHostProxy(HostProxy hostProxy)
       {
           new HostProxyDb().DelHostProxy(hostProxy);
       }
    }
}
