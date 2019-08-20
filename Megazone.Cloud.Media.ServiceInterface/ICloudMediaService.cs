using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;

namespace Megazone.Cloud.Media.ServiceInterface
{
    public interface ICloudMediaService
    {
        Task<Authorization> LoginAsync(string userName, string password);
        Task<UserProfile> GetUserAsync(Authorization authorization);
        Task<CaptionList> GetCaptionsAsync(GetCaptionsParameter parameter);
        Task<VideoList> GetVideosAsync(GetVideosParameter parameter);
        Task<Asset<Caption>> GetCaptionAsync(GetCaptionParameter parameter);
        Task<Video> GetVideoAsync(GetVideoParameter parameter);
    }
}