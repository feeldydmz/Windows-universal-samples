
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class CreateAssetElementParameter : GetAssetParameter
    {
        public CreateAssetElementParameter(Authorization authorization, string stageId, string projectId,
            string assetId, int assetVersion, Caption element) : base(authorization, stageId, projectId, assetId)
        {
            Element = element;
            AssetVersion = assetVersion;
        }

        public Caption Element { get; }
        public int AssetVersion { get; }
    }
}
