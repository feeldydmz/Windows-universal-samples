using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;

namespace Megazone.Cloud.Media.ServiceInterface
{
    public interface ICloudMediaService
    {
        Task<Authorization> LoginAsync(string userName, string password);
        Task<UserProfile> GetUserAsync(Authorization authorization);
        Task<CaptionList> GetCaptionAssetsAsync(GetAssetsParameter parameter);
        Task<CaptionAsset> GetCaptionAssetAsync(GetAssetParameter parameter);
        Task<CaptionAsset> CreateCaptionAssetAsync(CreateCaptionAssetParameter parameter);
        Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionAssetParameter parameter);
        Task<VideoList> GetVideosAsync(GetVideosParameter parameter);
        Task<Video> GetVideoAsync(GetVideoParameter parameter);
        Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter);
        Task<Settings> GetSettingsAsync(GetSettingsParameter parameter);
        Task UploadCaptionFileAsync(UploadCaptionFileParameter parameter);
    }
}