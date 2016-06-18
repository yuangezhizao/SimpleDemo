//using System.Collections.Generic;
//using ServiceStack.Configuration;
//using ServiceStack.Text;

//namespace ServiceStack.Authentication.OAuth2
//{
//    public class mmbOAuth2Provider : OAuth2Provider
//    {
//        public const string Name = "mmbOAuth";

//        public const string Realm = "http://localhost:8301/OAuth/Authorize";


//        public mmbOAuth2Provider(IAppSettings appSettings)
//            : base(appSettings, Realm, Name)
//        {
//            this.AuthorizeUrl = this.AuthorizeUrl ?? Realm;
//            this.AccessTokenUrl = this.AccessTokenUrl ?? "http://localhost:8301/OAuth/Token";
//            this.UserProfileUrl = this.UserProfileUrl ?? "http://localhost:8301/OAuth/Authorize";

//            if (this.Scopes.Length == 0)
//            {
//                this.Scopes = new[] {
//                    "http://localhost:62330/"
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


//    //public class mmbOAuth2Provider : AuthProvider
//    //{
//    //    public const string Name = "mmbOAuth";

//    //    public const string Realm = "http://localhost:8301/OAuth/Authorize";

//    //    public string AccessTokenUrl { get; set; }

//    //    public IAuthHttpGateway AuthHttpGateway { get; set; }

//    //    public string AuthorizeUrl { get; set; }

//    //    public string ConsumerKey { get; set; }

//    //    public string ConsumerSecret { get; set; }

//    //    public string RequestTokenUrl { get; set; }

//    //    public string UserProfileUrl { get; set; }

//    //    protected string[] Scopes { get; set; }

//    //    private static IAuthorizationState Authorization
//    //    {
//    //        get
//    //        {
//    //            try
//    //            {
//    //                if (HttpContext.Current.Session["Authorization"] != null)
//    //                    return (AuthorizationState)HttpContext.Current.Session["Authorization"];
//    //                else
//    //                    return null;
//    //            }
//    //            catch (Exception)
//    //            {
//    //                return null;
//    //            }
//    //        }
//    //        set { HttpContext.Current.Session["Authorization"] = value; }
//    //    }
//    //    private static AuthorizationServerDescription AuthServerDescription;

//    //    private readonly WebServerClient Client;

//    //    public mmbOAuth2Provider(IAppSettings appSettings)
//    //        : base(appSettings, Realm, Name)
//    //    {
//    //        this.AuthorizeUrl = this.AuthorizeUrl ?? Realm;
//    //        this.AccessTokenUrl = this.AccessTokenUrl ?? "http://localhost:90/OAuth/Token";
//    //        this.UserProfileUrl = this.UserProfileUrl ?? "http://localhost:90/OAuth/Authorize";
//    //        var scopes = appSettings.GetString("oauth.{0}.Scopes".Fmt(Name))
//    //          ?? FallbackConfig(appSettings.GetString("oauth.Scopes")) ?? "";
//    //        this.Scopes = scopes.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
   
//    //        AuthServerDescription = new AuthorizationServerDescription();
//    //        AuthServerDescription.TokenEndpoint = new Uri(AccessTokenUrl);
//    //        AuthServerDescription.AuthorizationEndpoint = new Uri(UserProfileUrl);

//    //        Client = new WebServerClient(AuthServerDescription, "sampleconsumer", "samplesecret");

//    //    }
//    //    public override object Authenticate(IServiceBase authService, IAuthSession session, Authenticate request)
//    //    {
//    //        //有没有访问令牌
//    //        if (Authorization != null)
//    //        {
//    //            return Authorization;
//    //        }
//    //        //申请访问令牌
//    //        IAuthorizationState authorization = Client.ProcessUserAuthorization();
//    //        if (authorization == null)
//    //        {
//    //            //无访问令牌 时 申请授权
//    //            Client.RequestUserAuthorization(Scopes);

//    //            return authorization;
//    //        }

//    //        Authorization = authorization;
//    //        //刷新当前页面
//    //        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Path);
//    //        return Authorization;
//    //    }

//    //    public override bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
//    //    {
//    //        if (Authorization != null)
//    //            return true;
//    //        return false;
//    //    }
//    //}
//}
