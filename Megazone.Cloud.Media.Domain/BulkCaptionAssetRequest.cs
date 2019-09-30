using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class BulkCaptionAssetRequest
    {
        public BulkCaptionAssetRequest(string endpoint, string accessToken, string stageId, string projectId,
            string videoId, int videoVersion, IEnumerable<string> assetIds)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            VideoId = videoId;
            VideoVersion = videoVersion;
            AssetIds = assetIds;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string VideoId { get; }
        public int VideoVersion { get; }
        public IEnumerable<string> AssetIds { get; }
    }
}