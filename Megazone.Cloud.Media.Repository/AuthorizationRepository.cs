using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.IoC;
using RestSharp;

namespace Megazone.Cloud.Media.Repository
{
    [Inject(Source = typeof(IAuthorizationRepository), Scope = LifetimeScope.Transient)]
    public class AuthorizationRepository : IAuthorizationRepository
    {
#if Dev
        private const string ClientId = "BOgo3m3uS37ZJu5UuQrSwpl45IDchp";
        private const string ClientSecretKey = "aGekFUy7nErDA5e9jQtFDlHSXdvtl9";
        public const string AUTH_REDIRECT_URL = "https://console.cloudplex.dev.megazone.io/megazone/login";
#elif STAGING
        private const string ClientId = "bRq7VLC1RdhUAZC9Dc4LRxfttESyjI";
        private const string ClientSecretKey = "TCz0ZBe8imNZRmoLVP30cAQ7AlAO1H";
        public const string AUTH_REDIRECT_URL = "https://console.cloudplex.stg.megazone.io/megazone/login";
#elif DEBUG
        private const string ClientId = "bRq7VLC1RdhUAZC9Dc4LRxfttESyjI";
        private const string ClientSecretKey = "TCz0ZBe8imNZRmoLVP30cAQ7AlAO1H";
        public const string AUTH_REDIRECT_URL = "https://console.cloudplex.stg.megazone.io/megazone/login";
#elif QA
        private const string ClientId = "p4Eo6DRG9TV0ClWQJEE181nRm7uAzd";
        private const string ClientSecretKey = "LYxZMcb051HDW8YqTNiyykiaOFOoen";
        public const string AUTH_REDIRECT_URL = "https://console.cloudplex.qa.megazone.io/megazone/login";
#else
        private const string ClientId = "d1J99hKQAdWH1C9lKM70QqeCDJI3zu";
        private const string ClientSecretKey = "rXjJLRKgQzzaeDizw6pJz7h63v8MGp";
        public const string AUTH_REDIRECT_URL = "https://console.cloudplex.megazone.io/megazone/login";
#endif
        public const string LOGIN_DOMAIN = "https://login.megazone.com";

        public const string MEGAZONE_API_DOMAIN = "https://www.megazoneapis.com";

        public const string LOGIN_URL =
            LOGIN_DOMAIN +
            "/oauth/authorize?response_type=code&client_id=" + ClientId + "&redirect_uri="+ AUTH_REDIRECT_URL;

        public AuthorizationResponse Get(AuthorizationRequest request)
        {
            // 사용자 인증.
            //var clientAuthorization = GetClientAuthorization(); 
            //var content =
            //    $"code={request.Code}&grant_type=authorization_code&redirect_uri=" + AUTH_REDIRECT_URL;

            //var restRequest = new RestRequest("v2/oauth_token", Method.GET)
            //    //var restRequest = new RestRequest("v1/token", Method.GET)
            //    .AddHeader("Authorization", $"Basic {clientAuthorization}")
            //    .AddParameter("code", request.Code)
            //    .AddParameter("clientId", "d1J99hKQAdWH1C9lKM70QqeCDJI3zu")
            //    .AddParameter("redirectUrl", "https://console.cloudplex.megazone.io/megazone/login");

            //    //.AddParameter("application/x-www-form-urlencoded", content, ParameterType.RequestBody);
            //Debug.WriteLine("--- restRequest : ", restRequest.ToString());

            //var rest = new RestClient($" https://api.cloudplex.megazone.io")
            //{
            //    UserAgent = "Hyper series/Megazone.Cloud.Media.SubtitleEditor"
            //};

            var restRequest = new RestRequest("oauth/token", Method.POST)
                .AddParameter("code", request.Code)
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("client_id", ClientId)
                .AddParameter("client_secret", ClientSecretKey)
                .AddParameter("redirect_uri", AUTH_REDIRECT_URL);

            //.AddParameter("application/x-www-form-urlencoded", content, ParameterType.RequestBody);

            var rest = new RestClient(LOGIN_DOMAIN)
            {
                UserAgent = "CloudPlex series/Megazone.Cloud.Media.SubtitleEditor"
            };

            var exResult = rest.Execute(restRequest);


            //var exResult = RestSharpExtension.CreateRestClient($"https://{LOGIN_DOMAIN}").Execute(restRequest);

            return exResult.Convert<AuthorizationResponse>();
        }


        public AuthorizationResponse RefreshAccessCode(AuthorizationRequest request)
        {
            // 사용자 인증.
            // 인증 URL : https://oauth.megazone.io/oauth/token
            var clientAuthorization = GetClientAuthorization();
            var content =
                $"refresh_token={request.Code}&grant_type=refresh_token";

            var restRequest = new RestRequest("oauth/token", Method.POST)
                //var restRequest = new RestRequest("v1/token", Method.GET)
                .AddHeader("Authorization", $"Basic {clientAuthorization}")
                .AddParameter("application/x-www-form-urlencoded", content, ParameterType.RequestBody);

            var exResult = RestSharpExtension.CreateRestClient(LOGIN_DOMAIN).Execute(restRequest);


            //// TODO 나중에 Get 방식으로 받는 코드로 교체
            //var restRequest = new RestRequest("v1/token", Method.GET)
            //    .AddHeader("origin", "https://console.media.megazone.io")
            //    .AddQueryParameter("code", requestCode)
            //    .AddQueryParameter("platform", "native");

            //var exResult = RestSharpExtension.CreateRestClient("https://api.media.megazone.io").Execute(restRequest);

            //Debug.WriteLine(exResult.StatusCode.ToString());

            //return exResult.Convert<AuthorizationResponse>();


            return exResult.Convert<AuthorizationResponse>();
        }

        public MeResponse GetMe(MeRequest request)
        {
            // API : https://api.media.megazone.io/v1/me

            var restRequest = new RestRequest("userinfo/v1/me", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}");
            
            var response = RestSharpExtension.CreateRestClient(MEGAZONE_API_DOMAIN).Execute(restRequest);

            return response.StatusCode == HttpStatusCode.Unauthorized ? null : response.Convert<MeResponse>();
        }

        private string GetClientAuthorization()
        {
            //const string clientId = "0a31e7dc-65eb-4430-9025-24f9e3d7d60d";
            //const string clientSecretKey = "02255b95-cb75-4b28-9c0a-30f4afd02f00";
            //const string clientId = "d1J99hKQAdWH1C9lKM70QqeCDJI3zu";
            //const string clientSecretKey = "rXjJLRKgQzzaeDizw6pJz7h63v8MGp";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecretKey}"));
        }
    }
}