namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class Authorization
    {
        public Authorization(string accessToken, string refreshToken, string expires)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expires = expires;
        }

        public string AccessToken { get; }
        public string RefreshToken { get; }
        public string Expires { get; }
    }
}
