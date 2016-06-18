using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mode;
using BLL;
using ServiceStack;

namespace Servers
{
    [Authenticate]
    ///需要验证才能访问
    public class UserServer : Service
    {


      //  public UserInfoRepository Repository { get; set; }

        public object Any(UserInfo request)
        {
            var session = SessionAs<CustomSession>();

            if (session.IsAuthenticated)
            {
                var roles = string.Join(", ", session.Roles.ToArray());
                return new UserResponse { Result = "name: " + request.Name + "role:" + roles };
            }
            else
                return new UserResponse { Result = "name: " + request.Name };
        }
    }
    public class UserResponse
    {
        public string Result { get; set; }
        public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized
    }
    
}
