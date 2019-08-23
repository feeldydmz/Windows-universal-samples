using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public abstract class RequiredParameter
    {
        protected RequiredParameter(Authorization authorization, string stageId, string projectId)
        {
            Authorization = authorization;
            StageId = stageId;
            ProjectId = projectId;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string ProjectId { get; }
    }
}