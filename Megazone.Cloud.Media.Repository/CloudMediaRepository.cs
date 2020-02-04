using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.IoC;
using RestSharp;
using RestSharp.Extensions;

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

        public async Task<string> Read(Uri fileUri)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, fileUri))
                {
                    var httpResponseMessage = await httpClient.SendAsync(httpRequest);
                    var response = await httpResponseMessage.Content.ReadAsStringAsync();
                    return response;
                }
            }
        }

        public IEnumerable<Language> GetLanguages(LanguageRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/languages", Method.GET)
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

            var response =RestSharpExtension.CreateRestClient(request.Endpoint)
                .Execute(restRequest);

            return response.Convert<VideoListResponse>();
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
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.Asset.Id}", Method.PATCH)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId).AddQueryParameter("version", request.Asset.Version.ToString())
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
                    Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.Video.Version.ToString())
                .AddJsonString(captionAssetList);

            var response = RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest);
            if (response.StatusCode == HttpStatusCode.OK) return !response.Content.Contains("errorCode");
            return false;
        }

        public Caption CreateCaption(CaptionRequest request)
        {
            // caption data.
            /*
country: "FR"
kind: "SUB_TITLE"
label: "불어"
language: "fr"
size: 280
url: "https://mz-cm-transcoding-output.s3.amazonaws.com/mz-cm-v1/test.vtt"
             */
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}/elements",
                    Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.Version.ToString())
                .AddJsonString(request.Caption);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest).Convert<Caption>();
        }

        public bool DeleteCaption(CaptionRequest request)
        {
            throw new NotImplementedException();
        }

        public Caption UpdateCaption(CaptionRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/assets/{request.AssetId}/elements/{request.Caption.Id}",
                    Method.PATCH)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.Version.ToString())
                .AddJsonString(request.Caption);

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest).Convert<Caption>();
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

        public IEnumerable<CaptionAsset> BulkCaptionAsset(BulkCaptionAssetRequest request)
        {
            var restRequest = new RestRequest($"v1/stages/{request.StageId}/videos/{request.VideoId}/captions/bulk", Method.POST)
                .AddHeader("Authorization", $"Bearer {request.AccessToken}")
                .AddHeader("projectId", request.ProjectId)
                .AddQueryParameter("version", request.VideoVersion.ToString())
                .AddJsonString(request.AssetIds.ToList().Select(id=> new BulkAssetModel(id)));

            return RestSharpExtension.CreateRestClient(request.Endpoint).Execute(restRequest)
                .Convert<IEnumerable<CaptionAsset>>();
        }

        private class BulkAssetModel
        {
            public BulkAssetModel(string assetId)
            {
                AssetId = assetId;
            }
            public string AssetId { get; }
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