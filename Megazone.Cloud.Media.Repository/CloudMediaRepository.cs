using System;
using System.Net;
using System.Text;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.IoC;
using Newtonsoft.Json;
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
               .AddJsonString<TAsset>(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<TAsset>();
        }

        public CaptionAsset CreateCaption(AssetRequest<CaptionAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.POST)
               .AddHeader("Authorization", $"Bearer {request.AccessToken}")
               .AddHeader("projectId", request.ProjectId)
               .AddJsonString<CaptionAsset>(request.Asset);

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

        public CaptionAsset GetCaption(AssetRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionAsset>();
        }

        public AssetListResponse<CaptionAsset> GetCaptions(AssetListRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/captions", Method.GET)
               .AddHeader("Authorization", $"Bearer {request.AccessToken}")
               .AddHeader("projectId", request.ProjectId)
               .AddQueryParameter("offset", request.Pagination.Offset.ToString())
               .AddQueryParameter("limit", request.Pagination.LimitPerPage.ToString());

            foreach (var condition in request.SearchConditions)
                restRequest.AddQueryParameter(condition.Key, condition.Value);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<AssetListResponse<CaptionAsset>>();
        }

        public MeResponse GetMe(MeRequest request)
        {
            // API : https://api.media.megazone.io/v1/me

            var restRequest = new RestRequest("v1/me", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}");

            var response = RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest);

            return response.StatusCode == HttpStatusCode.Unauthorized ? null : response.Convert<MeResponse>();
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
               .AddJsonString<TAsset>(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<TAsset>();
        }

        public CaptionAsset UpdateCaption(AssetRequest<CaptionAsset> request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets", Method.PUT)
               .AddHeader("Authorization", $"Bearer {request.AccessToken}")
               .AddHeader("projectId", request.ProjectId)
               .AddJsonString<CaptionAsset>(request.Asset);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionAsset>();
        }

        public Video UpdateVideo(VideoRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos", Method.PUT)
               .AddHeader("Authorization", $"Bearer {request.AccessToken}")
               .AddHeader("projectId", request.ProjectId)
               .AddJsonString<Video>(request.Video);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Video>();
        }
    }
}