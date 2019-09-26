namespace Megazone.Cloud.Media.Domain
{
    public interface IAuthorizationRepository
    {
        AuthorizationResponse Get(AuthorizationRequest request);
        MeResponse GetMe(MeRequest request);
    }
}