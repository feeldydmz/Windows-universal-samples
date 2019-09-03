using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Api.Transcoder.Domain;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class McmWorkContext
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly SubtitleParserProxy _subtitleService;

        public McmWorkContext(SubtitleViewModel subtitleViewModel, Video openedVideo, CaptionAsset openedCaptionAsset)
        {
            _subtitleViewModel = subtitleViewModel;
            _subtitleService = Bootstrapper.Container.Resolve<SubtitleParserProxy>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
            _cloudMediaService = Bootstrapper.Container.Resolve<ICloudMediaService>();
            

            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            VideoUrlByResolutions = GetVideoUrlDictionary(openedVideo);
            VideoMediaUrl = VideoUrlByResolutions?.FirstOrDefault().Value ?? "";
            CaptionKind = GetTrackKind(openedCaptionAsset);
        }

        public Video OpenedVideo { get; }
        public CaptionAsset OpenedCaptionAsset { get; }
        public string UploadInputPath { get; private set; }
        public string VideoMediaUrl { get; }

        public Dictionary<int, string> VideoUrlByResolutions { get; private set; }
        public TrackKind CaptionKind { get; }

        public bool CanDeploy()
        {
            return OpenedVideo != null;// && (Captions?.Any() ?? false);
        }

        public Task<Settings> GetMcmSettingAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId), CancellationToken.None);
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

        public async Task DeployAsync(IEnumerable<Caption> captions)
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var uploadInputPath = await GetUploadInputPathAsync();

            // upload caption files.
            foreach (var caption in captions)
            {
                var uploadData = GetTextBy(caption);
                var fileName = GetFileName(caption);
                await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(authorization,
                    stageId, projectId, uploadData, fileName, uploadInputPath, caption.Url), CancellationToken.None);
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

        private Dictionary<int, string> GetVideoUrlDictionary(Video video)
        {
            return video.Sources.SelectMany(renditionAsset => renditionAsset.Elements).ToDictionary(item => item.VideoSetting.Height, item => item.Urls.First() ?? "");
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

    [Obsolete("",true)]
    internal class CaptionContext : Caption
    {
        private readonly Caption _source;

        public CaptionContext(Caption source) : base(source.Id, source.IsDraft, source.IsPreferred, source.Language,
            source.Country, source.Kind, source.Label, source.Url)
        {
            _source = source;
        }

        public bool IsSelected { get; set; }
        public string Text { get; set; }

        public string GetFileName()
        {
            var caption = _source;
            var url = caption.Url;
            if (string.IsNullOrEmpty(url))
                return $"{caption.Label}_{caption.Language}_{DateTime.UtcNow.DateTimeToEpoch()}.vtt";

            var lastSlashIndex = url.LastIndexOf('/');

            var fileName = url.Substring(lastSlashIndex + 1, url.Length - lastSlashIndex - 1);
            return fileName;
        }
    }
}