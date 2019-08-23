using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class UpdateVideoParameter: GetVideoParameter
    {
        public UpdateVideoParameter(Authorization authorization, string stageId, string projectId, string videoId) : base(authorization, stageId, projectId, videoId)
        {
        }
    }
}