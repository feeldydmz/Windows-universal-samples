using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetVideoParameter : RequiredParameter
    {
        public GetVideoParameter(Authorization authorization, string stageId, string projectId, string videoId) : base(
            authorization, stageId, projectId)
        {
            VideoId = videoId;
        }

        public string VideoId { get; }
    }
}