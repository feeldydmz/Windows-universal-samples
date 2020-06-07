using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class CreateAssetElementBulkParameter : GetAssetParameter
    {
        public CreateAssetElementBulkParameter(Authorization authorization, string stageId, string projectId,
            string assetId, int assetVersion, IEnumerable<Caption> elements) : base(authorization, stageId, projectId, assetId)
        {
            Elements = elements;
            AssetVersion = assetVersion;
        }

        public IEnumerable<Caption> Elements { get; }
        public int AssetVersion { get; }
    }
}
