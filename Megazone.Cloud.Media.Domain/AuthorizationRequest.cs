namespace Megazone.Cloud.Media.Domain
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string endpoint, string username, string password)
        {
            Endpoint = endpoint;
            Username = username;
            Password = password;
        }

        public string Endpoint { get; }
        public string Username { get; }
        public string Password { get; }
    }
}
