using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetCaptionParameter
    {
        public GetCaptionParameter(Authorization authorization, string stageId, string projectId, string assetId)
        {
            Authorization = authorization;
            StageId = stageId;
            ProjectId = projectId;
            AssetId = assetId;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetId { get; }
    }
}