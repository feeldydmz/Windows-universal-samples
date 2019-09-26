namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetLanguageParameter
    {
        public GetLanguageParameter(string authorizationAccessToken, string stageId, string projectId)
        {
            AuthorizationAccessToken = authorizationAccessToken;
            StageId = stageId;
            ProjectId = projectId;
        }

        public string AuthorizationAccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
    }
}