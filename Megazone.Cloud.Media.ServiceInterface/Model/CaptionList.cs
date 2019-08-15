using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class CaptionList : PagingResultModel<Asset>
    {
        public CaptionList(int offset, int limitPerPage, int totalCount, IEnumerable<Asset> list) : base(offset,
            limitPerPage, totalCount, list)
        {
        }
    }
}