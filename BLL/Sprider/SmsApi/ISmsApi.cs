using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mode;

namespace BLL.Sprider.SmsApi
{
    public interface ISmsApi
    {
        string GetValidateMsg(string phone,string catid);
        SmsManger smsManger { get; set; }
        string GetPhoneNum(string catid);

    }
}
