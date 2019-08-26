﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    public class VideoListViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _backCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ICommand _captionSelectionChangedCommand;
        private ICommand _confirmCommand;

        private ICommand _enterCommand;
        private bool _isBusy;

        private bool _isShowCaption;

        private string _keyword;
        private IEnumerable<KeywordType> _keywordTypeItems;

        private ICommand _loadCaptionCommand;
        private ICommand _loadCommand;

        private ICommand _refreshCommand;
        private ICommand _searchCommand;
        private KeywordType _selectedKeywordType;
        private int _selectedPageIndex;
        private VideoItemViewModel _selectedVideoItem;

        private int _totalCount;
        private IList<VideoItemViewModel> _videoItems;

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

        public ICommand SearchCommand
        {
            get { return _searchCommand = _searchCommand ?? new RelayCommand<string>(SearchAsync); }
        }

        public ICommand LoadCaptionCommand
        {
            get
            {
                return _loadCaptionCommand =
                    _loadCaptionCommand ?? new RelayCommand<VideoItemViewModel>(LoadCaptionAsync);
            }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand = _refreshCommand ?? new RelayCommand(Refresh); }
        }


        public ICommand BackCommand
        {
            get { return _backCommand = _backCommand ?? new RelayCommand(Back); }
        }

        public ICommand EnterCommand
        {
            get { return _enterCommand = _enterCommand ?? new RelayCommand<string>(Enter); }
        }

        public ICommand CaptionSelectionChangedCommand
        {
            get
            {
                return _captionSelectionChangedCommand = _captionSelectionChangedCommand ??
                                                         new RelayCommand<CaptionElementItemViewModel>(
                                                             OnCaptionSelectionChanged);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
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

        public IEnumerable<KeywordType> KeywordTypeItems
        {
            get => _keywordTypeItems;
            set => Set(ref _keywordTypeItems, value);
        }

        public KeywordType SelectedKeywordType
        {
            get => _selectedKeywordType;
            set => Set(ref _selectedKeywordType, value);
        }

        public string Keyword
        {
            get => _keyword;
            set => Set(ref _keyword, value);
        }

        public bool IsShowCaption
        {
            get => _isShowCaption;
            set => Set(ref _isShowCaption, value);
        }


        public Action CloseAction { get; set; }

        private async void LoadAsync()
        {
            //KeywordTypeItems = new List<KeywordType>
            //{
            //    new KeywordType(Resource.CNT_NAME, "name"),
            //    new KeywordType(Resource.CNT_VIDEO_ID, "id")
            //};
            KeywordTypeItems = new List<KeywordType>
            {
                new KeywordType(Resource.CNT_NAME, "title"),
                new KeywordType(Resource.CNT_VIDEO_ID, "videoId")
            };

            if (SelectedKeywordType == null)
                SelectedKeywordType = KeywordTypeItems.First();

            await SearchVideoAsync();
        }

        private async void LoadCaptionAsync(VideoItemViewModel videoItem)
        {
            // 선택된 비디오에서 caption asset을 선택하면, 자막정보를 가져온다.
            IsBusy = true;
            try
            {
                IsShowCaption = true;
                ValidCancellationTokenSource();
                SelectedVideoItem = videoItem;
                
                var videoId = videoItem.Id;
                if (string.IsNullOrEmpty(videoId))
                    return;

                if (videoItem.CanUpdate)
                {
                    videoItem?.CaptionItems?.Clear();
                    var authorization = _signInViewModel.GetAuthorization();
                    var stageId = _signInViewModel.SelectedStage?.Id;
                    var projectId = _signInViewModel.SelectedStage?.Id;

                    var result = await _cloudMediaService.GetVideoAsync(
                        new GetVideoParameter(authorization, stageId, projectId, videoId), _cancellationTokenSource.Token);

                    videoItem.UpdateSource(result);
                }
            }
            finally
            {
                IsBusy = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void Refresh()
        {
            SearchAsync(Keyword);
        }

        private void Back()
        {
            IsShowCaption = false;
            if (IsBusy)
                _cancellationTokenSource.Cancel();
        }

        private void Enter(string keyword)
        {
            SearchAsync(keyword);
        }

        private async void SearchAsync(string keyword)
        {
            SelectedPageIndex = 0;
            var conditions = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(keyword))
                conditions.Add(SelectedKeywordType.Key, keyword);

            await SearchVideoAsync(conditions);
        }

        private async Task SearchVideoAsync(Dictionary<string, string> conditions = null)
        {
            ValidCancellationTokenSource();
            IsBusy = true;
            try
            {
                TotalCount = 0;
                SelectedVideoItem = null;
                VideoItems?.Clear();

                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedStage?.Id;

                var results = await _cloudMediaService.GetVideosAsync(
                    new GetVideosParameter(authorization, stageId, projectId, new Pagination(SelectedPageIndex),
                        conditions), _cancellationTokenSource.Token);

                TotalCount = results.TotalCount;
                VideoItems = new ObservableCollection<VideoItemViewModel>(
                    results.List?.Select(video => new VideoItemViewModel(video)).ToList() ??
                    new List<VideoItemViewModel>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            if (SelectedVideoItem == null)
                return;

            var captionAssetItem = SelectedVideoItem.CaptionItems.SingleOrDefault(assetItem =>
                assetItem.Elements.Any(element => element.Equals(item)));

            if (!SelectedVideoItem.SelectedCaptionAsset?.Equals(captionAssetItem) ?? true)
                SelectedVideoItem.SelectedCaptionAsset = captionAssetItem;

            SelectedVideoItem.Update();
        }

        private bool CanConfirm()
        {
            return !IsBusy && SelectedVideoItem != null;
        }

        private void Confirm()
        {
            // 선택된 video 정보를 메인 
            var video = SelectedVideoItem?.Source;
            var asset = SelectedVideoItem?.SelectedCaptionAsset?.Source;
            var selectedCaptionList =
                SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Where(caption => caption.IsSelected)
                    .Select(itemVm => itemVm.Source).ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new Subtitle.McmCaptionAssetOpenedMessage(this,
                new McmCaptionAssetOpenedMessageParameter(video, asset, selectedCaptionList)));
            CloseAction?.Invoke();
        }

        private void ValidCancellationTokenSource()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();
        }

        public class KeywordType
        {
            public KeywordType(string display, string key)
            {
                Display = display;
                Key = key;
            }

            public string Display { get; }
            public string Key { get; }
        }
    }
}