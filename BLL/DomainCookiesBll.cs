using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase;
using Mode;

namespace BLL
{
    public class DomainCookiesBll
    {
        public void SaveCookies(DomainCookies cookies)
        {
            new DomainCookiesDb().SaveCookies(cookies);
        }
        public DomainCookies GetOneByDomain(string domain)
        {
            return new DomainCookiesDb().GetOneByDomain(domain);
        }
    }
}
