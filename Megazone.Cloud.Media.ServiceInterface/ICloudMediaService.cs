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
        Task<CaptionList> GetCaptionsAsync(GetCaptionsParameter parameter);
        Task<CaptionAsset> GetCaptionAsync(GetCaptionParameter parameter);
        Task<CaptionAsset> CreateCaptionAsync(CreateCaptionParameter parameter);
        Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionParameter parameter);
        Task<VideoList> GetVideosAsync(GetVideosParameter parameter);
        Task<Video> GetVideoAsync(GetVideoParameter parameter);
        Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter);
        Task UploadCaptionFileAsync(UploadCaptionFileParameter parameter);
    }
}