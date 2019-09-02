using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Api.Transcoder.Domain;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class McmWorkContext
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleParserProxy _subtitleService;
        private readonly SubtitleViewModel _subtitleViewModel;

        public McmWorkContext(SubtitleViewModel subtitleViewModel, Video openedVideo = null,
            CaptionAsset openedCaptionAsset = null)
        {
            _subtitleViewModel = subtitleViewModel;
            _subtitleService = Bootstrapper.Container.Resolve<SubtitleParserProxy>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
            _cloudMediaService = Bootstrapper.Container.Resolve<ICloudMediaService>();

            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            VideoMediaUrl = GetVideoMediaUrl(openedVideo);
            CaptionKind = GetTrackKind(openedCaptionAsset);
        }

        public Video OpenedVideo { get; private set; }
        public CaptionAsset OpenedCaptionAsset { get; private set; }
        public string UploadInputPath { get; private set; }
        public string VideoMediaUrl { get; private set; }
        public TrackKind CaptionKind { get; private set; }

        public void Initialize(Video openedVideo, CaptionAsset openedCaptionAsset)
        {
            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            VideoMediaUrl = GetVideoMediaUrl(openedVideo);
            CaptionKind = GetTrackKind(openedCaptionAsset);
        }

        public async Task<CaptionAsset> GetCaptionAssetAsync(string captionAssetId)
        {
            if (string.IsNullOrEmpty(captionAssetId))
                return null;

            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return await _cloudMediaService.GetCaptionAssetAsync(
                new GetAssetParameter(authorization, stageId, projectId, captionAssetId), CancellationToken.None);
        }

        public async Task<Video> GetVideoAsync(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
                return null;

            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return await _cloudMediaService.GetVideoAsync(
                new GetVideoParameter(authorization, stageId, projectId, videoId), CancellationToken.None);
        }

        public bool CanDeploy()
        {
            return OpenedVideo != null; // && (Captions?.Any() ?? false);
        }

        public Task<Settings> GetMcmSettingAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId),
                CancellationToken.None);
        }

        public async Task<string> GetUploadInputPathAsync()
        {
            var uploadTargetPath = string.Empty;
            var setting = await GetMcmSettingAsync();

            if (setting != null)
            {
                var s3Path = setting.Asset?.InputStoragePrefix?.Value;
                var folderPath = setting.Asset?.InputStoragePath?.Value;
                if (!string.IsNullOrEmpty(s3Path))
                    uploadTargetPath = $"{s3Path}{folderPath}";

                if (string.IsNullOrEmpty(uploadTargetPath))
                {
                    s3Path = setting.General?.StoragePrefix?.Value;
                    folderPath = setting.General?.StoragePath?.Value;
                    if (!string.IsNullOrEmpty(s3Path))
                        uploadTargetPath = $"{s3Path}{folderPath}";
                }
            }

            if (!string.IsNullOrEmpty(uploadTargetPath))
            {
                var uri = new Uri(uploadTargetPath);
                uploadTargetPath = $"{uri.Host}{uri.LocalPath}";
            }

            return uploadTargetPath;
        }

        public async Task DeployAsync(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions)
        {
            var captionList = captions?.ToList() ?? new List<Caption>();
            if (!captionList.Any())
                return;

            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var uploadInputPath = await GetUploadInputPathAsync();

            // upload caption files.
            foreach (var caption in captionList)
            {
                var uploadData = GetTextBy(caption);
                var fileName = GetFileName(caption);
                await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(authorization,
                    stageId, projectId, uploadData, fileName, uploadInputPath, caption.Url), CancellationToken.None);
            }

            if (string.IsNullOrEmpty(captionAsset.Id))
            {
                // create
                var createAsset = await _cloudMediaService.CreateCaptionAssetAsync(
                    new CreateCaptionAssetParameter(authorization, stageId, projectId, captionAsset.Name, captionList),
                    CancellationToken.None);

                Debug.Assert(createAsset == null, "createAsset is null.");
            }
            else
            {
                var updatedCaption = await _cloudMediaService.UpdateCaptionAsync(
                    new UpdateCaptionAssetParameter(authorization, stageId, projectId, captionAsset.Id, captionList),
                    CancellationToken.None);

                Debug.Assert(updatedCaption == null, "updatedCaption is null.");
            }
        }

        private string GetTextBy(Caption caption)
        {
            var tabItem = _subtitleViewModel.Tabs.Single(tab => tab.Caption.Equals(caption));
            var parser = SubtitleListItemParserProvider.Get(TrackFormat.WebVtt);
            var subtitles = tabItem.Rows.Select(s => s.ConvertToString(parser)).ToList();
            return _subtitleService.ConvertToText(subtitles, TrackFormat.WebVtt);
        }

        public void SetUploadInputPath(string uploadInputPath)
        {
            UploadInputPath = uploadInputPath;
        }

        public bool CanImportFile()
        {
            return OpenedVideo != null && OpenedCaptionAsset != null;
        }

        private string GetFileName(Caption caption)
        {
            var url = caption.Url;
            if (string.IsNullOrEmpty(url))
                return $"{caption.Label}_{caption.Language}_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.vtt";

            var lastSlashIndex = url.LastIndexOf('/');

            var fileName = url.Substring(lastSlashIndex + 1, url.Length - lastSlashIndex - 1);
            return fileName;
        }

        private string GetVideoMediaUrl(Video video)
        {
            // video영상을 가져온다.
            var asset = video?.Sources?.FirstOrDefault(rendition => rendition.Type.ToUpper().Equals("HLS")) ??
                        video?.Sources?.FirstOrDefault();

            if (asset == null)
                return string.Empty;

            var url = asset.Urls?.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
                url = asset.Elements?.FirstOrDefault()?.Urls?.FirstOrDefault();
            return url;
        }

        private TrackKind GetTrackKind(CaptionAsset asset)
        {
            var kind = asset?.Elements?.FirstOrDefault()?.Kind?.ToUpper() ?? string.Empty;
            var trackKind = TrackKind.Subtitle;
            switch (kind)
            {
                case "SUBTITLE":
                    trackKind = TrackKind.Subtitle;
                    break;
                case "CAPTION":
                    trackKind = TrackKind.Caption;
                    break;
                case "CHAPTER":
                    trackKind = TrackKind.Chapter;
                    break;
                case "DESCRIPTION":
                    trackKind = TrackKind.Description;
                    break;
                case "METADATA":
                    trackKind = TrackKind.Metadata;
                    break;
            }

            return trackKind;
        }
    }
}