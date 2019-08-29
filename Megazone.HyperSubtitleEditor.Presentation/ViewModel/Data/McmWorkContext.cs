using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.Extension;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class McmWorkContext
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;
        private bool _isModified;

        public McmWorkContext(Video openedVideo, CaptionAsset openedCaptionAsset, IEnumerable<CaptionContext> captions)
        {
            OpenedVideo = openedVideo;
            OpenedCaptionAsset = openedCaptionAsset;
            Captions = captions;

            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
            _cloudMediaService = Bootstrapper.Container.Resolve<ICloudMediaService>();
        }

        public Video OpenedVideo { get; }
        public CaptionAsset OpenedCaptionAsset { get; }
        public IEnumerable<CaptionContext> Captions { get; }

        public string UploadInputPath { get; private set; }

        public bool CanDeploy()
        {
            return OpenedVideo != null;
        }

        public bool IsModified()
        {
            return _isModified;
        }

        public async Task<string> GetUploadInputPathAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var setting =
                await _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId),
                    CancellationToken.None);

            var uploadTargetPath = string.Empty;
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

        public async Task DeployAsync(IEnumerable<CaptionContext> captionItems)
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            var list = captionItems.Where(x => x.IsSelected).ToList();
            var uploadInputPath = await GetUploadInputPathAsync();

            // upload caption files.
            foreach (var caption in list)
            {
                var uploadData = caption.Text;
                var fileName = caption.GetFileName();
                await _cloudMediaService.UploadCaptionFileAsync(new UploadCaptionFileParameter(authorization,
                    stageId, projectId, uploadData, fileName, uploadInputPath, caption.Url), CancellationToken.None);
            }
        }

        public void SetUploadInputPath(string uploadInputPath)
        {
            UploadInputPath = uploadInputPath;
        }
    }

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