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
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class OpenVideoViewModel : ViewModelBase
    {
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ICommand _confirmCommand;

        private TimeSpan _durationEndTime;

        private ICommand _durationEndTimeChangedCommand;

        private TimeSpan _durationStartTime;

        private ICommand _durationStartTimeChangedCommand;

        private ICommand _enterCommand;
        private bool _isBusy;

        private bool _isConfirmButtonVisible;

        private bool _isFileOpenButtonChecked;
        private bool _isLoading;

        private bool _isMcmOpenButtonChecked;

        private bool _isNextButtonVisible = true;

        private bool _isShowCaption;

        private bool _isUrlOpenButtonChecked;

        private string _keyword;
        private IEnumerable<DisplayItem> _keywordTypeItems;

        private ICommand _loadCaptionCommand;
        private ICommand _loadCommand;

        private ICommand _nextCommand;

        private ICommand _refreshCommand;
        private ICommand _searchCommand;
        private DisplayItem _selectedKeywordType;
        private int _selectedPageNo;

        private ICommand _selectedPageNoChangedCommand;
        private VideoItemViewModel _selectedVideoItem;

        private int _totalCount;
        private IList<VideoItemViewModel> _videoItems;

        public OpenVideoViewModel(IBrowser browser, ICloudMediaService cloudMediaService,
            SignInViewModel signInViewModel)
        {
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
            _isUrlOpenButtonChecked = true;
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
            get { return _confirmCommand = _confirmCommand ?? new RelayCommand<IClosable>(Confirm, CanConfirm); }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand = _searchCommand ?? new RelayCommand<string>(Search); }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand = _refreshCommand ?? new RelayCommand(Refresh); }
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

        public bool IsUrlOpenButtonChecked
        {
            get => _isUrlOpenButtonChecked;
            set => Set(ref _isUrlOpenButtonChecked, value);
        }

        public bool IsFileOpenButtonChecked
        {
            get => _isFileOpenButtonChecked;
            set => Set(ref _isFileOpenButtonChecked, value);
        }

        public bool IsMcmOpenButtonChecked
        {
            get => _isMcmOpenButtonChecked;
            set => Set(ref _isMcmOpenButtonChecked, value);
        }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        private string _urlPath;

        public string UrlPath
        {
            get => _urlPath;
            set => Set(ref _urlPath, value);
        }


        public Action CloseAction { get; set; }
        public Action<string> SetTitleAction { get; set; }

        private bool CanNext()
        {
            return SelectedVideoItem != null;
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

        private async void Load()
        {
            KeywordTypeItems = new List<DisplayItem>
            {
                new DisplayItem(Resource.CNT_NAME, "name"),
                new DisplayItem(Resource.CNT_VIDEO_ID, "id")
            };

            if (SelectedKeywordType == null)
                SelectedKeywordType = KeywordTypeItems.First();

            _isLoading = true;
            await SearchAsync(Keyword, 0);
            _isLoading = false;
        }

        private bool CanLoadCaption(VideoItemViewModel videoItem)
        {
            return true; //videoItem?.CaptionItems?.Any() ?? false;
        }

        private async Task LoadVideoAssetAsync(VideoItemViewModel videoItem)
        {
            var videoAssetId = videoItem.Id;
            if (string.IsNullOrEmpty(videoAssetId))
                return;

            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage?.Id;
            var projectId = _signInViewModel.SelectedStage?.Id;

            //var result = await _cloudMediaService.GetVideoAsync(
            //    new GetVideoParameter(authorization, stageId, projectId, videoId),
            //    _cancellationTokenSource.Token);

            var result = await _cloudMediaService.GetVideoAssetAsync(
                new GetAssetParameter(authorization, stageId, projectId, videoAssetId), _cancellationTokenSource.Token);

            videoItem.UpdateSource(result);
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
                    TotalCount = 0;
                    VideoItems?.Clear();
                }

                SelectedVideoItem = null;

                var authorization = _signInViewModel.GetAuthorization();
                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;

                var results = await _cloudMediaService.GetVideoAssetsAsync(
                    new GetAssetsParameter(authorization, stageId, projectId, new Pagination(pageIndex), conditions),
                    _cancellationTokenSource.Token);

                //var results = await _cloudMediaService.GetVideosAsync(
                //    new GetVideosParameter(authorization, stageId, projectId, new Pagination(pageIndex),
                //        conditions), _cancellationTokenSource.Token);
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
            conditions.Add("mediaTypes", "VIDEO");

            return conditions;
        }


        private bool CanConfirm(IClosable window)
        {
            if (IsUrlOpenButtonChecked == true)
            {
                if (UrlPath.IsNotNullAndAny())
                    return true;
            }
            else if (IsFileOpenButtonChecked == true)
            {
                if (FilePath.IsNotNullAndAny())
                    return true;
            }
            else if (IsMcmOpenButtonChecked == true)
            {
                if (IsBusy)
                    return false;

                if (SelectedVideoItem != null)
                {
                    return true;
                }

                return false;
            }

            return !IsBusy && SelectedVideoItem != null;
        }

        private async void Confirm(IClosable window)
        {
            if (IsUrlOpenButtonChecked)
                OpenUrl();
            else if (IsFileOpenButtonChecked)
                OpenFilePath();
            else if (IsMcmOpenButtonChecked) await OpenMcmAsync();

            window?.Close();
        }

        private async Task OpenMcmAsync()
        {
            await LoadVideoAssetAsync(SelectedVideoItem);

            // 선택된 video 정보를 메인 
            var video = SelectedVideoItem?.Source;
            var asset = SelectedVideoItem?.SelectedCaptionAsset?.Source;
            var selectedCaptionList =
                SelectedVideoItem?.SelectedCaptionAsset?.Elements?.Where(caption => caption.IsSelected)
                    .Select(itemVm => itemVm.Source).ToList() ?? new List<Caption>();

            MessageCenter.Instance.Send(new CloudMedia.VideoOpenRequestedMessage(this, video));
        }

        private void OpenUrl()
        {
            MessageCenter.Instance.Send(new MediaPlayer.OpenMediaFromUrlMessage(this, UrlPath, false));
        }

        private void OpenFilePath()
        {
            MessageCenter.Instance.Send(new MediaPlayer.OpenMediaFromUrlMessage(this, FilePath, true));
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