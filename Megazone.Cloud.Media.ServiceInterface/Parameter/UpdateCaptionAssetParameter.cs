using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateCaptionAssetParameter : GetAssetParameter
    {
        public UpdateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetId, string name, string folderPath, string status) : base(authorization, stageId, projectId, assetId)
        {
            Name = name;
            Status = status;
            FolderPath = folderPath;
        }

        public string Name { get; }
        public string Status { get; }
        public string FolderPath { get; }
    }
}