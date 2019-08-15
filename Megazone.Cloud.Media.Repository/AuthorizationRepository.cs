using System;
using System.Text;
using System.Web;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.IoC;
using RestSharp;

namespace Megazone.Cloud.Media.Repository
{
    [Inject(Source = typeof(IAuthorizationRepository), Scope = LifetimeScope.Transient)]
    public class AuthorizationRepository : IAuthorizationRepository
    {
        public AuthorizationResponse Get(AuthorizationRequest request)
        {
            // 사용자 인증.
            // 인증 URL : https://megaone.io/oauth/token
            var clientAuthorization = GetClientAuthorization();
            var username = HttpUtility.UrlEncode(request.Username);
            var body = $"grant_type=password&username={username}&password={request.Password}";

            var restRequest = new RestRequest("oauth/token", Method.POST)
                .AddHeader("Authorization", $"Basic {clientAuthorization}")
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest)
                .Convert<AuthorizationResponse>();
        }

        private string GetClientAuthorization()
        {
            const string clientId = "0a31e7dc-65eb-4430-9025-24f9e3d7d60d";
            const string clientSecretKey = "02255b95-cb75-4b28-9c0a-30f4afd02f00";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecretKey}"));
        }
    }
}