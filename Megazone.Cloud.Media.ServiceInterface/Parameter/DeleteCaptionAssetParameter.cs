using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class DeleteCaptionAssetParameter : RequiredParameter
    {
        public DeleteCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string captionAssetId, int version) : base(authorization, stageId, projectId)
        {
            CaptionAssetId = captionAssetId;
            Version = version;
        }

        public string CaptionAssetId { get; }
        public int Version { get; }
    }
}