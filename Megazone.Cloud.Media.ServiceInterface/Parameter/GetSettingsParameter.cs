using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetSettingsParameter : RequiredParameter
    {
        public GetSettingsParameter(Authorization authorization, string stageId, string projectId) : base(authorization,
            stageId, projectId)
        {
        }
    }
}