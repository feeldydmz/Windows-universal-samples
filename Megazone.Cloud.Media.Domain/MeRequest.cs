namespace Megazone.Cloud.Media.Domain
{
    public class MeRequest
    {
        public MeRequest(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }
    }
}