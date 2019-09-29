using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    // TODO: 기존 로직과 온라인에서 관리해야하는 롤이 추가되변서, 기능이 여기 저기 혼재 되어 있다. 정리 필요.
    // 저장/오픈/게시등 데이터 관리 뷰모델.
    // SubtitleViewModel은 편집 관련 
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class WorkBarViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ILogger _logger;
        private readonly RecentlyLoader _recentlyLoader;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _editAssetNameCommand;
        private bool _hasData;
        private bool _isOnlineData;
        private bool _isOpenVideoInfoPopup;
        private ICommand _loadCommand;
        private ICommand _openVideoInfoPopupCommand;
        private CaptionAssetItemViewModel _selectedCaptionAssetItem;
        private ICommand _unloadCommand;
        private VideoItemViewModel _videoItem;

        public WorkBarViewModel(ICloudMediaService cloudMediaService, ILogger logger, SignInViewModel signInViewModel,
            RecentlyLoader recentlyLoader)
        {
            _cloudMediaService = cloudMediaService;
            _logger = logger;
            _signInViewModel = signInViewModel;
            _recentlyLoader = recentlyLoader;
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

        public bool HasData
        {
            get => _hasData;
            set => Set(ref _hasData, value);
        }

        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }

        public CaptionAssetItemViewModel SelectedCaptionAssetItem
        {
            get => _selectedCaptionAssetItem;
            set => Set(ref _selectedCaptionAssetItem, value);
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand UnloadCommand
        {
            get { return _unloadCommand = _unloadCommand ?? new RelayCommand(Unload); }
        }

        public ICommand OpenVideoInfoPopupCommand
        {
            get
            {
                return _openVideoInfoPopupCommand =
                    _openVideoInfoPopupCommand ?? new RelayCommand(() => { IsOpenVideoInfoPopup = true; });
            }
        }

        public ICommand EditAssetNameCommand
        {
            get { return _editAssetNameCommand = _editAssetNameCommand ?? new RelayCommand(EditAssetName); }
        }

        private void EditAssetName()
        {
            // 에셋 이름 편집창 띄우기.
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
            MessageCenter.Instance.Regist<CloudMedia.CaptionOpenMessage>(OnCaptionOpenRequest);
        }

        private void UnregisterMessageHandlers()
        {
            MessageCenter.Instance.Unregist<CloudMedia.CaptionOpenMessage>(OnCaptionOpenRequest);
        }

        private async void OnCaptionOpenRequest(CloudMedia.CaptionOpenMessage message)
        {
            if (message.Param?.Video == null)
                return;

            // 초기화
            IsOnlineData = false;
            HasData = false;

            try
            {
                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;
                var videoId = message.Param.Video?.Id;
                var assetId = message.Param.Asset?.Id;
                var captions = message.Param.Captions;

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
                    SelectedCaptionAssetItem = new CaptionAssetItemViewModel(captionAsset);

                HasData = video != null || captionAsset != null;
                IsOnlineData = true;
                _recentlyLoader.Save(new RecentlyItem.OnlineRecentlyCreator().SetVideo(video)
                    .SetCaptionAsset(captionAsset).SetCaptions(captions).Create());
            }
            catch (Exception e)
            {
                HasData = false;
                _logger.Error.Write(e);
            }
        }
    }
}