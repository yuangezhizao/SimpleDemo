using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servers
{
    public class CustomCredentialsAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(IServiceBase authService, string userName, string password)
        {
            if (!CheckInDB(userName, password)) return false;
            var session = (CustomSession)authService.GetSession(false);
            session.CompanyName = "Company from DB";
            session.UserAuthId = userName;
            session.IsAuthenticated = true;

            // add roles 
            session.Roles = new List<string>();
            if (session.UserAuthId == "admin") session.Roles.Add(RoleNames.Admin);
            session.Roles.Add("User");

            return true;
        }

        private bool CheckInDB(string userName, string password)
        {
            if (userName != "admin" && userName != "user") return false;
            return password == "123";
        }
    }

      

    public class CustomSession : AuthUserSession
    {
        public string CompanyName { get; set; }


       
    }
}