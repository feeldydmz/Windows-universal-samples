using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class CaptionList : PagingResultModel<Asset<Caption>>
    {
        public CaptionList(int offset, int limitPerPage, int totalCount, IEnumerable<Asset<Caption>> list) : base(offset,
            limitPerPage, totalCount, list)
        {
        }
    }
}