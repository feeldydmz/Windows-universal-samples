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

        private ICommand _captionAssetSectionChangedCommand;

        private ICommand _captionSelectionChangedCommand;
        private ICommand _confirmCommand;

        private TimeSpan _durationEndTime;

        private ICommand _durationEndTimeChangedCommand;

        private TimeSpan _durationStartTime;

        private ICommand _durationStartTimeChangedCommand;

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

        private ICommand _selectedPageNoChangedCommand;
        private int _selectedPageNo;
        private VideoItemViewModel _selectedVideoItem;

        private int _totalCount;
        private IList<VideoItemViewModel> _videoItems;

        public VideoListViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        public ICommand CaptionAssetSectionChangedCommand
        {
            get
            {
                return _captionAssetSectionChangedCommand =
                    _captionAssetSectionChangedCommand ?? new RelayCommand(OnCaptionAssetSectionChanged);
            }
        }

        public ICommand SelectedPageNoChangedCommand
        {
            get
            {
                return _selectedPageNoChangedCommand =
                    _selectedPageNoChangedCommand ?? new RelayCommand<int>(OnSelectedPageNoChanged);
            }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand(Confirm, CanConfirm); }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand = _searchCommand ?? new RelayCommand<string>(Search); }
        }

        public ICommand LoadCaptionCommand
        {
            get
            {
                return _loadCaptionCommand = _loadCaptionCommand ??
                                             new RelayCommand<VideoItemViewModel>(LoadCaptionAsync, CanLoadCaption);
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
                                                             OnCaptionSelectionChanged, CanCaptionSelectionChanged);
            }
        }

        public ICommand DurationStartTimeChangedCommand
        {
            get
            {
                return _durationStartTimeChangedCommand =
                    _durationStartTimeChangedCommand ?? new RelayCommand(OnDurationStartTimeChanged);
            }
        }

        public ICommand DurationEndTimeChangedCommand
        {
            get
            {
                return _durationEndTimeChangedCommand =
                    _durationEndTimeChangedCommand ?? new RelayCommand(OnDurationEndTimeChanged);
            }
        }

        public TimeSpan DurationStartTime
        {
            get => _durationStartTime;
            set => Set(ref _durationStartTime, value);
        }

        public TimeSpan DurationEndTime
        {
            get => _durationEndTime;
            set => Set(ref _durationEndTime, value);
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

        public int SelectedPageNo
        {
            get => _selectedPageNo;
            set => Set(ref _selectedPageNo, value);
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
        public Action<string> SetTitleAction { get; set; }

        private void OnSelectedPageNoChanged(int selectedPageNo)
        {
            SearchAsync(Keyword, selectedPageNo - 1, true);
        }

        private void OnDurationEndTimeChanged()
        {
            if (DurationEndTime.TotalSeconds < DurationStartTime.TotalSeconds)
                DurationStartTime = TimeSpan.FromSeconds(DurationEndTime.TotalSeconds);
        }

        private void OnDurationStartTimeChanged()
        {
            if (DurationEndTime.TotalSeconds < DurationStartTime.TotalSeconds)
                DurationEndTime = TimeSpan.FromSeconds(DurationStartTime.TotalSeconds);
        }

        private void OnCaptionAssetSectionChanged()
        {
            SelectedVideoItem?.SelectedCaptionAsset?.SelectAll();
            SelectedVideoItem?.Update();
            if (SelectedVideoItem?.CaptionItems != null)
                foreach (var captionAssetItem in SelectedVideoItem.CaptionItems)
                    if (!captionAssetItem.Equals(SelectedVideoItem?.SelectedCaptionAsset))
                        captionAssetItem.Initialize();
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanCaptionSelectionChanged(CaptionElementItemViewModel arg)
        {
            return SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Any(element => element.Equals(arg)) ?? false;
        }

        private void Load()
        {
#if STAGE
            KeywordTypeItems = new List<KeywordType>
            {
                new KeywordType(Resource.CNT_NAME, "title"),
                new KeywordType(Resource.CNT_VIDEO_ID, "videoId")
            };
#else
            KeywordTypeItems = new List<KeywordType>
            {
                new KeywordType(Resource.CNT_NAME, "name"),
                new KeywordType(Resource.CNT_VIDEO_ID, "id")
            };
#endif
            if (SelectedKeywordType == null)
                SelectedKeywordType = KeywordTypeItems.First();

            SearchAsync(Keyword, 0);
        }

        private bool CanLoadCaption(VideoItemViewModel videoItem)
        {
            return true; //videoItem?.CaptionItems?.Any() ?? false;
        }

        private async void LoadCaptionAsync(VideoItemViewModel videoItem)
        {
            // 선택된 비디오에서 caption asset을 선택하면, 자막정보를 가져온다.
            IsBusy = true;
            try
            {
                SetTitleAction?.Invoke($"{Resource.CNT_VIDEO} - {videoItem.Name}");
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
                        new GetVideoParameter(authorization, stageId, projectId, videoId),
                        _cancellationTokenSource.Token);

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
            SearchAsync(Keyword, 0);
        }

        private void Back()
        {
            SelectedVideoItem?.Update();
            SetTitleAction?.Invoke($"{Resource.CNT_VIDEO}");
            IsShowCaption = false;
            if (IsBusy)
                _cancellationTokenSource.Cancel();
        }

        private void Enter(string keyword)
        {
            SearchAsync(keyword, 0);
        }

        private void Search(string keyword)
        {
            SearchAsync(keyword, 0);
        }

        private async void SearchAsync(string keyword, int pageOffset, bool isPaging = false)
        {
            var conditions = MakeSearchConditions(keyword, DurationStartTime, DurationEndTime);
            await SearchVideoAsync(pageOffset, conditions, isPaging);
        }

        private async Task SearchVideoAsync(int pageOffset, Dictionary<string, string> conditions, bool isPaging)
        {
            ValidCancellationTokenSource();
            IsBusy = true;
            try
            {
                if (!isPaging)
                {
                    TotalCount = 0;
                    VideoItems?.Clear();
                }
                SelectedVideoItem = null;

                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedStage?.Id;

                var results = await _cloudMediaService.GetVideosAsync(
                    new GetVideosParameter(authorization, stageId, projectId, new Pagination(pageOffset),
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

        private Dictionary<string, string> MakeSearchConditions(string keyword, TimeSpan startDuration, TimeSpan endDuration)
        {
            var conditions = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(keyword))
                conditions.Add(SelectedKeywordType.Key, keyword);

#if STAGE
            if (startDuration.TotalSeconds > 0 || endDuration.TotalSeconds > 0)
            {
                var startValue = startDuration.TotalMilliseconds;
                var endValue = endDuration.TotalSeconds > startDuration.TotalSeconds
                    ? endDuration.TotalMilliseconds
                    : startDuration.TotalMilliseconds + 999;

                conditions.Add("beginDuration", $"{startValue}");
                conditions.Add("endDuration", $"{endValue}");
            }
#else
            if (startDuration.TotalSeconds > 0 || endDuration.TotalSeconds > 0)
            {
                var startTime = startDuration.TotalMilliseconds;
                var endTime =
                    endDuration.TotalSeconds > startDuration.TotalSeconds
                        ? endDuration.TotalMilliseconds
                        : startDuration.TotalMilliseconds + 999;
                conditions.Add("duration", $"{startTime}~{endTime}");
            }
#endif
            return conditions;
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            if (SelectedVideoItem == null)
                return;

            var captionAssetItem = SelectedVideoItem.CaptionItems.SingleOrDefault(assetItem =>
                assetItem.Elements.Any(element => element.Equals(item)));

            if (!SelectedVideoItem.SelectedCaptionAsset?.Equals(captionAssetItem) ?? true)
            {
                SelectedVideoItem.CaptionItems?.ToList().ForEach(asset =>
                {
                    if (!asset.Equals(captionAssetItem))
                        asset.Initialize();
                });
                SelectedVideoItem.SelectedCaptionAsset = captionAssetItem;
            }

            SelectedVideoItem.Update();
        }

        private bool CanConfirm()
        {
            if (IsShowCaption)
            {
                if (IsBusy)
                    return false;

                if (SelectedVideoItem?.CaptionItems?.Any() ?? false)
                {
                    if (SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Any() ?? false)
                        return SelectedVideoItem.SelectedCaptionCount > 0;
                    return true;
                }

                return false;
            }

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