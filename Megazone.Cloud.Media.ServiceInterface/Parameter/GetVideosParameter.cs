using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetVideosParameter
    {
        public GetVideosParameter(Authorization authorization, string stageId, string projectId, Pagination pagination,
            Dictionary<string, string> searchConditions = null)
        {
            Authorization = authorization;
            StageId = stageId;
            ProjectId = projectId;
            Pagination = pagination;
            SearchConditions = searchConditions;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public Pagination Pagination { get; }
        public Dictionary<string, string> SearchConditions { get; }
    }
}