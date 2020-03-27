using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;

// ReSharper disable InconsistentNaming

namespace Megazone.Cloud.Media.Service
{
    [Inject(Source = typeof(ICloudMediaService), Scope = LifetimeScope.Singleton)]
    internal class CloudMediaService : ICloudMediaService
    {
        // ReSharper disable once InconsistentNaming
#if STAGING
        private const string CLOUD_MEDIA_ENDPOINT = "https://api.media.stg.continuum.co.kr"; // stage version
        private const string UPLOAD_HOST_ENDPOINT = "https://upload.media.stg.continuum.co.kr"; // stage
#elif DEBUG
        private const string CLOUD_MEDIA_ENDPOINT =
            "https://api.cloudplex.dev.megazone.io"; // develop version

        private const string UPLOAD_HOST_ENDPOINT =
            "http://mz-cm-upload-load-balancer-830877039.ap-northeast-2.elb.amazonaws.com"; // develop
#else
        //private const string CLOUD_MEDIA_ENDPOINT = "https://api.media.megazone.io"; // production version
        //private const string UPLOAD_HOST_ENDPOINT = "https://upload.media.megazone.io";// production

        private const string CLOUD_MEDIA_ENDPOINT =
            "http://mz-cm-api-load-balancer-1319778791.ap-northeast-2.elb.amazonaws.com"; // develop version

        private const string UPLOAD_HOST_ENDPOINT =
            "http://mz-cm-upload-load-balancer-830877039.ap-northeast-2.elb.amazonaws.com"; // develop
#endif
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ICloudMediaRepository _cloudMediaRepository;

        public CloudMediaService(IAuthorizationRepository authorizationRepository,
            ICloudMediaRepository cloudMediaRepository)
        {
            _authorizationRepository = authorizationRepository;
            _cloudMediaRepository = cloudMediaRepository;
        }

