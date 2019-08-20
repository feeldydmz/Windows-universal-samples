using Megazone.Cloud.Media.Domain.Assets;
using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class AssetListResponse<TAsset>
    {
        public AssetListResponse(int totalCount, IEnumerable<TAsset> assets)
        {
            TotalCount = totalCount;
            Assets = assets;
        }

        public int TotalCount { get; }
        public IEnumerable<TAsset> Assets { get; }
    }
}