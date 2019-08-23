using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class CreateCaptionAssetParameter
    {
        public CreateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetName, IEnumerable<Caption> captions)
        {
            Authorization = authorization;
            StageId = stageId;
            ProjectId = projectId;
            AssetName = assetName;
            Captions = captions;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string AssetName { get; }
        public IEnumerable<Caption> Captions { get; }
    }
}