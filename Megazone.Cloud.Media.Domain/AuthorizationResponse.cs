// ReSharper disable InconsistentNaming
namespace Megazone.Cloud.Media.Domain
{
    public class AuthorizationResponse
    {
        public AuthorizationResponse(string access_token, string token_type, string refresh_token, string expires_in, string scope)
        {
            AccessToken = access_token;
            TokenType = token_type;
            RefreshToken = refresh_token;
            Expires = expires_in;
            Scope = scope;
        }

        public string AccessToken { get; }
        public string TokenType { get; }
        public string RefreshToken { get; }
        public string Expires { get; }
        public string Scope { get; }
    }
}
