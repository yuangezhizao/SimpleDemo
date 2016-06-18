//using System.Collections.Generic;
//using ServiceStack.Configuration;
//using ServiceStack.Text;

//namespace ServiceStack.Authentication.OAuth2
//{
//    public class QQOAuth2Provider : OAuth2Provider
//    {
//        public const string Name = "QQOAuth";

//        public const string Realm = "https://open.t.qq.com/cgi-bin/oauth2/authorize";

//        public QQOAuth2Provider(IAppSettings appSettings)
//            : base(appSettings, Realm, Name)
//        {
//            this.AuthorizeUrl = this.AuthorizeUrl ?? Realm;
//            this.AccessTokenUrl = this.AccessTokenUrl ?? "https://open.t.qq.com/cgi-bin/oauth2/access_token";
//            this.UserProfileUrl = this.UserProfileUrl ?? "https://open.t.qq.com/cgi-bin/oauth2/authorize";

//            if (this.Scopes.Length == 0)
//            {
//                this.Scopes = new[] {
//                    "https://open.t.qq.com/api/REQUEST_METHOD"
//                };
//            }
//        }

//        protected override Dictionary<string, string> CreateAuthInfo(string accessToken)
//        {
//            var url = this.UserProfileUrl.AddQueryParam("access_token", accessToken);
//            string json = url.GetJsonFromUrl();
//            var obj = JsonObject.Parse(json);
//            var authInfo = new Dictionary<string, string>
//            {
//                { "user_id", obj["id"] }, 
//                { "username", obj["email"] }, 
//                { "email", obj["email"] }, 
//                { "name", obj["name"] }, 
//                { "first_name", obj["given_name"] }, 
//                { "last_name", obj["family_name"] },
//                { "gender", obj["gender"] },
//                { "birthday", obj["birthday"] },
//                { "link", obj["link"] },
//                { "picture", obj["picture"] },
//                { "locale", obj["locale"] },
//            };
//            return authInfo;
//        }
//    }
//}
