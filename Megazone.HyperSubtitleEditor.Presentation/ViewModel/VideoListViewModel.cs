using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class VideoListViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private ICommand _addDataCommand;
        private ICommand _backCommand;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ICommand _captionAssetSectionChangedCommand;
        private ICommand _captionSelectionChangedCommand;
        private ICommand _closeCommand;
        private ICommand _confirmCommand;

        private TimeSpan _durationEndTime;

        private ICommand _durationEndTimeChangedCommand;

        private TimeSpan _durationStartTime;

        private ICommand _durationStartTimeChangedCommand;

        private ICommand _enterCommand;

        private ICommand _initializeCommand;
        private bool _isBusy;

        private bool _isConfirmButtonVisible;

        private bool _isInitialized;
        private bool _isLoading;

        private bool _isNextButtonVisible = true;
        private bool _isOpen;
        private bool _isShowCaption;

        private string _keyword;
        private IEnumerable<DisplayItem> _keywordTypeItems;

        private ICommand _loadCaptionCommand;
        private ICommand _loadCommand;

        private ICommand _nextStepCommand;

        private ICommand _openCommand;

        private ICommand _refreshCommand;
        private ICommand _searchCommand;
        private DisplayItem _selectedKeywordType;
        private int _selectedPageNo = 1;

        private ICommand _selectedPageNoChangedCommand;
        private VideoItemViewModel _selectedVideoItem;

        private int _totalCount;
        private IList<VideoItemViewModel> _videoItems;

        public VideoListViewModel(IBrowser browser, ICloudMediaService cloudMediaService,
            SignInViewModel signInViewModel)
        {
            _browser = browser;
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

        public ICommand InitializeCommand
        {
            get { return _initializeCommand = _initializeCommand ?? new RelayCommand(Initialize); }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand AddDataCommand
        {
            get { return _addDataCommand = _addDataCommand ?? new RelayCommand(AddData); }
        }

        public ICommand NextStepCommand
        {
            get { return _nextStepCommand = _nextStepCommand ?? new RelayCommand(NextStep, CanNextStep); }
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

        public bool IsNextButtonVisible
        {
            get => _isNextButtonVisible;
            set => Set(ref _isNextButtonVisible, value);
        }

        public bool IsConfirmButtonVisible
        {
            get => _isConfirmButtonVisible;
            set => Set(ref _isConfirmButtonVisible, value);
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

        public IEnumerable<DisplayItem> KeywordTypeItems
        {
            get => _keywordTypeItems;
            set => Set(ref _keywordTypeItems, value);
        }

        public DisplayItem SelectedKeywordType
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

        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
        }

        public Action OnLoadAction { get; set; }
        public Action CloseAction { get; set; }
        public Action<string> SetTitleAction { get; set; }

        public ICommand OpenCommand
        {
            get { return _openCommand = _openCommand ?? new RelayCommand(Open); }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand = _closeCommand ?? new RelayCommand(Close); }
        }

        private async void AddData()
        {
            ValidCancellationTokenSource();
            IsBusy = true;

            try
            {
                var conditions = MakeSearchConditions(Keyword, DurationStartTime, DurationEndTime);
                var nextPageIndex = SelectedPageNo;
                var countPerPage = 30;
                var results = await GetVideoListAsync(new Pagination(nextPageIndex, countPerPage), conditions,
                    _cancellationTokenSource.Token);

                if (results.List.Any())
                {
                    if (VideoItems.Any())
                        foreach (var videoItem in results.List)
                            VideoItems.Add(new VideoItemViewModel(videoItem));
                    else
                        VideoItems = new ObservableCollection<VideoItemViewModel>(
                            results.List?.Select(video => new VideoItemViewModel(video)).ToList() ??
                            new List<VideoItemViewModel>());
                    SelectedPageNo += 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanNextStep()
        {
            return SelectedVideoItem != null;
        }

        private void NextStep()
        {
            if (SelectedVideoItem != null) LoadCaptionAsync(SelectedVideoItem);
        }

        private async void OnSelectedPageNoChanged(int selectedPageNo)
        {
            if (_isLoading)
                return;
            await SearchAsync(Keyword, selectedPageNo - 1, true);
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
            if (SelectedVideoItem?.CaptionAssetItems != null)
                foreach (var captionAssetItem in SelectedVideoItem.CaptionAssetItems)
                    if (!captionAssetItem.Equals(SelectedVideoItem?.SelectedCaptionAsset))
                        captionAssetItem.Initialize();
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanCaptionSelectionChanged(CaptionElementItemViewModel arg)
        {
            return SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Any(element => element.Equals(arg)) ?? false;
        }

        private void Initialize()
        {
            _isInitialized = false;

            KeywordTypeItems = new List<DisplayItem>
            {
                new DisplayItem(Resource.CNT_NAME, "name"),
                new DisplayItem(Resource.CNT_VIDEO_ID, "id")
            };

            if (SelectedKeywordType == null)
                SelectedKeywordType = KeywordTypeItems.First();
            SelectedPageNo = 1;
            _isInitialized = true;
        }

        public void ClearSearchParameter()
        {
            Keyword = "";
            DurationStartTime = TimeSpan.FromSeconds(0);
            DurationEndTime = TimeSpan.FromSeconds(0);
        }

        private void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
            CloseAction?.Invoke();
        }

        private async void Load()
        {
            if (!_isInitialized)
                Initialize();

            if (_isLoading)
                return;

            OnLoadAction?.Invoke();
            _isLoading = true;
            await SearchAsync(Keyword, 0);
            _isLoading = false;
            IsOpen = true;
        }

        private bool CanLoadCaption(VideoItemViewModel videoItem)
        {
            return true; //videoItem?.CaptionAssetItems?.Any() ?? false;
        }

        private async void LoadCaptionAsync(VideoItemViewModel videoItem)
        {
            IsNextButtonVisible = false;
            IsConfirmButtonVisible = true;

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
                    videoItem?.CaptionAssetItems?.Clear();
                    var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                    var stageId = _signInViewModel.SelectedStage?.Id;
                    var projectId = _signInViewModel.SelectedProject.ProjectId;

                    var result = await _cloudMediaService.GetVideoAsync(
                        new GetVideoParameter(authorization, stageId, projectId, videoId),
                        _cancellationTokenSource.Token);

                    videoItem.UpdateSource(result);
                    if (videoItem.CaptionAssetItems != null)
                        if (videoItem.CaptionAssetItems is IList<CaptionAssetItemViewModel> list)
                            list.Add(CaptionAssetItemViewModel.Empty);
                }
            }
            finally
            {
                IsBusy = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async void Refresh()
        {
            await SearchAsync(Keyword, 0);
        }

        private void Back()
        {
            SelectedVideoItem?.Update();
            SelectedVideoItem?.Initialize();
            SetTitleAction?.Invoke($"{Resource.CNT_VIDEO}");
            IsShowCaption = false;
            if (IsBusy)
                _cancellationTokenSource.Cancel();
            IsNextButtonVisible = true;
            IsConfirmButtonVisible = false;
        }

        private async void Enter(string keyword)
        {
            await SearchAsync(keyword, 0);
        }

        private async void Search(string keyword)
        {
            await SearchAsync(keyword, 0);
        }

        private async Task SearchAsync(string keyword, int pageIndex, bool isPaging = false)
        {
            var conditions = MakeSearchConditions(keyword, DurationStartTime, DurationEndTime);
            await SearchVideoAsync(pageIndex, conditions, isPaging);
        }

        private async Task SearchVideoAsync(int pageIndex, Dictionary<string, string> conditions, bool isPaging)
        {
            ValidCancellationTokenSource();
            IsBusy = true;
            try
            {
                if (!isPaging)
                {
                    SelectedPageNo = 0;
                    TotalCount = 0;
                    VideoItems?.Clear();
                }

                SelectedVideoItem = null;
                var results = await GetVideoListAsync(new Pagination(pageIndex), conditions,
                    _cancellationTokenSource.Token);

                if (results == null) return;

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

        private async Task<VideoList> GetVideoListAsync(Pagination pagination, Dictionary<string, string> conditions,
            CancellationToken cancellationToken)
        {
            try
            {
                var authorization = _signInViewModel.GetAuthorizationAsync().Result;
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;

                if (string.IsNullOrEmpty(stageId) || string.IsNullOrEmpty(projectId) ||
                    string.IsNullOrEmpty(authorization?.AccessToken))
                    return null;

                return await _cloudMediaService.GetVideosAsync(
                    new GetVideosParameter(authorization, stageId, projectId, pagination, conditions),
                    cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private Dictionary<string, string> MakeSearchConditions(string keyword, TimeSpan startDuration,
            TimeSpan endDuration)
        {
            var conditions = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(keyword))
                conditions.Add(SelectedKeywordType.Key, keyword);

            if (startDuration.TotalSeconds > 0 || endDuration.TotalSeconds > 0)
            {
                var startTime = startDuration.TotalMilliseconds;
                var endTime =
                    endDuration.TotalSeconds > startDuration.TotalSeconds
                        ? endDuration.TotalMilliseconds
                        : startDuration.TotalMilliseconds + 999;
                conditions.Add("duration", $"{startTime}~{endTime}");
            }

            return conditions;
        }

        private void OnCaptionSelectionChanged(CaptionElementItemViewModel item)
        {
            if (SelectedVideoItem == null)
                return;

            var captionAssetItem = SelectedVideoItem.CaptionAssetItems.SingleOrDefault(assetItem =>
                assetItem.Elements?.Any(element => element.Equals(item)) ?? false);

            if (!SelectedVideoItem.SelectedCaptionAsset?.Equals(captionAssetItem) ?? true)
            {
                SelectedVideoItem.CaptionAssetItems?.ToList().ForEach(asset =>
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

                if (SelectedVideoItem?.CaptionAssetItems?.Any() ?? false)
                    //if (SelectedVideoItem?.SelectedCaptionAsset != null
                    //    && SelectedVideoItem.SelectedCaptionAsset.Source == null)
                    //    return true;

                    //if (SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Any() ?? false)
                    //    return SelectedVideoItem.SelectedCaptionCount > 0;
                    //return false;

                    return SelectedVideoItem?.SelectedCaptionAsset != null;

                return false;
            }

            return false;
        }

        private void Confirm()
        {
            var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            if (subtitleVm.Tabs?.Any() ?? false)
            {
                if (subtitleVm.Tabs.Any(tab => tab.CheckDirty()))
                    if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            Resource.MSG_UNSAVED_IN_PROGRESS_OPEN_VIDEO, 
                            MessageBoxButton.OKCancel, 
                            Application.Current.MainWindow)) == MessageBoxResult.Cancel)
                        return;

                var removeTabs = subtitleVm.Tabs.ToList();
                foreach (var tab in removeTabs)
                    MessageCenter.Instance.Send(
                        new Subtitle.CloseTabMessage(this, tab as SubtitleTabItemViewModel));
            }

            // 선택된 video 정보를 메인 
            var video = SelectedVideoItem?.Source;
            var asset = SelectedVideoItem?.SelectedCaptionAsset?.Source;
            var selectedCaptionList =
                SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Where(caption => caption.IsSelected)
                    .Select(itemVm => itemVm.Source).ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new CloudMedia.CaptionOpenRequestedMessage(this,
                new CaptionOpenMessageParameter(video, asset, selectedCaptionList, true)));
            CloseAction?.Invoke();
            MessageCenter.Instance.Send(new LeftSideMenu.CloseMessage(this));
        }

        private void ValidCancellationTokenSource()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}