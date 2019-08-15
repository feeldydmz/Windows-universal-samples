using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class CaptionListResponse
    {
        public CaptionListResponse(int totalCount, List<Asset> assets)
        {
            TotalCount = totalCount;
            Assets = assets;
        }

        public int TotalCount { get; }
        public List<Asset> Assets { get; }
    }
}