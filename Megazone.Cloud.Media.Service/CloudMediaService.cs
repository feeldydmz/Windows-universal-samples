using System;
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
        // ReSharper disable once InconsistentNaming
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

        public async Task<Authorization> LoginAsync(string userName, string password)
        {
            const string authorizationEndpoint = "https://megaone.io";
            return await Task.Factory.StartNew(() =>
            {
                var authorizationResponse =
                    _authorizationRepository.Get(new AuthorizationRequest(authorizationEndpoint, userName, password));
                var accessToken = authorizationResponse.AccessToken;
                var refreshToken = authorizationResponse.RefreshToken;
                var expires = authorizationResponse.Expires;

                return new Authorization(accessToken, refreshToken, expires);
            });
        }

        public async Task<UserProfile> GetUserAsync(Authorization authorization)
        {
            return await Task.Factory.StartNew(() =>
                new UserProfile(
                    _cloudMediaRepository.GetMe(new MeRequest(CLOUD_MEDIA_ENDPOINT, authorization.AccessToken))));
        }

        public async Task<CaptionList> GetCaptionAssetsAsync(GetAssetsParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetCaptions(new AssetListRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    parameter.StageId, parameter.ProjectId, parameter.Pagination, parameter.SearchConditions));

                return new CaptionList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage,
                    response.TotalCount, response.Assets);
            });
        }

        public async Task<CaptionAsset> GetCaptionAssetAsync(GetAssetParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetCaption(new AssetRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));
                return response;
            });
        }

        public async Task<CaptionAsset> CreateCaptionAssetAsync(CreateCaptionAssetParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetName = parameter.AssetName;

                var asset = new CaptionAsset(null, assetName, "ACTIVE", "CAPTION", "TEXT", 0, 1, null, null);

                var response = _cloudMediaRepository.CreateCaption(new AssetRequest<CaptionAsset>(CLOUD_MEDIA_ENDPOINT,
                    accessToken, stageId, projectId, null, asset));

                return response;
            });
        }

        public async Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionAssetParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetId = parameter.AssetId;
                var version = parameter.Version;
                var captions = parameter.Captions;

                var asset = new CaptionAsset(assetId, null, null, null, null, 0, version, null, captions);

                var response = _cloudMediaRepository.UpdateAsset(new AssetRequest<CaptionAsset>(CLOUD_MEDIA_ENDPOINT,
                    accessToken, stageId, projectId, assetId, asset));
                return response;
            });
        }

        public async Task<VideoList> GetVideosAsync(GetVideosParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetVideos(new VideoListRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    parameter.StageId, parameter.ProjectId, parameter.Pagination, parameter.SearchConditions));

                var totalCount = response?.TotalCount ?? 0;
                var videos = response?.Videos ?? new List<Video>();
                return new VideoList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage, totalCount,
                    videos);
            });
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

        public async Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter)
        {
            // 캡션 추가.
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.UpdateVideo(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId));

                return response;
            });
        }

        public async Task<Settings> GetSettingsAsync(GetSettingsParameter parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetSetting(new SettingRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    parameter.StageId, parameter.ProjectId));
                return response;
            });
        }

        public Task UploadCaptionFileAsync(UploadCaptionFileParameter parameter)
        {
            //var uploadHostApiUrl = "https://upload.media.megazone.io";// production
            //var uploadHostApiUrl = "https://upload.media.stg.continuum.co.kr"; // stage
            var uploadHostApiUrl =
                "http://mz-cm-upload-load-balancer-830877039.ap-northeast-2.elb.amazonaws.com"; // develop

            return Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                _cloudMediaRepository.UploadCaption(new UploadCaptionRequest(uploadHostApiUrl, accessToken,
                    parameter.StageId, parameter.ProjectId, parameter.UploadFullPath, parameter.UploadData));
            });
        }

        public async Task<string> ReadAsync(Uri fileUri)
        {
            return await Task.Factory.StartNew(() => _cloudMediaRepository.Read(fileUri));
        }
    }
}