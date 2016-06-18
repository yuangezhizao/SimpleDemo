using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.WeiBo
{
    public interface IWeiboBll
    {
        void write(string contents,string pic);

        void getAccessToken(string code);

    }
}
