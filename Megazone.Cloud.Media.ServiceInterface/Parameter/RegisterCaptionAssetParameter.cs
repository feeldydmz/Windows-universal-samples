using System.Collections.Generic;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class RegisterCaptionAssetParameter : RequiredParameter
    {
        public RegisterCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string videoId, int videoVersion, IEnumerable<string> assetIds) : base(authorization, stageId, projectId)
        {
            VideoId = videoId;
            VideoVersion = videoVersion;
            AssetIds = assetIds;
        }

        public string VideoId { get; }
        public int VideoVersion { get; }
        public IEnumerable<string> AssetIds { get; }
    }
}