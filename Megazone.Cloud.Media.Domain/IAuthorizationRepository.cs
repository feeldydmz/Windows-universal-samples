namespace Megazone.Cloud.Media.Domain
{
    public interface IAuthorizationRepository
    {
        AuthorizationResponse Get(AuthorizationRequest request);

        AuthorizationResponse RefreshAccessCode(AuthorizationRequest request);
        MeResponse GetMe(MeRequest request);
    }
}