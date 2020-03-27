using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateCaptionParameter : GetAssetParameter
    {
        public UpdateCaptionParameter(Authorization authorization, string stageId, string projectId,
            string assetId, int assetVersion, Caption caption) : base(authorization, stageId, projectId, assetId)
        {
            Caption = caption;
            AssetVersion = assetVersion;
        }

        public Caption Caption { get; }
        public int AssetVersion { get; }
    }
}