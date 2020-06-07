using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        Caption CreateCaption(CaptionRequest request);
        Caption UpdateCaptionElement(CaptionRequest request);
        Caption CreateCaptionElement(CaptionRequest request);

        IEnumerable<Caption> CreateCaptionElementBulk(CaptionBulkRequest request);
        bool DeleteCaption(CaptionRequest request);

        Settings GetSetting(SettingRequest request);
        bool UploadCaptionFile(UploadCaptionRequest request);
        AssetUploadUrl GetUploadUrl(GetUploadUrlRequest request);
        Task<string> Read(Uri fileUri);
        IEnumerable<CaptionAsset> BulkCaptionAsset(BulkCaptionAssetRequest request);
    }
}