using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class CaptionListRequest
    {
        public CaptionListRequest(string endpoint, string accessToken, string stageId, string projectId,
            int offset = 0,
            int limitPerPage = 10, Dictionary<string, string> searchConditions = null)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            Offset = offset;
            LimitPerPage = limitPerPage;
            SearchConditions = searchConditions ?? new Dictionary<string, string>();
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public int Offset { get; }
        public int LimitPerPage { get; }
        public Dictionary<string, string> SearchConditions { get; }
    }
}