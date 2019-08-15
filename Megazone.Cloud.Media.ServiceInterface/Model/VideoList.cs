using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class VideoList : PagingResultModel<Video>
    {
        public VideoList(int offset, int limitPerPage, int totalCount, IEnumerable<Video> list) : base(offset,
            limitPerPage, totalCount, list)
        {
        }
    }
}