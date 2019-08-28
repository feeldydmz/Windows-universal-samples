using System;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.Domain
{
    public interface ICloudMediaRepository
    {
        MeResponse GetMe(MeRequest request);
        ProjectListResponse GetProjects(ProjectListRequest listRequest);
        AssetListResponse<TAsset> GetAssets<TAsset>(AssetListRequest request);
        TAsset GetAsset<TAsset>(AssetRequest request);
        TAsset UpdateAsset<TAsset>(AssetRequest<TAsset> request);
        TAsset CreateAsset<TAsset>(AssetRequest<TAsset> request);

        AssetListResponse<CaptionAsset> GetCaptions(AssetListRequest request);
        CaptionAsset GetCaption(AssetRequest request);
        CaptionAsset UpdateCaption(AssetRequest<CaptionAsset> request);
        CaptionAsset CreateCaption(AssetRequest<CaptionAsset> request);
        VideoListResponse GetVideos(VideoListRequest request);
        Video GetVideo(VideoRequest request);
        Video UpdateVideo(VideoRequest request);

        Settings GetSetting(SettingRequest request);
        void UploadCaptionFile(UploadCaptionRequest request);
        string Read(Uri fileUri);
    }
}