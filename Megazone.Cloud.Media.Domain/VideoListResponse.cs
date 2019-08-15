using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class VideoListResponse
    {
        public VideoListResponse(int totalCount, List<Video> videos)
        {
            TotalCount = totalCount;
            Videos = videos;
        }

        public int TotalCount { get; }
        public List<Video> Videos { get; }
    }
}