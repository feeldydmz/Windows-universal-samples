namespace Megazone.Cloud.Media.Domain
{
    public interface ICloudMediaRepository
    {
        MeResponse GetMe(MeRequest request);
        CaptionListResponse GetCaptions(CaptionListRequest request);
        VideoListResponse GetVideos(VideoListRequest request);
        Asset GetCaption(CaptionRequest request);
        Video GetVideo(VideoRequest request);
    }
}
