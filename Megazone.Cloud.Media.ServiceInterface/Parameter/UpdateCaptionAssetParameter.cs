using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateCaptionAssetParameter : GetAssetParameter
    {
        public UpdateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetId, int version, IEnumerable<Caption> captions) : base(authorization, stageId, projectId,
            assetId)
        {
            Version = version;
            Captions = captions;
        }

        public int Version { get; }

        public IEnumerable<Caption> Captions { get; }
    }
}