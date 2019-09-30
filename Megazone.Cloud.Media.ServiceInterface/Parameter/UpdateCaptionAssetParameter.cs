using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateCaptionAssetParameter : GetAssetParameter
    {
        public UpdateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetId, string name) : base(authorization, stageId, projectId, assetId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}