        public async Task<Authorization> LoginAsync(string userName, string password,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var authorizationResponse =
                    _authorizationRepository.Get(new AuthorizationRequest(userName, password, string.Empty));
                var accessToken = authorizationResponse.AccessToken;
                var refreshToken = authorizationResponse.RefreshToken;
                var expires = authorizationResponse.Expires;

                return new Authorization(accessToken, refreshToken, expires);
            }, cancellationToken);
        }

        public async Task<Authorization> LoginByAuthorizationCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var authorizationResponse =
                    _authorizationRepository.Get(new AuthorizationRequest(string.Empty, string.Empty, code));

                return new Authorization(authorizationResponse?.AccessToken, authorizationResponse?.RefreshToken,
                    authorizationResponse?.Expires);
            }, cancellationToken);
        }

        public async Task<Authorization> RefreshByRefreshCodeAsync(string refreshCode, string accessCode, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var authorizationResponse =
                    _authorizationRepository.RefreshAccessCode(new AuthorizationRequest(string.Empty, accessCode, refreshCode));

                return new Authorization(authorizationResponse?.AccessToken, authorizationResponse?.RefreshToken,
                    authorizationResponse?.Expires);
            }, cancellationToken);
        }

        public async Task<Video> GetVideoAssetAsync(GetAssetParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetAsset<RenditionAsset>(new AssetRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));

                var renditionAssetList = new List<RenditionAsset> {response};

                var video = new Video(response.Id,
                    response.Name,
                    response.Type,
                    response.Status,
                    response.Duration,
                    response.CreatedAt,
                    response.Version,
                    "",
                    null,
                    null,
                    renditionAssetList,
                    null,
                    null,
                    null,
                    response.Encryptions);

                return video;
            });
        }

        public async Task<VideoList> GetVideoAssetsAsync(GetAssetsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetAssets<VideoAsset>(new AssetListRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.Pagination,
                    parameter.SearchConditions));

                Debug.WriteLine("GetAssetsAsync");

                if (response?.Assets == null) return null;

                var videoList = new List<Video>();
                foreach (var asset in response.Assets)
                {
                    var item = new Video(asset.Id,
                        asset.Name,
                        asset.Type,
                        asset.Status,
                        asset.Duration,
                        asset.CreatedAt,
                        asset.Version,
                        asset.Thumbnails?.FirstOrDefault()?.AccessUrl,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        asset.Encryptions);

                    videoList.Add(item);
                }


                return new VideoList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage,
                    response.TotalCount, videoList);
                //return response.Assets.FirstOrDefault();

                //return !response.MediaType?.ToUpper().Equals("VIDEO") ?? false ? null : response;
            }, cancellationToken);
        }

        public async Task<ProjectListResponse> GetProjectsAsync(GetProjectsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(
                () => _cloudMediaRepository.GetProjects(new ProjectListRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.Name)), cancellationToken);
        }

        public async Task<IEnumerable<Stage>> GetStagesAsync(GetStagesParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(
                () => _cloudMediaRepository.GetStages(CLOUD_MEDIA_ENDPOINT, parameter.Authorization.AccessToken),
                cancellationToken);
        }

        public async Task<UserProfile> GetUserAsync(Authorization authorization, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _authorizationRepository.GetMe(new MeRequest(authorization.AccessToken));
                return response == null ? null : new UserProfile(response);
            }, cancellationToken);
        }

        public async Task<CaptionAssetList> GetCaptionAssetsAsync(GetAssetsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetCaptionAssets(new AssetListRequest(CLOUD_MEDIA_ENDPOINT,
                    accessToken, parameter.StageId, parameter.ProjectId, parameter.Pagination,
                    parameter.SearchConditions));

                return new CaptionAssetList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage,
                    response.TotalCount, response.Assets);
            }, cancellationToken);
        }

        public async Task<CaptionAsset> GetCaptionAssetAsync(GetAssetParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetCaptionAsset(new AssetRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));

                return !response.MediaType?.ToUpper().Equals("TEXT") ?? false ? null : response;
            }, cancellationToken);
        }

        public async Task<CaptionAsset> CreateCaptionAssetAsync(CreateCaptionAssetParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetName = parameter.AssetName;
                var captions = parameter.Captions;

                var asset = new CaptionAsset(null, assetName, "ACTIVE", "CAPTION", "TEXT", "DIRECT", 0, 1, null,
                    captions);

                var response = _cloudMediaRepository.CreateCaptionAsset(new AssetRequest<CaptionAsset>(
                    CLOUD_MEDIA_ENDPOINT,
                    accessToken, stageId, projectId, null, asset));

                return response;
            }, cancellationToken);
        }

        public async Task<CaptionAsset> UpdateCaptionAssetAsync(UpdateCaptionAssetParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetId = parameter.AssetId;

                var asset = _cloudMediaRepository.GetCaptionAsset(new AssetRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    stageId, projectId, assetId));

                if (asset == null)
                    throw new Exception("asset is null");

                var assetName = string.IsNullOrEmpty(parameter.Name) ? asset.Name : parameter.Name;

                var updateAsset = new CaptionAsset(asset.Id, assetName, asset.Status, asset.Type, asset.MediaType,
                    asset.IngestType, asset.Duration, asset.Version, asset.CreatedAt, asset.Elements);

                var response = _cloudMediaRepository.UpdateCaptionAsset(
                    new AssetRequest<CaptionAsset>(CLOUD_MEDIA_ENDPOINT, accessToken, stageId, projectId, assetId,
                        updateAsset));
                return response;
            }, cancellationToken);
        }

        public async Task<Caption> UpdateCaptionElementAsync(UpdateCaptionParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetId = parameter.AssetId;
                var assetVersion = parameter.AssetVersion;
                var caption = parameter.Caption;

                return _cloudMediaRepository.UpdateCaptionElement(new CaptionRequest(CLOUD_MEDIA_ENDPOINT,
                    accessToken, stageId, projectId, assetId, assetVersion, caption));
            }, cancellationToken);
        }

        public async Task<Caption> CreateCaptionElementsAsync(CreateAssetElementParameter parameter,
            CancellationToken cancellationToken)
        {
            var accessToken = parameter.Authorization.AccessToken;
            var stageId = parameter.StageId;
            var projectId = parameter.ProjectId;
            var assetId = parameter.AssetId;
            var element = parameter.Element;
            var version = parameter.AssetVersion;

            return await Task.Factory.StartNew(() => 
                _cloudMediaRepository.CreateCaptionElement(
                new CaptionRequest(CLOUD_MEDIA_ENDPOINT, accessToken, stageId, projectId, assetId, version, element)), 
                cancellationToken);
        }

        public async Task<VideoList> GetVideosAsync(GetVideosParameter parameter, CancellationToken cancellationToken)
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
            }, cancellationToken);
        }

        public async Task<IEnumerable<Language>> GetLanguageAsync(GetLanguageParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetLanguages(new LanguageRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.AuthorizationAccessToken, parameter.StageId, parameter.ProjectId));

                return response;
            }, cancellationToken);
        }

        public async Task<Video> GetVideoAsync(GetVideoParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetVideo(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId));

                return response;
            }, cancellationToken);
        }

        public async Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter, CancellationToken cancellationToken)
        {
            // 캡션 추가.
            return await Task.Factory.StartNew(() =>
            {
                var result = _cloudMediaRepository.UpdateVideoCaptions(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId,
                    parameter.Video));
                return result ? parameter.Video : null;
            }, cancellationToken);
        }

        // 비디오에 CaptionAsset을 등록한다.
        public async Task<IEnumerable<CaptionAsset>> RegisterCaptionAssetAsync(RegisterCaptionAssetParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var results = _cloudMediaRepository.BulkCaptionAsset(new BulkCaptionAssetRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId,
                    parameter.VideoVersion, parameter.AssetIds));
                return results;
            }, cancellationToken);
        }

        public async Task<Settings> GetSettingsAsync(GetSettingsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetSetting(new SettingRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    parameter.StageId, parameter.ProjectId));
                return response;
            }, cancellationToken);
        }

        public async Task<AssetUploadUrl> GetUploadUrlAsync(GetUploadUrlParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetUploadUrl(
                    new GetUploadUrlRequest(CLOUD_MEDIA_ENDPOINT, accessToken, parameter.StageId,
                        parameter.ProjectId, parameter.AssetId, parameter.FileName, parameter.IsAttachId));
                return response;
            }, cancellationToken);
        }

        public async Task<bool> UploadCaptionFileAsync(UploadCaptionFileParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() => 
                _cloudMediaRepository.UploadCaptionFile(new UploadCaptionRequest(parameter.UploadUrl, parameter.UploadData)), 
                cancellationToken);
        }

        public async Task<string> ReadAsync(Uri fileUri, CancellationToken cancellationToken)
        {
            return await _cloudMediaRepository.Read(fileUri);
            //   return await Task.Factory.StartNew(() => _cloudMediaRepository.Read(fileUri), cancellationToken);
        }

        public Task DeleteCaptionAssetAsync(DeleteCaptionAssetParameter parameter, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetId = parameter.CaptionAssetId;
                var version = parameter.Version;

                var result = _cloudMediaRepository.DeleteCaptionAsset(new DeleteAssetRequest(CLOUD_MEDIA_ENDPOINT,
                    accessToken,
                    stageId, projectId, assetId, version));
                Debug.Assert(!result, "Asset 삭제 실패.");
            }, cancellationToken);
        }
    }

}