﻿using System.Text;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.IoC;
using Newtonsoft.Json;
using RestSharp;

namespace Megazone.Cloud.Media.Repository
{
    [Inject(Source = typeof(ICloudMediaRepository), Scope = LifetimeScope.Transient)]
    internal class CloudMediaRepository : ICloudMediaRepository
    {
        public MeResponse GetMe(MeRequest request)
        {
            // API : https://api.media.megazone.io/v1/me

            var restRequest = new RestRequest("v1/me", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}");

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<MeResponse>();
        }

        public CaptionListResponse GetCaptions(CaptionListRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/captions", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("offset", request.Pagination.Offset.ToString())
                .AddQueryParameter("limit", request.Pagination.LimitPerPage.ToString());
            
            foreach (var condition in request.SearchConditions)
                restRequest.AddQueryParameter(condition.Key, condition.Value);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<CaptionListResponse>();
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

        public Asset<Caption> GetCaption(CaptionRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.CaptionId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest).Convert<Asset<Caption>>();
        }

        public Video GetVideo(VideoRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos/{request.VideoId}", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("stageId", request.StageId)
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest).Convert<Video>();
        }
    }
}