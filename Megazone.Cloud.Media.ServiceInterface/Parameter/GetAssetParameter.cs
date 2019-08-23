using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetAssetParameter : RequiredParameter
    {
        public GetAssetParameter(Authorization authorization, string stageId, string projectId, string assetId) : base(
            authorization, stageId, projectId)
        {
            AssetId = assetId;
        }

        public string AssetId { get; }
    }
}