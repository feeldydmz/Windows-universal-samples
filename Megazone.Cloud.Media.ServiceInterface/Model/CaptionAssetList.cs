using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class CaptionAssetList : PagingResultModel<CaptionAsset>
    {
        public CaptionAssetList(int offset, int limitPerPage, int totalCount, IEnumerable<CaptionAsset> list) : base(offset,
            limitPerPage, totalCount, list)
        {
        }
    }
}