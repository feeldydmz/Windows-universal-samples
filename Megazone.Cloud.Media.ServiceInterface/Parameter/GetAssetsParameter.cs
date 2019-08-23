using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetAssetsParameter : RequiredParameter
    {
        public GetAssetsParameter(Authorization authorization, string stageId, string projectId, Pagination pagination,
            Dictionary<string, string> searchConditions = null) : base(authorization, stageId, projectId)
        {
            Pagination = pagination;
            SearchConditions = searchConditions;
        }

        public Pagination Pagination { get; }

        public Dictionary<string, string> SearchConditions { get; }
    }
}