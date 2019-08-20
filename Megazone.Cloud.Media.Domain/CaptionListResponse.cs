using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class CaptionListResponse
    {
        public CaptionListResponse(int totalCount, IEnumerable<Asset<Caption>> assets)
        {
            TotalCount = totalCount;
            Assets = assets;
        }

        public int TotalCount { get; }
        public IEnumerable<Asset<Caption>> Assets { get; }
    }
}