﻿using System;
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
#if Dev
       private const string SPACE_ENDPOINT = "https://api.cloudplex.dev.megazone.io"; // develop version
       public const string CLOUD_PLEX_WEB_HOST_ENDPOINT = "https://console.cloudplex.dev.megazone.io"; // develop version
#elif STAGING
        private const string SPACE_ENDPOINT = "https://api.cloudplex.stg.megazone.io"; // stage version
        public const string CLOUD_PLEX_WEB_HOST_ENDPOINT = "https://console.cloudplex.stg.megazone.io";
#elif DEBUG
        private const string SPACE_ENDPOINT = "https://api.cloudplex.stg.megazone.io"; // stage version
        public const string CLOUD_PLEX_WEB_HOST_ENDPOINT = "https://console.cloudplex.stg.megazone.io";
#elif QA
        private const string SPACE_ENDPOINT = "https://api.cloudplex.qa.megazone.io"; // stage version
        public const string CLOUD_PLEX_WEB_HOST_ENDPOINT = "https://console.cloudplex.qa.megazone.io";
#else
        private const string SPACE_ENDPOINT = "https://api.cloudplex.megazone.io"; // production version
        public const string CLOUD_PLEX_WEB_HOST_ENDPOINT = "https://console.cloudplex.megazone.io";
#endif
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ICloudMediaRepository _cloudMediaRepository;

        public string Endpoint { get; set; }

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
                var asset = _cloudMediaRepository.GetAsset<RenditionAsset>(new AssetRequest(Endpoint,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));

                var renditionAssetList = new List<RenditionAsset> {asset};

                var video = new Video(asset.Id,
                    asset.Name,
                    asset.Type,
                    asset.Status,
                    asset.Duration,
                    asset.CreatedAt,
                    asset.Version,
                    "",
                    null,
                    null,
                    renditionAssetList,
                    null,
                    null,
                    null,
                    asset.Encryptions,
                    asset.FolderPath);

                return video;
            });
        }

        public async Task<VideoList> GetVideoAssetsAsync(GetAssetsParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetAssets<VideoAsset>(new AssetListRequest(Endpoint,
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
                        asset.Encryptions,
                        asset.FolderPath);

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
                () => _cloudMediaRepository.GetProjects(new ProjectListRequest(parameter.Endpoint,
                    parameter.Authorization.AccessToken)), cancellationToken);
        }

        public async Task<IEnumerable<Stage>> GetStagesAsync(GetStagesParameter parameter,
            CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(
                () => _cloudMediaRepository.GetStages(SPACE_ENDPOINT, parameter.Authorization.AccessToken),
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
                var response = _cloudMediaRepository.GetCaptionAssets(new AssetListRequest(Endpoint,
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
                var response = _cloudMediaRepository.GetCaptionAsset(new AssetRequest(Endpoint,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.AssetId));

                string[] arr = { "CAPTION", "TEXT"};
                var temp = arr.Contains(response.MediaType?.ToUpper()) ? response : null;
                Debug.WriteLine(" test : ", temp);
                return temp;
                //return !response.MediaType?.ToUpper().Equals("CAPTION") ?? false ? null : response;
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
                var folderPath = parameter.FolderPath;
                //var captions = parameter.Captions;

                var asset = new CaptionAsset(null, assetName, "ACTIVE", "CAPTION", "TEXT", "DIRECT", 0, 1, null,
                    null, folderPath);

                var response = _cloudMediaRepository.CreateCaptionAsset(new AssetRequest<CaptionAsset>(
                    Endpoint,
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
                var status = parameter.Status;
                var folderPath = parameter.FolderPath;

                var asset = _cloudMediaRepository.GetCaptionAsset(new AssetRequest(Endpoint, accessToken,
                    stageId, projectId, assetId));

                if (asset == null)
                    throw new Exception("asset is null");

                var assetName = string.IsNullOrEmpty(parameter.Name) ? asset.Name : parameter.Name;

                var updateAsset = new CaptionAsset(asset.Id, assetName, status, asset.Type, asset.MediaType,
                    asset.IngestType, asset.Duration, asset.Version, asset.CreatedAt, asset.Elements, folderPath);

                var response = _cloudMediaRepository.UpdateCaptionAsset(
                    new AssetRequest<CaptionAsset>(Endpoint, accessToken, stageId, projectId, assetId,
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

                return _cloudMediaRepository.UpdateCaptionElement(new CaptionRequest(Endpoint,
                    accessToken, stageId, projectId, assetId, assetVersion, caption));
            }, cancellationToken);
        }

        public async Task<Caption> CreateCaptionElementAsync(CreateAssetElementParameter parameter,
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
                new CaptionRequest(Endpoint, accessToken, stageId, projectId, assetId, version, element)), 
                cancellationToken);
        }

        public async Task<IEnumerable<Caption>> CreateCaptionElementBulkAsync(CreateAssetElementBulkParameter parameter,
            CancellationToken cancellationToken)
        {
            var accessToken = parameter.Authorization.AccessToken;
            var stageId = parameter.StageId;
            var projectId = parameter.ProjectId;
            var assetId = parameter.AssetId;
            var elements = parameter.Elements;
            var version = parameter.AssetVersion;

            return await Task.Factory.StartNew(() =>
                    _cloudMediaRepository.CreateCaptionElementBulk(
                        new CaptionBulkRequest(Endpoint, accessToken, stageId, projectId, assetId, version, elements)),
                cancellationToken);
        }


        public async Task<VideoList> GetVideosAsync(GetVideosParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var accessToken = parameter.Authorization.AccessToken;
                var response = _cloudMediaRepository.GetVideos(new VideoListRequest(Endpoint, accessToken,
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
            Debug.WriteLine($"GetLanguageAsync Endpoint : ${Endpoint}");
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetLanguages(new LanguageRequest(Endpoint,
                    parameter.AuthorizationAccessToken, parameter.StageId, parameter.ProjectId));

                return response;
            }, cancellationToken);
        }

        public async Task<Video> GetVideoAsync(GetVideoParameter parameter, CancellationToken cancellationToken)
        {
            return await Task.Factory.StartNew(() =>
            {
                var response = _cloudMediaRepository.GetVideo(new VideoRequest(Endpoint,
                    parameter.Authorization.AccessToken, parameter.StageId, parameter.ProjectId, parameter.VideoId));

                return response;
            }, cancellationToken);
        }

        public async Task<Video> UpdateVideoAsync(UpdateVideoParameter parameter, CancellationToken cancellationToken)
        {
            // 캡션 추가.
            return await Task.Factory.StartNew(() =>
            {
                var result = _cloudMediaRepository.UpdateVideoCaptions(new VideoRequest(Endpoint,
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
                var results = _cloudMediaRepository.BulkCaptionAsset(new BulkCaptionAssetRequest(Endpoint,
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
                var response = _cloudMediaRepository.GetSetting(new SettingRequest(Endpoint, accessToken,
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
                    new GetUploadUrlRequest(Endpoint, accessToken, parameter.StageId,
                        parameter.ProjectId, parameter.AssetId, parameter.ElementId, parameter.FileName, parameter.ShouldOverwrite));
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

                var result = _cloudMediaRepository.DeleteCaptionAsset(new DeleteAssetRequest(Endpoint,
                    accessToken,
                    stageId, projectId, assetId, version));
                Debug.Assert(!result, "Asset 삭제 실패.");
            }, cancellationToken);
        }

        public string GetWebHostEndPoint()
        {
            return CLOUD_PLEX_WEB_HOST_ENDPOINT;
        }
    }

}