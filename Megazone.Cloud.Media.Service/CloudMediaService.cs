using System.Collections.Generic;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;

namespace Megazone.Cloud.Media.Service
{
    [Inject(Source = typeof(ICloudMediaService), Scope = LifetimeScope.Singleton)]
    internal class CloudMediaService : ICloudMediaService
    {
        private const string AUTHORIZATION_ENDPOINT = "https://megaone.io";

        //private const string CLOUD_MEDIA_ENDPOINT = "https://api.media.megazone.io"; // production version
        private const string CLOUD_MEDIA_ENDPOINT = "https://api.media.stg.continuum.co.kr"; // stage version
        //private const string CLOUD_MEDIA_ENDPOINT =
        //    "http://mz-cm-api-load-balancer-1319778791.ap-northeast-2.elb.amazonaws.com"; // develop version

        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ICloudMediaRepository _cloudMediaRepository;

        public CloudMediaService(IAuthorizationRepository authorizationRepository,
            ICloudMediaRepository cloudMediaRepository)
        {
            _authorizationRepository = authorizationRepository;
            _cloudMediaRepository = cloudMediaRepository;
        }

        public Task<CaptionAsset> CreateCaptionAsync(CreateCaptionParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public async Task<CaptionAsset> GetCaptionAsync(GetCaptionParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetCaption(new AssetRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));

                return response;
            });
        }

        public async Task<CaptionList> GetCaptionsAsync(GetCaptionsParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetCaptions(new AssetListRequest(CLOUD_MEDIA_ENDPOINT, accessToken, parameter.StageId, parameter.ProjectId, parameter.Pagination, parameter.SearchConditions));

                return new CaptionList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage,
                    response.TotalCount, response.Assets);
            });
        }

        public async Task<UserProfile> GetUserAsync(Authorization authorization)
        {
            return await Task.Factory.StartNew(() =>
                new UserProfile(
                    _cloudMediaRepository.GetMe(new MeRequest(CLOUD_MEDIA_ENDPOINT, authorization.AccessToken))));
        }

        public async Task<Video> GetVideoAsync(GetVideoParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetVideo(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId));

                return response;
            });
        }

        public async Task<VideoList> GetVideosAsync(GetVideosParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetVideos(new VideoListRequest(CLOUD_MEDIA_ENDPOINT, accessToken, parameter.StageId, parameter.ProjectId, parameter.Pagination, parameter.SearchConditions));

                var totalCount = response?.TotalCount ?? 0;
                var videos = response?.Videos ?? new List<Video>();
                return new VideoList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage, totalCount, videos);
            });
        }

        public async Task<Authorization> LoginAsync(string userName, string password)
        {
            return await Task.Factory.StartNew(() =>
            {
                var authorizationResponse =
                    _authorizationRepository.Get(new AuthorizationRequest(AUTHORIZATION_ENDPOINT, userName, password));
                var accessToken = authorizationResponse.AccessToken;
                var refreshToken = authorizationResponse.RefreshToken;
                var expires = authorizationResponse.Expires;

                return new Authorization(accessToken, refreshToken, expires);
            });
        }

        public Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public Task UploadCaptionFileAsync(UploadCaptionFileParameter parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}