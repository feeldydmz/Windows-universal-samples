namespace Megazone.Cloud.Media.Domain
{
    public class DeleteAssetRequest : AssetRequest
    {
        public DeleteAssetRequest(string endpoint, string accessToken, string stageId, string projectId, string assetId,
            int version) : base(endpoint, accessToken, stageId, projectId, assetId)
        {
            Version = version;
        }

        public int Version { get; }
    }
}