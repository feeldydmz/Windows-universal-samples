using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateAssetParameter<TAsset> : RequiredParameter
    {
        public UpdateAssetParameter(Authorization authorization, string stageId, string projectId, TAsset asset) : base(
            authorization, stageId, projectId)
        {
            Asset = asset;
        }

        public TAsset Asset { get; }
    }
}