using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Megazone.Core.VideoTrack;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

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

        // Video / Caption 데이터 유무.
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

        // 작업대상인 Caption 정보.
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


        private void ResetAssetElement()
        {
            Console.WriteLine("ResetAssetElement");



            //MessageCenter.Instance.Send(new CloudMedia.CaptionResetMessage(this));
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
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CaptionElementCreateNewMessage>(OnCaptionElementCreateNew);
            MessageCenter.Instance.Regist<CloudMedia.DeployRequestedMessage>(Deploy);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.FileOpenedMessage>(OnFileOpened);
            MessageCenter.Instance.Regist<CloudMedia.CaptionOpenRequestedByIdMessage>(OnCaptionOpenByIdRequest);
        }

        private void UnregisterMessageHandlers()
        {
            MessageCenter.Instance.Unregist<CloudMedia.CaptionOpenRequestedMessage>(OnCaptionOpenRequest);
            MessageCenter.Instance.Unregist<CloudMedia.CaptionAssetRenameRequestedMessage>(RenameCaptionAsset);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CaptionElementCreateNewMessage>(OnCaptionElementCreateNew);
            MessageCenter.Instance.Unregist<CloudMedia.DeployRequestedMessage>(Deploy);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.FileOpenedMessage>(OnFileOpened);
            MessageCenter.Instance.Unregist<CloudMedia.CaptionOpenRequestedByIdMessage>(OnCaptionOpenByIdRequest);
        }


        private void OnFileOpened(Message.SubtitleEditor.FileOpenedMessage message)
        {
            IsOnlineData = VideoItem != null || CaptionAssetItem != null;
//                new CaptionAssetItemViewModel(new CaptionAsset(null, "untitle", null, null, null, null, 0, 0, null,
//                    null));

            HasWorkData = true;
        }

        private void OnCaptionElementCreateNew(Message.SubtitleEditor.CaptionElementCreateNewMessage message)
        {
            IsOnlineData = false;

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
                    var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                    var stageId = _signInViewModel.SelectedStage?.Id;
                    var projectId = _signInViewModel.SelectedProject?.ProjectId;
                    var assetId = message.CaptionAsset?.Id;

                    var captionAsset = await _cloudMediaService.UpdateCaptionAssetAsync(
                        new UpdateCaptionAssetParameter(authorization, stageId, projectId, assetId, assetName),
                        CancellationToken.None);

                    if (captionAsset != null)
                        CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset, SourceLocationKind.Cloud);
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
                        null,""));
            }
        }

        private async void OnCaptionOpenRequest(CloudMedia.CaptionOpenRequestedMessage requestedMessage)
        {
            //var isOnline = requestedMessage.Param.IsOnline;
            //var videoId = requestedMessage.Param.Video?.Id;
            //var captionAssetId = requestedMessage.Param.Asset?.Id;
            //var captionElements = requestedMessage.Param.Captions;


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
                //await OpenAsset(videoId, captionAssetId, captionElements, isOnline);

                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
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

                    if (VideoItem.CaptionAssetList != null)
                        VideoItem.CaptionAssetList.SelectedCaptionAssetItem =
                            VideoItem.CaptionAssetList.CaptionAssetItems?.SingleOrDefault(asset => asset.Id.Equals(assetId));
                }

                var captionAsset = string.IsNullOrEmpty(assetId)
                    ? null
                    : await _cloudMediaService.GetCaptionAssetAsync(
                        new GetAssetParameter(authorization, stageId, projectId, assetId), CancellationToken.None);

                if (captionAsset != null)
                    CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset, SourceLocationKind.Cloud);

                HasWorkData = video != null || captionAsset != null;

                //var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                //subtitleViewModel.OnOpenCaptionRequest(requestedMessage);
                MessageCenter.Instance.Send(
                    new Message.SubtitleEditor.CaptionOpenRequestedMessage(this, requestedMessage.Param));

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

        private void OnCaptionOpenByIdRequest(CloudMedia.CaptionOpenRequestedByIdMessage message)
        {
            var videoId = message.Param.VideoId;
            var captionAssetId = message.Param.CaptionAssetId;
            var captionElements = message.Param.CaptionElements;

            LoadAssetById(videoId, captionAssetId, captionElements);
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
                            if (captionAsset == null)
                                return;
                            //if (captionAsset != null)
                            //    CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset);
                            HasWorkData = true;
                            IsOnlineData = true;

                            var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                            foreach (var tab in subtitleViewModel.Tabs)
                            {
                                var newCaption =
                                    captionAsset?.Elements?.FirstOrDefault(caption => caption.Label.Equals(tab.Name));
                                if (newCaption != null) {
                                    if (tab is SubtitleTabItemViewModel subtitleTabItemViewModel)
                                    {
                                        subtitleTabItemViewModel.Caption = newCaption;
                                        subtitleTabItemViewModel.UpdateOriginData();
                                        subtitleTabItemViewModel.SetAsDeployed();
                                    }
                                }
                            }
                            var linkUrl = video != null ? GetVideoUrl(video) : GetAssetUrl(captionAsset);

                            if (CaptionAssetItem != null)
                            {
                                var diffList = CaptionAssetItem.Elements
                                    ?.Where(e => captionAsset.Elements.All(u => u.Id != e.Id)).Select(c => c.Source)
                                    .ToList();

                                var mergedCaptionAssets =
                                    diffList.Union(captionAsset.Elements).OrderBy(d => d.Id).ToList();
                                captionAsset.Elements = mergedCaptionAssets;
                            }
                            
                            CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset, SourceLocationKind.Cloud);

                            //foreach (var caption in message.Param.Captions.ToList())
                            //{
                            //    var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                            //    var tabItem = subtitleViewModel.Tabs.SingleOrDefault(tab =>
                            //        tab.Name.Equals(caption.Label) && tab.LanguageCode.Equals(caption.Language) &&
                            //        tab.CountryCode.Equals(caption.Country));
                            //    tabItem?.SetAsDeployed();
                            //}


                            _browser.Main.ShowMcmDeployConfirmDialog(video, captionAsset,
                                message.Param.Captions.ToList(), linkUrl);

                            subtitleViewModel.CollapseAllPopup();
                        }
                        else
                        {
                            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_ERROR,
                                Resource.MSG_SAVE_FAIL,
                                MessageBoxButton.OK,
                                Application.Current.MainWindow));
                        }
                    });
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);

                _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_ERROR,
                    Resource.MSG_SAVE_FAIL,
                    MessageBoxButton.OK,
                    Application.Current.MainWindow));
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
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                //--------------
                // Create Caption 
                //--------------
                if (string.IsNullOrEmpty(captionAsset.Id))
                {
                    var folderPath = "";
                    if (video != null)
                    {
                        folderPath = video.Sources.First().FolderPath;
                        folderPath = folderPath.Substring(0, folderPath.LastIndexOf("/"));
                    }

                    var assetName = captionAsset.Name;
                    var createAsset = await _cloudMediaService.CreateCaptionAssetAsync(
                        new CreateCaptionAssetParameter(authorization, stageId, projectId, assetName,
                            folderPath),
                        CancellationToken.None);

                    var assetId = createAsset?.Id;

                    if (!string.IsNullOrEmpty(assetId))
                    {
                        var version = createAsset.Version;
                        List<Caption> newCaptions = new List<Caption>();
                        // upload caption files.
                        foreach (var caption in captionList)
                        {
                            var newCaptionElement =  await CreateCaptionElementWithUploadAsync(assetId, version, caption);
                            newCaptions.Add(newCaptionElement);
                            ++version;
                        }

                        createAsset.Elements = newCaptions;
                    }

                    if (!string.IsNullOrEmpty(video?.Id))
                    {
                        
                        var originalVideo = await _cloudMediaService.GetVideoAsync(
                            new GetVideoParameter(authorization, stageId, projectId, video?.Id),
                            CancellationToken.None);

                        // 여러개의 Caption Asset을 동시에 편집하지 않으므로 
                        // 업데이트 하지 않고 새로 생성된 Caption Caption 만 비디오에 등록한다.
                        var registeredCaptionAssets = await RegisterCaptionAssetAsync(originalVideo, createAsset);

                        finishAction?.Invoke(registeredCaptionAssets.Any(), originalVideo, createAsset);

                        return;
                    }

                    finishAction?.Invoke(true, video, createAsset);
                    return;
                }

                //-------------
                // Update Caption
                //-------------
                var updatedCaptionAssets = await UpdateCaptionAssetAsync(captionAsset.Id, captionList);


                //var diffList = CaptionAssetItem.Elements.Where(e => updatedCaptionAssets.Elements.All(u => u.Id != e.Id)).Select(c => c.Source).ToList();


                //var mergedCaptionAssets = diffList.Union(updatedCaptionAssets.Elements);

                //foreach (var asset in mergedCaptionAssets)
                //{
                //    Debug.WriteLine($"asset : {asset.Label}");
                //}

                //updatedCaptionAssets.Elements = mergedCaptionAssets;
                //CaptionAssetItem.

                //var 

                //foreach (var captionElementItem in CaptionAssetItem.Elements)
                //{
                //    updatedCaptionAssets.Elements.FirstOrDefault(c => c.Id == captionElementItem.Id);
                //}

                //updatedCaptionAssets?.Elements.Union(CaptionAssetItem.Elements, new CaptionComparer());
                ////Debug.Assert(updatedCaption != null, "updatedCaption is null.");

                finishAction?.Invoke(!string.IsNullOrEmpty(captionAsset.Id), video, updatedCaptionAssets);
                return;
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
                throw;
            }
        }
        public class CaptionComparer : IEqualityComparer<Caption>
        {

            #region IEqualityComparer<Skill> Members

            public bool Equals(Caption skill, Caption skillToCompare)
            {
                return skill.Id == skillToCompare.Id;
            }

            public int GetHashCode(Caption obj)
            {
                return obj.Id.GetHashCode();
            }

            #endregion
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
                tab.Name.Equals(caption.Label) 
                && tab.LanguageCode.Equals(caption.Language) 
                && tab.CountryCode.Equals(caption.Country) 
                && tab.Caption?.Id == caption.Id);



            var parser = SubtitleListItemParserProvider.Get(SubtitleFormatKind.WebVtt);
            var subtitles = tabItem.Rows.Select(s => s.ConvertToString(parser)).ToList();
            return _subtitleService.ConvertToText(subtitles, SubtitleFormatKind.WebVtt);
        }

        private async Task<Caption> CreateCaptionElementWithUploadAsync(string assetId, int assetVersion, Caption caption)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                var uploadData = GetTextBy(caption);
                var fileName = GetFileName(caption);

                var assetUploadUrl = await UploadCaptionFileAsync(assetId, fileName, uploadData, true);

                if (assetUploadUrl != null)
                {
                    caption.Url = assetUploadUrl.Url;
                    var response = await _cloudMediaService.CreateCaptionElementsAsync(
                        new CreateAssetElementParameter(authorization, stageId, projectId, assetId, assetVersion,
                            caption), CancellationToken.None);

                    if (response == null)
                        throw new Exception($"Caption element, create fail (element name : {caption.Label})");

                    return response;
                }
                else
                {
                    throw  new Exception($"Caption element, subtitle file upload fail (assetId : {assetId}, fileName : {fileName}) ");
                }

            } catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<Caption> UpdateCaptionElementWithUploadAsync(string assetId, int assetVersion, Caption caption)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                var uploadData = GetTextBy(caption);

                var fileName = GetFileName(caption);

                var assetUploadUrl = await UploadCaptionFileAsync(assetId, fileName, uploadData, false);

                if (assetUploadUrl != null)
                {
                    return await _cloudMediaService.UpdateCaptionElementAsync(
                        new UpdateCaptionParameter(authorization, stageId, projectId, assetId, assetVersion,
                            caption),
                        CancellationToken.None);
                }
                
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<AssetUploadUrl> UploadCaptionFileAsync(string assetId, string fileName, string uploadData, bool isAttacheId)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                var uploadPath = await _cloudMediaService.GetUploadUrlAsync(
                    new GetUploadUrlParameter(authorization, stageId, projectId, assetId, fileName, isAttacheId),
                    CancellationToken.None);

                ////업로드 로직
                var isUploadSuccess = await _cloudMediaService.UploadCaptionFileAsync(
                    new UploadCaptionFileParameter(uploadPath.UploadUrl, uploadData),
                    CancellationToken.None);

                return isUploadSuccess ? uploadPath : null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        private async Task<CaptionAsset> UpdateCaptionAssetAsync(string assetId, List<Caption>  captionList)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                var asset = await GetCaptionAssetAsync(assetId);

                if (asset == null)
                    throw new Exception("asset is null");

                var oldCaptions = asset.Elements?.ToList() ?? new List<Caption>();
                List<Caption> updatingCaptionList = new List<Caption>();
                if (captionList.Any())
                {
                    var createCaptionList = new List<Caption>();
                    foreach (var workingCaption in captionList)
                    {
                        var findCaption =
                            oldCaptions.SingleOrDefault(caption => caption.Id.Equals(workingCaption.Id));

                        // findCaption 에 값이 없다면 새로 추가된 caption
                        if (findCaption == null)
                        {
                            createCaptionList.Add(workingCaption);
                        }
                        else
                        {
                            updatingCaptionList.Add(workingCaption);
                        }
                    }

                    List<Caption> createdCaptionAssetList = new List<Caption>();
                    if (createCaptionList.Any())
                    {
                        // 추가된 자막 파일은 지정된 Asset에 추가한다.
                        foreach (var caption in createCaptionList)
                        {
                            var newCaptionElement =
                                await CreateCaptionElementWithUploadAsync(assetId, asset.Version, caption);

                            if (newCaptionElement != null)
                                createdCaptionAssetList.Add(newCaptionElement);
                        }
                    }

                    // update를 하면 asset의 version이 변경 되므로 제일 마지막 로직에 배치
                    for (int i = 0; i < updatingCaptionList.Count; i++)
                    {
                        var caption = updatingCaptionList[i];
                        var response =
                            await UpdateCaptionElementWithUploadAsync(assetId, asset.Version, caption);

                        createdCaptionAssetList.Insert(i, caption);

                        if (response == null)
                        {
                            throw new Exception($"caption update file. name: {caption}");
                        }
                    }

                    var updateAsset = new CaptionAsset(asset.Id, asset.Name, asset.Status, asset.Type, asset.MediaType,
                        asset.IngestType, asset.Duration, asset.Version, asset.CreatedAt, createdCaptionAssetList, asset.FolderPath);

                    return updateAsset;
                }

                return null;
            }
            catch (Exception) {
                throw;
            }
        }



        private async Task<IEnumerable<CaptionAsset>> RegisterCaptionAssetAsync(Video video, CaptionAsset captionAsset)
        {
            var authorization = _signInViewModel.GetAuthorizationAsync().Result;
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;

            

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
            var assetList = new List<string> { captionAsset.Id };
            var registeredCaptionAssets = await _cloudMediaService.RegisterCaptionAssetAsync(
                new RegisterCaptionAssetParameter(authorization, stageId, projectId, video.Id, video.Version,
                    assetList),
                CancellationToken.None);

            return registeredCaptionAssets;
        }

        public Task<Settings> GetMcmSettingAsync()
        {
            var authorization = _signInViewModel.GetAuthorizationAsync().Result;
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return _cloudMediaService.GetSettingsAsync(new GetSettingsParameter(authorization, stageId, projectId),
                CancellationToken.None);
        }


        public bool CanImportFile()
        {
            return true;
//            return VideoItem != null || CaptionAssetItem != null;
        }

        public async void LoadAssetById(string videoId, string assetId, IEnumerable<string> captionIds)
        {
            _logger.Debug.Write("start");
            _browser.Main.LoadingManager.Show();

            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage.Id;
                var projectId = _signInViewModel.SelectedProject.ProjectId;

                Cloud.Media.Domain.Video video = null;
                CaptionAsset captionAsset = null;
                List<Caption> captionElementItems = null;

                if (!string.IsNullOrEmpty(videoId))
                {
                    video = await _cloudMediaService.GetVideoAsync(
                        new GetVideoParameter(authorization, stageId, projectId, videoId),
                        CancellationToken.None);
                }

                if (!string.IsNullOrEmpty(assetId))
                {
                    captionAsset = await GetCaptionAssetAsync(assetId);

                    if (captionIds is List<string> captions)
                    {
                        if (captions.Count > 0)
                        {
                            captionElementItems = new List<Caption>();
                            //captionElementItems =
                            //    captionAsset?.Elements?.Where(caption => captions.Contains(caption.Id)).ToList();

                            if (captionAsset?.Elements != null)
                            {
                                captionElementItems.AddRange(
                                    (captionAsset?.Elements).Where(element => captions.Contains(element.Id)));
                            }
                        }
                    }
                }

                await OpenAsset(new CloudMedia.CaptionOpenRequestedMessage(this,
                    new CaptionOpenMessageParameter(video, captionAsset, captionElementItems, true)));

                _logger.Debug.Write("end");
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                _browser.Main.LoadingManager.Hide();
            }
        }

        //private async Task OpenAsset(string videoId, string captionAssetId, IEnumerable<Caption> captionElements, bool isOnline)
        private async Task OpenAsset(CloudMedia.CaptionOpenRequestedMessage message)
        {
            _logger.Debug.Write("start");

            var isOnline = message.Param.IsOnline;
            var video = message.Param.Video;
            var captionAsset = message.Param.Asset;
            var captionElements = message.Param.Captions;

            // 초기화
            Initialize();
            IsOnlineData = isOnline;

            if (IsBusy)
                return;

            IsBusy = true;
            IsLoading = true;
            _browser.Main.LoadingManager.Show();
            try
            {
                //var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                //var stageId = _signInViewModel.SelectedStage?.Id;
                //var projectId = _signInViewModel.SelectedProject?.ProjectId;

                //var video = string.IsNullOrEmpty(videoId)
                //    ? null
                //    : await _cloudMediaService.GetVideoAsync(
                //        new GetVideoParameter(authorization, stageId, projectId, videoId), CancellationToken.None);

                if (video != null)
                {
                    VideoItem = new VideoItemViewModel(video);

                    if (VideoItem.CaptionAssetList != null)
                        VideoItem.CaptionAssetList.SelectedCaptionAssetItem =
                            VideoItem.CaptionAssetList.CaptionAssetItems?.SingleOrDefault(asset => asset.Id.Equals(captionAsset?.Id));
                }

                //var captionAsset = string.IsNullOrEmpty(captionAssetId)
                    //? null
                    //: await _cloudMediaService.GetCaptionAssetAsync(
                    //    new GetAssetParameter(authorization, stageId, projectId, captionAssetId), CancellationToken.None);

                if (captionAsset != null)
                    CaptionAssetItem = new CaptionAssetItemViewModel(captionAsset, SourceLocationKind.Cloud);

                HasWorkData = video != null || captionAsset != null;

                //var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
                //subtitleViewModel.OnOpenCaptionRequest(requestedMessage);
                MessageCenter.Instance.Send(
                    new Message.SubtitleEditor.CaptionOpenRequestedMessage(this, message.Param));

                _recentlyLoader.Save(new RecentlyItem.OnlineRecentlyCreator().SetVideo(video)
                    .SetCaptionAsset(captionAsset).SetCaptions(captionElements).Create());

                _logger.Debug.Write("end");
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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public async Task<CaptionAsset> GetCaptionAssetAsync(string captionAssetId)
        {
            if (string.IsNullOrEmpty(captionAssetId))
                return null;

            var authorization = _signInViewModel.GetAuthorizationAsync().Result;
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return await _cloudMediaService.GetCaptionAssetAsync(
                new GetAssetParameter(authorization, stageId, projectId, captionAssetId), CancellationToken.None);
        }

        public async Task<Video> GetVideoAsync(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
                return null;

            var authorization = _signInViewModel.GetAuthorizationAsync().Result;
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId = _signInViewModel.SelectedProject.ProjectId;
            return await _cloudMediaService.GetVideoAsync(
                new GetVideoParameter(authorization, stageId, projectId, videoId), CancellationToken.None);
        }
    }
}