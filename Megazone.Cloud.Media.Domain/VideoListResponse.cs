using System.Collections.Generic;

namespace Megazone.Cloud.Media.Domain
{
    public class VideoListResponse
    {
        public VideoListResponse(int statusCode, int totalCount, List<Video> videos)
        {
            StatusCode = statusCode;
            TotalCount = totalCount;
            Videos = videos;
        }

        public int StatusCode { get; }
        public int TotalCount { get; }
        public List<Video> Videos { get; }
    }
}