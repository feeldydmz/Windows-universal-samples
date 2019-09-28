﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.IoC;
using RestSharp;

namespace Megazone.Cloud.Media.Repository
{
    [Inject(Source = typeof(ICloudMediaRepository), Scope = LifetimeScope.Transient)]
    internal class CloudMediaRepository : ICloudMediaRepository
    {
        public TAsset CreateAsset<TAsset>(AssetRequest<TAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<TAsset>();
        }

        public CaptionAsset CreateCaptionAsset(AssetRequest<CaptionAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionAsset>();
        }

        public TAsset GetAsset<TAsset>(AssetRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<TAsset>();
        }

        public AssetListResponse<TAsset> GetAssets<TAsset>(AssetListRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("offset", request.Pagination.Offset.ToString())
                .AddQueryParameter("limit", request.Pagination.LimitPerPage.ToString());

            foreach (var condition in request.SearchConditions)
                restRequest.AddQueryParameter(condition.Key, condition.Value);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<AssetListResponse<TAsset>>();
        }

        public CaptionAsset GetCaptionAsset(AssetRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionAsset>();
        }

        public AssetListResponse<CaptionAsset> GetCaptionAssets(AssetListRequest request)
        {
            //var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/captions", Method.GET)
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("offset", request.Pagination.Offset.ToString())
                .AddQueryParameter("limit", request.Pagination.LimitPerPage.ToString());

            foreach (var condition in request.SearchConditions)
                restRequest.AddQueryParameter(condition.Key, condition.Value);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<AssetListResponse<CaptionAsset>>();
        }

        public IEnumerable<Stage> GetStages(string apiEndpoint, string accessToken)
        {
            var restRequest =
                new RestRequest("v1/stages", Method.GET).AddHeader("Authorization", $"Bearer {accessToken}");

            return RestSharpExtension.CreateRestClient(apiEndpoint).Execute(restRequest).Convert<IEnumerable<Stage>>();
        }

        public ProjectListResponse GetProjects(ProjectListRequest listRequest)
        {
            var restRequest = new RestRequest($"v1/stages/{listRequest.StageId}/projects", Method.GET)
                .AddHeader("Authorization", $"Bearer {listRequest.AccessToken}")
                .AddHeader("stageId", listRequest.StageId);

            var response = RestSharpExtension.CreateRestClient(listRequest.Endpoint).Execute(restRequest);

            return response.StatusCode != HttpStatusCode.OK ? null : response.Convert<ProjectListResponse>();
        }

        public Settings GetSetting(SettingRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/settings", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Settings>();
        }

        public UploadResult UploadCaptionFile(UploadCaptionRequest request)
        {
            var sha256 = GetSha256(request.Text);

            var restRequest = new RestRequest($"v1/stages/{request.StageId}/upload", Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddParameter("contentHash", sha256)
                .AddFileBytes("file", Encoding.UTF8.GetBytes(request.Text), request.FileName);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest).Convert<UploadResult>();

            string GetSha256(string data)
            {
                var fileBytes = Encoding.UTF8.GetBytes(data);
                var contentHashBytes = HashAlgorithm.Create("SHA-256")?.ComputeHash(fileBytes);
                return ToHexString(contentHashBytes);

                string ToHexString(byte[] bytes)
                {
                    var result = "";
                    foreach (var @byte in bytes)
                        result += @byte.ToString("x2"); /* hex format */
                    return result;
                }
            }
        }

        public string Read(Uri fileUri)
        {
            var endpoint = $"https://{fileUri.Host}";
            var localPath = fileUri.LocalPath;
            if (localPath.StartsWith("/"))
                localPath = localPath.Substring(1, localPath.Length - 1);

            var response = RestSharpExtension.CreateRestClient(endpoint)
                .Execute(new RestRequest(localPath, Method.GET));

            return response?.Content;
        }

        public IEnumerable<Language> GetLanguages(LanguageRequest request)
        {
            var restRequest = new RestRequest("v1/stages/mz-cm-v1/languages", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest)
                .Convert<IEnumerable<Language>>();
        }

        public Video GetVideo(VideoRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos/{request.VideoId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Video>();
        }

        public VideoListResponse GetVideos(VideoListRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("offset", request.Pagination.Offset.ToString())
                .AddQueryParameter("limit", request.Pagination.LimitPerPage.ToString());

            foreach (var condition in request.SearchConditions)
                restRequest.AddQueryParameter(condition.Key, condition.Value);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<VideoListResponse>();
        }

        public TAsset UpdateAsset<TAsset>(AssetRequest<TAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.PUT)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<TAsset>();
        }

        public CaptionAsset UpdateCaptionAsset(AssetRequest<CaptionAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.PUT)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionAsset>();
        }

        public Video UpdateVideo(VideoRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos/{request.VideoId}", Method.PUT)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Video);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Video>();
        }

        public bool UpdateVideoCaptions(VideoRequest request)
        {
            var captionAssetList = request.Video.Captions.Select(asset => new VideoAsset(asset.Id)).ToList();
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos/{request.VideoId}/captions/bulk",
                    Method.PATCH)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.Video.Version.ToString())
                .AddJsonString(captionAssetList);

            var response = RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest);
            if (response.StatusCode == HttpStatusCode.OK) return !response.Content.Contains("errorCode");
            return false;
        }

        public bool DeleteCaptionAsset(DeleteAssetRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}", Method.DELETE)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.Version.ToString());
            var response = RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public class VideoAsset
        {
            public VideoAsset(string assetId)
            {
                AssetId = assetId;
            }

            public string AssetId { get; }
        }
    }
}