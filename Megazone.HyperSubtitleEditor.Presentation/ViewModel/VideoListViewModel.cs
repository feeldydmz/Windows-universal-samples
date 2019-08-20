using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class VideoListViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _loadCommand;
        private ICommand _confirmCommand;

        private int _selectedPageIndex;
        private bool _isLoading;

        private int _totalCount;

        private IList<VideoItemViewModel> _videoItems;
        private VideoItemViewModel _selectedVideoItem;
        private CaptionAssetItemViewModel _selectedCaptionAsset;
        private IList<CaptionAssetItemViewModel> _captionItems;
        private ICommand _videoSelectionChangedCommand;
        private bool _isBusy;

        public VideoListViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(LoadAsync); }
        }

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
        }

        public ICommand VideoSelectionChangedCommand
        {
            get { return _videoSelectionChangedCommand = _videoSelectionChangedCommand ?? new RelayCommand(OnVideoSelectionChanged); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public CaptionAssetItemViewModel SelectedCaptionAsset
        {
            get => _selectedCaptionAsset;
            set => Set(ref _selectedCaptionAsset, value);
        }

        public IList<CaptionAssetItemViewModel> CaptionItems
        {
            get => _captionItems;
            set => Set(ref _captionItems, value);
        }

        public VideoItemViewModel SelectedVideoItem
        {
            get => _selectedVideoItem;
            set => Set(ref _selectedVideoItem, value);
        }

        public IList<VideoItemViewModel> VideoItems
        {
            get => _videoItems;
            set => Set(ref _videoItems, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => Set(ref _totalCount, value);
        }

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => Set(ref _selectedPageIndex, value);
        }

        private async void LoadAsync()
        {
            _isLoading = true;
            IsBusy = true;
            try
            {
                SelectedVideoItem = null;
                SelectedCaptionAsset = null;
                VideoItems?.Clear();
                VideoItems = null;
                CaptionItems?.Clear();
                CaptionItems = null;

                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedStage?.Id;

                var results = await _cloudMediaService.GetVideosAsync(new GetVideosParameter(authorization, stageId,
                    projectId, new Pagination(SelectedPageIndex)));
                TotalCount = results.TotalCount;
                var list = results.List?.Select(video => new VideoItemViewModel(video)).ToList();
                VideoItems = new ObservableCollection<VideoItemViewModel>(list);
            }
            finally
            {
                IsBusy = false;
                _isLoading = false;
            }
        }

        private async void OnVideoSelectionChanged()
        {
            if (_isLoading)
            {
                System.Diagnostics.Debug.WriteLine("isLoading");
                return;
            }
            // 선택된 비디오에서 caption asset을 선택하면, 자막정보를 가져온다.
            IsBusy = true;
            try
            {
                CaptionItems?.Clear();
                CaptionItems = null;
                var videoId = SelectedVideoItem?.Id;
                if (string.IsNullOrEmpty(videoId))
                    return;

                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedStage?.Id;

                var results = await _cloudMediaService.GetCaptionsAsync(new GetCaptionsParameter(authorization, stageId,
                    projectId, new Pagination(SelectedPageIndex), new Dictionary<string, string> { { "videoId", videoId } }));
                TotalCount = results.TotalCount;
                var list = results.List?.Select(asset => new CaptionAssetItemViewModel(asset)).ToList();
                CaptionItems = new ObservableCollection<CaptionAssetItemViewModel>(list);
            }
            finally
            {
                IsBusy = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool CanConfirm()
        {
            return !IsBusy && SelectedVideoItem != null;
        }

        private void Confirm()
        {
            // 선택된 video 정보를 메인 
            var video = SelectedVideoItem?.Source;
            var asset = SelectedCaptionAsset?.Source;
            var captionList = SelectedCaptionAsset?.Elements?.Where(caption => caption.IsSelected).Select(itemVm => itemVm.Source).ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new Subtitle.McmCaptionAssetOpenedMessage(this, new McmCaptionAssetOpenedMessageParameter(video, asset, captionList)));
            CloseAction?.Invoke();
        }
        public Action CloseAction { get; set; }
    }
}