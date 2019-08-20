namespace Megazone.Cloud.Media.Domain
{
    public class VideoRequest
    {
        public VideoRequest(string endpoint, string accessToken, string stageId, string projectId, string videoId, Video video = null)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            VideoId = videoId;
            Video = video;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string VideoId { get; }
        public Video Video { get; }
    }
}