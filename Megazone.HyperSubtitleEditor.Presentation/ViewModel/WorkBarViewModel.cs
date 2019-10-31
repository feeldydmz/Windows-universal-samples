﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;
using Subtitle = Megazone.HyperSubtitleEditor.Presentation.Message.Subtitle;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    // TODO: 기존 로직과 온라인에서 관리해야하는 롤이 추가되변서, 기능이 여기 저기 혼재 되어 있다. 정리 필요.
    // 저장/오픈/게시등 데이터 관리 뷰모델.
    // SubtitleViewModel은 편집 관련 
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class WorkBarViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ILogger _logger;
        private readonly RecentlyLoader _recentlyLoader;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleParserProxy _subtitleService;
        private CaptionAssetItemViewModel _captionAssetItem;
        private bool _hasWorkData;
        private bool _isBusy;

        private bool _isLoading;
        private bool _isOnlineData;
        private bool _isOpenVideoInfoPopup;
        private ICommand _loadCommand;

        private ICommand _openAssetEditorCommand;
        private ICommand _unloadCommand;
        private VideoItemViewModel _videoItem;

        public WorkBarViewModel(IBrowser browser, ICloudMediaService cloudMediaService, ILogger logger,
            SignInViewModel signInViewModel, SubtitleParserProxy subtitleService, RecentlyLoader recentlyLoader)
        {
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _logger = logger;
            _signInViewModel = signInViewModel;
            _recentlyLoader = recentlyLoader;
            _subtitleService = subtitleService;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public bool IsOpenVideoInfoPopup
        {
            get => _isOpenVideoInfoPopup;
            set => Set(ref _isOpenVideoInfoPopup, value);
        }

        public bool IsOnlineData
        {
            get => _isOnlineData;
            set => Set(ref _isOnlineData, value);
        }

        // Video / Asset 데이터 유무.
        public bool HasWorkData
        {
            get => _hasWorkData;
            set => Set(ref _hasWorkData, value);
        }

        // 작업대상인 Video 정보.
        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }

        // 작업대상인 Asset 정보.
        public CaptionAssetItemViewModel CaptionAssetItem
        {
            get => _captionAssetItem;
            set => Set(ref _captionAssetItem, value);
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand UnloadCommand
        {
            get { return _unloadCommand = _unloadCommand ?? new RelayCommand(Unload); }
        }

        public ICommand OpenAssetEditorCommand
        {
            get { return _openAssetEditorCommand = _openAssetEditorCommand ?? new RelayCommand(OpenAssetEditor); }
        }

        public string UploadInputPath { get; private set; }

        public void Initialize()
        {
            IsOnlineData = false;
            HasWorkData = false;
            VideoItem = null;
            CaptionAssetItem = null;
        }

        private void OpenAssetEditor()
        {
            _browser.Main.ShowAssetEditorDialog(CaptionAssetItem?.Source);
        }


        private void Load()
        {
            RegisterMessageHandlers();
        }

        private void Unload()
        {
            UnregisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            MessageCenter.Instance.Regist<CloudMedia.CaptionOpenRequestedMessage>(OnCaptionOpenRequest);
            MessageCenter.Instance.Regist<CloudMedia.CaptionAssetRenameRequestedMessage>(RenameCaptionAsset);
            MessageCenter.Instance.Regist<CloudMedia.DeployRequestedMessage>(Deploy);
            MessageCenter.Instance.Regist<Subtitle.FileOpenedMessage>(OnFileOpened);
        }

        private void UnregisterMessageHandlers()
        {
            MessageCenter.Instance.Unregist<CloudMedia.CaptionOpenRequestedMessage>(OnCaptionOpenRequest);
            MessageCenter.Instance.Unregist<CloudMedia.CaptionAssetRenameRequestedMessage>(RenameCaptionAsset);
            MessageCenter.Instance.Unregist<CloudMedia.DeployRequestedMessage>(Deploy);
            MessageCenter.Instance.Unregist<Subtitle.FileOpenedMessage>(OnFileOpened);
        }


        private void OnFileOpened(Subtitle.FileOpenedMessage message)
        {
            IsOnlineData = VideoItem != null || CaptionAssetItem != null;


            CaptionAssetItem =
                new CaptionAssetItemViewModel(new CaptionAsset(null, "untitle", null, null, null, null, 0, 0, null,
                    null));

            HasWorkData = true;
        }


        private async void RenameCaptionAsset(CloudMedia.CaptionAssetRenameRequestedMessage message)
        {
            var assetName = message.Name;
            if (string.IsNullOrEmpty(assetName))
                return;

            if (!string.IsNullOrEmpty(message.CaptionAsset?.Id))
            {
                if (IsBusy)
                    return;
                IsBusy = true;

                // 바로 적용.
                _browser.Main.LoadingManager.Show();
                try
                {
                    var authorization = _signInViewModel.GetAuthorization();
                    var stageId = _signInViewModel.SelectedStage?.Id;
                    var projectId = _signInViewModel.SelectedProject?.ProjectId;
                    var assetId = message.CaptionAsset?.Id;

                    var captionAsset = await _cloudMediaService.UpdateCaptionAssetAsync(
                        new UpdateCaptionAssetParameter(authorization, stageId, projectId, assetId, assetName),
                        CancellationToken.None);

                    if (captionAsset != null)
                        CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    IsBusy = false;
                    _browser.Main.LoadingManager.Hide();
                }
            }
            else
            {
                CaptionAssetItem =
                    new CaptionAssetItemViewModel(new CaptionAsset(null, assetName, null, null, null, null, 0, 0, null,
                        null));
            }
        }

        private async void OnCaptionOpenRequest(CloudMedia.CaptionOpenRequestedMessage requestedMessage)
        {
            // 초기화
            Initialize();
            IsOnlineData = requestedMessage.Param.IsOnline;

            if (IsBusy)
                return;

            IsBusy = true;
            IsLoading = true;
            _browser.Main.LoadingManager.Show();
            try
            {
                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;
                var videoId = requestedMessage.Param.Video?.Id;
                var assetId = requestedMessage.Param.Asset?.Id;
                var captions = requestedMessage.Param.Captions;

                var video = string.IsNullOrEmpty(videoId)
                    ? null
                    : await _cloudMediaService.GetVideoAsync(
                        new GetVideoParameter(authorization, stageId, projectId, videoId), CancellationToken.None);

                if (video != null)
                {
                    VideoItem = new VideoItemViewModel(video);
                    VideoItem.SelectedCaptionAsset =
                        VideoItem.CaptionAssetItems.SingleOrDefault(asset => asset.Id.Equals(assetId));
                }

                var captionAsset = string.IsNullOrEmpty(assetId)
                    ? null
                    : await _cloudMediaService.GetCaptionAssetAsync(
                        new GetAssetParameter(authorization, stageId, projectId, assetId), CancellationToken.None);

                if (captionAsset != null)
                    CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset);

                HasWorkData = video != null || captionAsset != null;

                _recentlyLoader.Save(new RecentlyItem.OnlineRecentlyCreator().SetVideo(video)
                    .SetCaptionAsset(captionAsset).SetCaptions(captions).Create());
            }
            catch (Exception e)
            {
                HasWorkData = false;
                _logger.Error.Write(e);
            }
            finally
            {
                IsBusy = false;
                IsLoading = false;
                _browser.Main.LoadingManager.Hide();
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private async void Deploy(CloudMedia.DeployRequestedMessage message)
        {
            try
            {
                IsBusy = true;
                _browser.Main.LoadingManager.Show();
                // 현재 정보
                await DeployAsync(message.Param.Video, message.Param.CaptionAsset, message.Param.Captions.ToList(),
                    (isSuccess, video, captionAsset) =>
                    {
                        if (isSuccess)
                        {
                            if (video != null)
                                VideoItem = new VideoItemViewModel(video);
                            if (captionAsset != null)
                                CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset);
                            HasWorkData = true;
                            IsOnlineData = true;

                            var subtitle = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                            foreach (var tab in subtitle.Tabs)
                            {
                                var newCaption =
                                    captionAsset?.Elements?.FirstOrDefault(caption => caption.Label.Equals(tab.Name));
                                if (newCaption != null)
                                    ((SubtitleTabItemViewModel) tab).Caption = newCaption;
                            }

                            var linkUrl = video != null ? GetVideoUrl(video) : GetAssetUrl(captionAsset);

                            foreach (var caption in message.Param.Captions.ToList())
                            {
                                var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                                var tabItem = subtitleVm.Tabs.SingleOrDefault(tab =>
                                    tab.Name.Equals(caption.Label) && tab.LanguageCode.Equals(caption.Language) &&
                                    tab.CountryCode.Equals(caption.Country));
                                tabItem?.SetAsDeployed();
                            }


                            _browser.Main.ShowMcmDeployConfirmDialog(video, captionAsset,
                                message.Param.Captions.ToList(), linkUrl);
                        }
                        else
                            // [resource]
                        {
                            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_ERROR,
                                "저장을 실패하였습니다.\n관리자에게 문의하십시오.",
                                MessageBoxButton.OK));
                        }
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
                _browser.Main.LoadingManager.Hide();
            }

            return;

            string GetVideoUrl(Video video)
            {
#if STAGING
                var hostUrl = "https://console.media.stg.continuum.co.kr"; // stage
#elif DEBUG
                var hostUrl = "http://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com"; // develop
#else
                //var hostUrl = "https://console.media.megazone.io";  // Production
                var hostUrl = "http://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com"; // develop
#endif
                return string.IsNullOrEmpty(video?.Id)
                    ? string.Empty
                    : $"{hostUrl}/contents/videos/{video.Id}";
            }

            string GetAssetUrl(CaptionAsset captionAsset)
            {
#if STAGING
                var hostUrl = "https://console.media.stg.continuum.co.kr"; // stage
#elif DEBUG
                var hostUrl = "http://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com"; // develop
#else
                //var hostUrl = "https://console.media.megazone.io";  // Production
                var hostUrl = "http://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com"; // develop
#endif
                return string.IsNullOrEmpty(captionAsset?.Id)
                    ? string.Empty
                    : $"{hostUrl}/contents/assets/{captionAsset.Id}";
            }
        }


        public async Task DeployAsync(Video video, CaptionAsset captionAsset, IEnumerable<Caption> captions,
            Action<bool, Video, CaptionAsset> finishAction)
        {
            var captionList = captions?.ToList() ?? new List<Caption>();
            if (!captionList.Any())
            {
                finishAction?.Invoke(false, video, captionAsset);
                return;
            }

            try
            {
                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;
                var uploadInputPath = await GetUploadInputPathAsync();

                // upload caption files.
                foreach (var caption in captionList)
                {
                    var uploadData = GetTextBy(caption);
                    var fileName = GetFileName(caption);
                    var uploadedPath = await _cloudMediaService.UploadCaptionFileAsync(
                        new UploadCaptionFileParameter(authorization, stageId, projectId, uploadData, fileName,
                            uploadInputPath, caption.Url), CancellationToken.None);
                    caption.Url = uploadedPath;
                }

                if (string.IsNullOrEmpty(captionAsset.Id))
                {
                    var createAsset = await _cloudMediaService.CreateCaptionAssetAsync(
                        new CreateCaptionAssetParameter(authorization, stageId, projectId, captionAsset.Name,
                            captionList),
                        CancellationToken.None);
                    //Debug.Assert(!string.IsNullOrEmpty(createAsset?.Id), "createAsset is null.");

                    if (!string.IsNullOrEmpty(video?.Id))
                    {
                        var originalVideo = await _cloudMediaService.GetVideoAsync(
                            new GetVideoParameter(authorization, stageId, projectId, video.Id), CancellationToken.None);

                        #region Video Update.

                        //if (originalVideo != null)
                        //{
                        //    var captionAssetList = originalVideo.Captions.ToList();
                        //    captionAssetList.Add(createAsset);

                        //    var updateVideo = new Video(originalVideo.Id, originalVideo.Name, originalVideo.Description,
                        //        originalVideo.Status, originalVideo.Duration, originalVideo.CreatedAt,
                        //        originalVideo.Version, originalVideo.ImageUrl, originalVideo.PrimaryPoster,
                        //        originalVideo.Origins, originalVideo.Sources, captionAssetList, originalVideo.Thumbnails,
                        //        originalVideo.Posters);

                        //    var updatedVideo = await _cloudMediaService.UpdateVideoAsync(
                        //        new UpdateVideoParameter(authorization, stageId, projectId, video.Id, updateVideo),
                        //        CancellationToken.None);

                        //    if (string.IsNullOrEmpty(updatedVideo?.Id))
                        //    {
                        //        // 등록된 asset 삭제.
                        //        await _cloudMediaService.DeleteCaptionAssetAsync(
                        //            new DeleteCaptionAssetParameter(authorization, stageId, projectId, createAsset.Id,
                        //                createAsset.Version),
                        //            CancellationToken.None);

                        //        finishAction?.Invoke(false, updatedVideo, createAsset);
                        //        return;
                        //    }

                        //    finishAction?.Invoke(true, updatedVideo, createAsset);
                        //    return;
                        //} 

                        #endregion

                        // 업데이트가 아니라, 추가된 Asset만 비디오에 등록한다.
                        var videoId = originalVideo.Id;
                        var version = originalVideo.Version;
                        var assetList = new List<string> {createAsset.Id};
                        var registeredCaptionAssets = await _cloudMediaService.RegisterCaptionAssetAsync(
                            new RegisterCaptionAssetParameter(authorization, stageId, projectId, videoId, version,
                                assetList),
                            CancellationToken.None);

                        finishAction?.Invoke(registeredCaptionAssets.Any(), originalVideo, createAsset);
                        return;
                    }

                    finishAction?.Invoke(true, video, createAsset);
                    return;
                }

                var updatedCaption = await _cloudMediaService.UpdateCaptionAsync(
                    new UpdateCaptionParameter(authorization, stageId, projectId, captionAsset.Id, captionList),
                    CancellationToken.None);

                //Debug.Assert(updatedCaption != null, "updatedCaption is null.");

                finishAction?.Invoke(!string.IsNullOrEmpty(updatedCaption?.Id), video, updatedCaption);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            finishAction?.Invoke(false, video, captionAsset);
        }

        private string GetFileName(Caption caption)
        {
            var url = caption.Url;
            if (string.IsNullOrEmpty(url))
                return
                    $"{caption.Label}_{caption.Language}_{caption.Country}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.vtt";

            var lastSlashIndex = url.LastIndexOf('/');

            var fileName = url.Substring(lastSlashIndex + 1, url.Length - lastSlashIndex - 1);
            return fileName;
        }

        private string GetTextBy(Caption caption)
        {
            var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            var tabItem = subtitleVm.Tabs.SingleOrDefault(tab =>
                tab.Name.Equals(caption.Label) && tab.LanguageCode.Equals(caption.Language) &&
                tab.CountryCode.Equals(caption.Country) && tab.Caption?.Id == caption.Id);
            var parser = SubtitleListItemParserProvider.Get(TrackFormat.WebVtt);
            var subtitles = tabItem.Rows.Select(s => s.ConvertToString(parser)).ToList();
            return _subtitleService.ConvertToText(subtitles, TrackFormat.WebVtt);
        }

        public async Task<string> GetUploadInputPathAsync()
        {
            var uploadTargetPath = string.Empty;
            var setting = await GetMcmSettingAsync();

            if (setting != null)
            {
                var s3Path = setting.Asset?.OutputStoragePrefix?.Value;
                var folderPath = setting.Asset?.OutputStoragePath?.Value;
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

        public Task<Settings> GetMcmSettingAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId),
                CancellationToken.None);
        }

        public void SetUploadInputPath(string uploadInputPath)
        {
            UploadInputPath = uploadInputPath;
        }

        public bool CanImportFile()
        {
            return VideoItem != null || CaptionAssetItem != null;
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
    }
}