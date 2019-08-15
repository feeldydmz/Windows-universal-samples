using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetVideoParameter
    {
        public GetVideoParameter(Authorization authorization, string stageId, string projectId, string videoId)
        {
            Authorization = authorization;
            StageId = stageId;
            ProjectId = projectId;
            VideoId = videoId;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string VideoId { get; }
    }
}