namespace Megazone.Cloud.Media.Domain
{
    public class AssetRequest
    {
        public AssetRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            ProjectId = projectId;
            AssetId = assetId;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetId { get; }
    }

    public class AssetRequest<TAsset> : AssetRequest
    {
        public AssetRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId, TAsset asset) : base(endpoint, accessToken, stageId, projectId, assetId)
        {
            Asset = asset;
        }
        public TAsset Asset { get; }
    }
}