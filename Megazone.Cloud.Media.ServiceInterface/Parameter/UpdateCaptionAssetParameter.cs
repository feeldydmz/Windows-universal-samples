using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateCaptionAssetParameter : GetAssetParameter
    {
        public UpdateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetId, IEnumerable<Caption> captions) : base(authorization, stageId, projectId, assetId)
        {
            Captions = captions;
        }

        public IEnumerable<Caption> Captions { get; }
    }
}