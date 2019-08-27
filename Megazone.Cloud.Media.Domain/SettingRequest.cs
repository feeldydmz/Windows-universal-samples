namespace Megazone.Cloud.Media.Domain
{
    public class SettingRequest
    {
        public SettingRequest(string endpoint, string accessToken, string stageId, string projectId)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
    }
}