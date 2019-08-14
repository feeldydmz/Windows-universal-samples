namespace Megazone.Cloud.Media.Domain
{
    public class AuthorizationRequest
    {
        public AuthorizationRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}
