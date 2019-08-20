using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class VideoListRequest
    {
        public VideoListRequest(string endpoint, string accessToken, string stageId, string projectId, Pagination pagination, Dictionary<string, string> searchConditions = null)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            Pagination = pagination;
            SearchConditions = searchConditions ?? new Dictionary<string, string>();
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public Pagination Pagination { get; }
        public Dictionary<string, string> SearchConditions { get; }
    }
}