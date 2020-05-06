using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class CreateCaptionAssetParameter : RequiredParameter
    {
        public CreateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetName, string folderPath) : base(authorization, stageId, projectId)
        {
            AssetName = assetName;
            FolderPath = folderPath;
        }

        public string AssetName { get; }

        public string FolderPath { get; }
        //public IEnumerable<Caption> Captions { get; }
    }
}