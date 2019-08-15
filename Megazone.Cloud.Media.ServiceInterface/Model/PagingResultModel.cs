using System.Collections.Generic;

namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public abstract class PagingResultModel<T>
    {
        protected PagingResultModel(int offset, int limitPerPage, int totalCount, IEnumerable<T> list)
        {
            Offset = offset;
            LimitPerPage = limitPerPage;
            TotalCount = totalCount;
            List = list;
        }

        public int Offset { get; }
        public int LimitPerPage { get; }
        public int TotalCount { get; }
        public IEnumerable<T> List { get; }
    }
}