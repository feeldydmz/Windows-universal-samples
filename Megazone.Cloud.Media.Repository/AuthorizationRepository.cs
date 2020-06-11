using System;
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
        private const string Domain = "oauth.megazone.io";
        //private const string Domain = "megaone.io";

        public const string LOGIN_URL =
            "https://" + Domain +
            "/oauth/authorize?response_type=code&client_id=0a31e7dc-65eb-4430-9025-24f9e3d7d60d&redirect_uri=https://console.media.stg.continuum.co.kr/megazone/login";

        public AuthorizationResponse Get(AuthorizationRequest request)
        {
            // 사용자 인증.
            // 인증 URL : https://oauth.megazone.io/oauth/token
            var clientAuthorization = GetClientAuthorization(); 
            var content =
                $"code={request.Code}&grant_type=authorization_code&redirect_uri=https://console.media.stg.continuum.co.kr/megazone/login";

            var restRequest = new RestRequest("oauth/token", Method.POST)
                //var restRequest = new RestRequest("v1/token", Method.GET)
                .AddHeader("Authorization", $"Basic {clientAuthorization}")
                .AddParameter("application/x-www-form-urlencoded", content, ParameterType.RequestBody);

            var rest = new RestClient($"https://{Domain}")
            {
                UserAgent = "Hyper series/Megazone.Cloud.Media.SubtitleEditor"
            };

            var exResult = rest.Execute(restRequest);


            //var exResult = RestSharpExtension.CreateRestClient($"https://{Domain}").Execute(restRequest);


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

            var exResult = RestSharpExtension.CreateRestClient($"https://{Domain}").Execute(restRequest);


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

            var restRequest = new RestRequest("resource/v1/me", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}");
            var apiEndpoint = $"https://{Domain}";
            var response = RestSharpExtension.CreateRestClient(apiEndpoint).Execute(restRequest);


            return response.StatusCode == HttpStatusCode.Unauthorized ? null : response.Convert<MeResponse>();
        }

        private string GetClientAuthorization()
        {
            const string clientId = "0a31e7dc-65eb-4430-9025-24f9e3d7d60d";
            const string clientSecretKey = "02255b95-cb75-4b28-9c0a-30f4afd02f00";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecretKey}"));
        }
    }
}