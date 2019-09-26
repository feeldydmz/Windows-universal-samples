using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.Cloud.Media.Domain
{
    public interface ICloudMediaRepository
    {
        IEnumerable<Stage> GetStages(string apiEndpoint, string accessToken);
        ProjectListResponse GetProjects(ProjectListRequest listRequest);
        AssetListResponse<TAsset> GetAssets<TAsset>(AssetListRequest request);
        TAsset GetAsset<TAsset>(AssetRequest request);
        TAsset UpdateAsset<TAsset>(AssetRequest<TAsset> request);
        TAsset CreateAsset<TAsset>(AssetRequest<TAsset> request);

        AssetListResponse<CaptionAsset> GetCaptionAssets(AssetListRequest request);
        CaptionAsset GetCaptionAsset(AssetRequest request);
        bool DeleteCaptionAsset(DeleteAssetRequest request);
        CaptionAsset UpdateCaptionAsset(AssetRequest<CaptionAsset> request);
        CaptionAsset CreateCaptionAsset(AssetRequest<CaptionAsset> request);
        VideoListResponse GetVideos(VideoListRequest request);
        IEnumerable<Language> GetLanguages(LanguageRequest request);
        Video GetVideo(VideoRequest request);
        Video UpdateVideo(VideoRequest request);
        bool UpdateVideoCaptions(VideoRequest request);

        Settings GetSetting(SettingRequest request);
        UploadResult UploadCaptionFile(UploadCaptionRequest request);
        string Read(Uri fileUri);
    }
}