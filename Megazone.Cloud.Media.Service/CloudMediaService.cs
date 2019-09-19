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
            "http://mz-cm-api-load-balancer-1319778791.ap-northeast-2.elb.amazonaws.com"; // develop version

        private const string UPLOAD_HOST_ENDPOINT =
            "http://mz-cm-upload-load-balancer-830877039.ap-northeast-2.elb.amazonaws.com"; // develop
#else
        private const string CLOUD_MEDIA_ENDPOINT = "https://api.media.megazone.io"; // production version
        private const string UPLOAD_HOST_ENDPOINT = "https://upload.media.megazone.io";// production
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


        public async Task<ProjectListResponse> GetProjects(GetProjectsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(
                () => _cloudMediaRepository.GetProjects(new ProjectListRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.Name)), cancellationToken);
        }

        public async Task<UserProfile> GetUserAsync(Authorization authorization, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response =
                    _cloudMediaRepository.GetMe(new MeRequest(CLOUD_MEDIA_ENDPOINT, authorization.AccessToken));

                return response == null ? null : new UserProfile(response);
            }, cancellationToken);
        }

        public async Task<CaptionList> GetCaptionAssetsAsync(GetAssetsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetCaptionAssets(new AssetListRequest(CLOUD_MEDIA_ENDPOINT,
                    accessToken, parameter.StageId, parameter.ProjectId, parameter.Pagination,
                    parameter.SearchConditions));

                return new CaptionList(parameter.Pagination.Offset, parameter.Pagination.LimitPerPage,
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

        public async Task<CaptionAsset> UpdateCaptionAsync(UpdateCaptionAssetParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var stageId = parameter.StageId;
                var projectId = parameter.ProjectId;
                var assetId = parameter.AssetId;
                var captionList = parameter.Captions.ToList();

                var asset = _cloudMediaRepository.GetCaptionAsset(new AssetRequest(CLOUD_MEDIA_ENDPOINT, accessToken,
                    stageId, projectId, assetId));

                if (asset == null)
                    throw new Exception("asset is null");

                var updatingCaptionList = asset.Elements?.ToList() ?? new List<Caption>();
                if (captionList.Any())
                {
                    var addList = new List<Caption>();
                    foreach (var newCaption in captionList)
                    {
                        var findCaption =
                            updatingCaptionList.SingleOrDefault(caption => caption.Id.Equals(newCaption.Id));
                        if (findCaption == null)
                        {
                            addList.Add(newCaption);
                        }
                        else
                        {
                            var index = updatingCaptionList.IndexOf(findCaption);
                            updatingCaptionList.Remove(findCaption);
                            updatingCaptionList.Insert(index, newCaption);
                        }
                    }

                    if (addList.Any())
                        updatingCaptionList.AddRange(addList);
                }

                var updateAsset = new CaptionAsset(asset.Id, asset.Name, asset.Status, asset.Type, asset.MediaType,
                    asset.IngestType, asset.Duration, asset.Version, asset.CreatedAt, updatingCaptionList);

                var response = _cloudMediaRepository.UpdateCaptionAsset(new AssetRequest<CaptionAsset>(
                    CLOUD_MEDIA_ENDPOINT,
                    accessToken, stageId, projectId, assetId, updateAsset));
                return response;
            }, cancellationToken);
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
#if DEBUG
                var result = _cloudMediaRepository.UpdateVideoCaptions(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId,
                    parameter.Video));
                return result ? parameter.Video : null;
#else
                var response = _cloudMediaRepository.UpdateVideo(new VideoRequest(CLOUD_MEDIA_ENDPOINT,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId,
                    parameter.Video));
                return response;
#endif
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

        public async Task<string> UploadCaptionFileAsync(UploadCaptionFileParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.UploadCaptionFile(new UploadCaptionRequest(UPLOAD_HOST_ENDPOINT,
                    accessToken, parameter.StageId, parameter.ProjectId, parameter.InputPath, parameter.FileName,
                    parameter.UploadData));
                return response.UploadedPath;
            }, cancellationToken);
        }

        public async Task<string> ReadAsync(Uri fileUri, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() => _cloudMediaRepository.Read(fileUri), cancellationToken);
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