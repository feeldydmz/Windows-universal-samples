using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;

namespace Megazone.Cloud.Media.ServiceInterface
{
    public interface ICloudMediaService
    {
        Task<Authorization> LoginAsync(string userName, string password, CancellationToken cancellationToken);
        Task<Authorization> LoginByAuthorizationCodeAsync(string code, CancellationToken cancellationToken);

        Task<Authorization> RefreshByRefreshCodeAsync(string refreshCode, string accessCode, CancellationToken cancellationToken);
        Task<UserProfile> GetUserAsync(Authorization authorization, CancellationToken cancellationToken);
        Task<ProjectListResponse> GetProjectsAsync(GetProjectsParameter parameter, CancellationToken cancellationToken);
        Task<IEnumerable<Stage>> GetStagesAsync(GetStagesParameter parameter, CancellationToken cancellationToken);
		Task<Video> GetVideoAssetAsync(GetAssetParameter parameter, CancellationToken cancellationToken);
        Task<VideoList> GetVideoAssetsAsync(GetAssetsParameter parameter, CancellationToken cancellationToken);
        Task<CaptionAssetList> GetCaptionAssetsAsync(GetAssetsParameter parameter, CancellationToken cancellationToken);
        Task<CaptionAsset> GetCaptionAssetAsync(GetAssetParameter parameter, CancellationToken cancellationToken);
        Task<CaptionAsset> CreateCaptionAssetAsync(CreateCaptionAssetParameter parameter, CancellationToken cancellationToken);
        Task<CaptionAsset> UpdateCaptionAssetAsync(UpdateCaptionAssetParameter parameter, CancellationToken cancellationToken);
        Task<Caption> UpdateCaptionElementAsync(UpdateCaptionParameter parameter, CancellationToken cancellationToken);
        Task<IEnumerable<Language>> GetLanguageAsync(GetLanguageParameter parameter, CancellationToken cancellationToken);
        Task<VideoList> GetVideosAsync(GetVideosParameter parameter, CancellationToken cancellationToken);
        Task<Video> GetVideoAsync(GetVideoParameter parameter, CancellationToken cancellationToken);
        Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter, CancellationToken cancellationToken);
        // 비디오에 CaptionAsset을 등록한다.
        Task<IEnumerable<CaptionAsset>> RegisterCaptionAssetAsync(RegisterCaptionAssetParameter parameter, CancellationToken cancellationToken);
        Task<AssetUploadUrl> GetUploadUrlAsync(GetUploadUrlParameter parameter, CancellationToken cancellationToken);
        Task<Settings> GetSettingsAsync(GetSettingsParameter parameter, CancellationToken cancellationToken);
        Task<bool> UploadCaptionFileAsync(UploadCaptionFileParameter parameter, CancellationToken cancellationToken);
        Task<Caption> CreateCaptionElementAsync(CreateAssetElementParameter parameter,CancellationToken cancellationToken);

        Task<IEnumerable<Caption>> CreateCaptionElementBulkAsync(CreateAssetElementBulkParameter parameter,
            CancellationToken cancellationToken);
        Task<string> ReadAsync(Uri fileUri, CancellationToken cancellationToken);
        Task DeleteCaptionAssetAsync(DeleteCaptionAssetParameter parameter, CancellationToken cancellationToken);
        string GetWebHostEndPoint();

        string Endpoint { get; set; }
    }
}