namespace Megazone.Cloud.Media.Domain
{
    public class MeRequest
    {
        public MeRequest(string endpoint, string accessToken)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
    }
}
