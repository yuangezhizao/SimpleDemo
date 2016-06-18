using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL;

namespace Servers
{
    public class SmsServer
    {
        public static List<string> result = new List<string>();
        public static int  TreadCount;
        public static int Total;

        public void Working()
        {

            for (int i = 0; i < TreadCount; i++)
            {
                Task.Factory.StartNew(RegionUser);
            }
        }

        int _currCount;

        private void RegionUser()
        {
            while (_currCount < Total)
            {
                var result = new BenlaiShenhuo().RegionUser();
                if (result == "1")
                    _currCount++;
            }
        }
    }
}
