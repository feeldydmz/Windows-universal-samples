using System;
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
        Task<UserProfile> GetUserAsync(Authorization authorization, CancellationToken cancellationToken);
        Task<ProjectListResponse> GetProjects(GetProjectsParameter parameter, CancellationToken cancellationToken);
        Task<CaptionList> GetCaptionAssetsAsync(GetAssetsParameter parameter, CancellationToken cancellationToken);
        Task<CaptionAsset> GetCaptionAssetAsync(GetAssetParameter parameter, CancellationToken cancellationToken);

        Task<CaptionAsset> CreateCaptionAssetAsync(CreateCaptionAssetParameter parameter,
            CancellationToken cancellationToken);

        Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionAssetParameter parameter,
            CancellationToken cancellationToken);

        Task<VideoList> GetVideosAsync(GetVideosParameter parameter, CancellationToken cancellationToken);
        Task<Video> GetVideoAsync(GetVideoParameter parameter, CancellationToken cancellationToken);
        Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter, CancellationToken cancellationToken);
        Task<Settings> GetSettingsAsync(GetSettingsParameter parameter, CancellationToken cancellationToken);
        Task<string> UploadCaptionFileAsync(UploadCaptionFileParameter parameter, CancellationToken cancellationToken);
        Task<string> ReadAsync(Uri fileUri, CancellationToken cancellationToken);
        Task DeleteCaptionAssetAsync(DeleteCaptionAssetParameter parameter, CancellationToken cancellationToken);
    }
}