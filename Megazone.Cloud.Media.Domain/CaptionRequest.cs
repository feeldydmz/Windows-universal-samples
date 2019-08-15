namespace Megazone.Cloud.Media.Domain
{
    public class CaptionRequest
    {
        public CaptionRequest(string endpoint, string accessToken, string stageId, string projectId, string captionId)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            CaptionId = captionId;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string CaptionId { get; }
    }
}