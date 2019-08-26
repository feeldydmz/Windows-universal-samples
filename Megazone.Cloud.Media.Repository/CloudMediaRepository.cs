using System;
using System.IO;
using System.Net;
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

        public CaptionAsset CreateCaption(AssetRequest<CaptionAsset> request)
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

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<MeResponse>();
        }

        public Settings GetSetting(SettingRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/settings", Method.GET)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Settings>();
        }

        public void UploadCaptionFile(UploadCaptionRequest request)
        {
            var sha256 = GetSha256(request.Text);
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(request.Text));

            var restRequest = new RestRequest($"v1/stages/{request.StageId}/upload", Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddParameter("contentHash", sha256)
                .AddFileBytes("file", UTF8Encoding.UTF8.GetBytes(request.Text), request.FileName);

            var response = RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest);
            if (response != null)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception();
                }
            }

            string GetSha256(string data)
            {
                var fileBytes = System.Text.Encoding.UTF8.GetBytes(data);
                var contentHashBytes = System.Security.Cryptography.HashAlgorithm.Create("SHA-256")?.ComputeHash(fileBytes);
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

        public CaptionAsset UpdateCaption(AssetRequest<CaptionAsset> request)
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
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos", Method.PUT)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddJsonString(request.Video);

            return RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest).Convert<Video>();
        }
    }
}