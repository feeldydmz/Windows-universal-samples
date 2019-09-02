using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Api.Transcoder.Domain;
using Megazone.Api.Transcoder.ServiceInterface;
using Megazone.Cloud.Aws.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Storage.ServiceInterface.S3;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.Core.Windows.Xaml.Behaviors;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;
using Subtitle = Megazone.HyperSubtitleEditor.Presentation.Message.Subtitle;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class SubtitleViewModel : ViewModelBase
    {
        // ReSharper disable once InconsistentNaming
        private const decimal MIN_INTERVAL = (decimal) 0.25;
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ExcelService _excelService;
        private readonly FileManager _fileManager;
        private readonly IJobService _jobService;
        private readonly ILogger _logger;

        private readonly TimeSpan _minimumDuration = TimeSpan.FromMilliseconds(1000);
        private readonly IList<Track> _removedTracks = new List<Track>();
        private readonly IS3Service _s3Service;
        private readonly SubtitleListItemValidator _subtitleListItemValidator;

        private readonly SubtitleParserProxy _subtitleService;
        private readonly ITrackService _trackService;
        private ICommand _addItemCommand;
        private ICommand _closeTabCommand;
        private IList<SubtitleListItemViewModel> _copiedRows;
        private ICommand _deleteSelectedItemsCommand;

        private ICommand _dropToAddSubtitleCommand;
        private IList<EncodingInfo> _encodings;

        private ICommand _goToSelectedRowCommand;

        private bool _hasRegisteredMessageHandlers;
        private bool _isCutRequest;
        private ICommand _loadedCommand;
        private decimal _previousPosition;
        private EncodingInfo _selectedEncoding;
        private ISubtitleListItemViewModel _selectedItem;
        private SubtitleTabItemViewModel _selectedTab;
        private ICommand _selectNextRowCommand;
        private ICommand _selectPreviousRowCommand;
        private ICommand _syncMediaPositionCommand;
        private IList<ISubtitleTabItemViewModel> _tabs;
        private ICommand _unloadedCommand;

        public SubtitleViewModel(SubtitleParserProxy subtitleService,
            ILogger logger,
            FileManager fileManager,
            ExcelService excelService,
            SubtitleListItemValidator subtitleListItemValidator,
            IBrowser browser,
            IS3Service s3Service,
            ITrackService trackService,
            IJobService jobService,
            ICloudMediaService cloudMediaService)
        {
            _subtitleService = subtitleService;
            _logger = logger;
            _fileManager = fileManager;
            _excelService = excelService;
            _browser = browser;
            _trackService = trackService;
            _jobService = jobService;
            _cloudMediaService = cloudMediaService;
            _subtitleListItemValidator = subtitleListItemValidator;
            _browser = browser;
            _s3Service = s3Service;

            MediaPlayer = new MediaPlayerViewModel(OnMediaPositionChanged, OnMediaPlayStateChanged);
        }

        public McmWorkContext WorkContext { get; private set; }
        public MediaPlayerViewModel MediaPlayer { get; }

        // ReSharper disable once UnusedMember.Global
        public bool IsApiEndpointSet => !string.IsNullOrEmpty(RegionManager.Instance.Current?.API);

        public ICommand DeleteSelectedItemsCommand
        {
            get
            {
                return
                    _deleteSelectedItemsCommand =
                        _deleteSelectedItemsCommand ?? new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems);
            }
        }

        public ICommand AddItemCommand
        {
            get { return _addItemCommand = _addItemCommand ?? new RelayCommand(AddItem, CanAddItem); }
        }

        public ICommand LoadedCommand
        {
            get { return _loadedCommand = _loadedCommand ?? new RelayCommand(OnLoaded); }
        }

        public ICommand UnloadedCommand
        {
            get { return _unloadedCommand = _unloadedCommand ?? new RelayCommand(OnUnloaded); }
        }

        public EncodingInfo SelectedEncoding
        {
            get => _selectedEncoding;
            set => Set(ref _selectedEncoding, value);
        }

        public IList<EncodingInfo> Encodings
        {
            get => _encodings;
            private set => Set(ref _encodings, value);
        }

        public ICommand SyncMediaPositionCommand
        {
            get { return _syncMediaPositionCommand = _syncMediaPositionCommand ?? new RelayCommand(SyncMediaPosition); }
        }

        public ISubtitleListItemViewModel SelectedItem
        {
            get => _selectedItem;
            private set => Set(ref _selectedItem, value);
        }

        public ICommand CloseTabCommand
        {
            get { return _closeTabCommand = _closeTabCommand ?? new RelayCommand<SubtitleTabItemViewModel>(CloseTab); }
        }

        public ICommand DropToAddSubtitleCommand
        {
            get
            {
                return
                    _dropToAddSubtitleCommand =
                        _dropToAddSubtitleCommand ?? new RelayCommand<object>(DropToAddSubtitle, CanDropToAddSubtitle);
            }
        }

        /// <summary>
        ///     Binding Mode: One way to source
        /// </summary>
        public IListBoxScroll ListBoxScroll { get; set; }

        public ICommand SelectPreviousRowCommand
        {
            get
            {
                return
                    _selectPreviousRowCommand =
                        _selectPreviousRowCommand ?? new RelayCommand(SelectPreviousRow, CanSelectPreviousRow);
            }
        }

        public ICommand SelectNextRowCommand
        {
            get
            {
                return
                    _selectNextRowCommand =
                        _selectNextRowCommand ?? new RelayCommand(SelectNextRow, CanSelectNextRow);
            }
        }

        public ICommand GoToSelectedRowCommand
        {
            get
            {
                return
                    _goToSelectedRowCommand =
                        _goToSelectedRowCommand ?? new RelayCommand(GoToSelectedRow, CanGoToSelectedRow);
            }
        }

        public SubtitleTabItemViewModel SelectedTab
        {
            get => _selectedTab;
            private set => Set(ref _selectedTab, value);
        }

        public IList<ISubtitleTabItemViewModel> Tabs
        {
            get => _tabs;
            set => Set(ref _tabs, value);
        }

        public bool HasTab => Tabs?.Any() ?? false;

        public int SelectedTabRowsCount => SelectedTab?.Rows?.Count ?? 0;
        public bool HasSelectedRows => (SelectedTab?.SelectedRows?.Count ?? 0) > 0;

        public bool IsMediaReady => MediaPlayer.NaturalDuration > 0;
        public bool HasCopiedRows => (_copiedRows?.Count ?? 0) > 0;

        private void GoToSelectedRow()
        {
            var lineNumber = SelectedTab?.SelectedRow?.Number;
            if (lineNumber != null)
                MessageCenter.Instance.Send(new SubtitleView.GoToLineNumberMessage(this, lineNumber.Value));
        }

        private bool CanGoToSelectedRow()
        {
            return SelectedTab?.SelectedRow != null;
        }

        private bool CanDropToAddSubtitle(object parameter)
        {
            if (!(parameter is IDataObject dataObject))
                return true;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return false;
                // 라이브러리 or 하드 드라이브는 드롭할 수 없다.
                var logicalDrives = Environment.GetLogicalDrives();
                var hasDrive = filePaths.Any(f => logicalDrives.Contains(f.ToUpper()));
                if (hasDrive)
                    return false;
                if (filePaths.Count > 1) return false;
                return !filePaths.Any(f =>
                {
                    if (Directory.Exists(f))
                        return true;
                    var fileExtension = Path.GetExtension(f);
                    return !(FileTypeDistributer.IsSupportedSubtitleFormat(fileExtension) || fileExtension == ".xlsx");
                });
            }

            return false;
        }

        private void DropToAddSubtitle(object parameter)
        {
            if (!(parameter is IDataObject dataObject)) return;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return;
                var firstFilePath = filePaths.First();
                if (File.Exists(firstFilePath))
                {
                    var fileExtension = Path.GetExtension(firstFilePath);
                    if (fileExtension == ".xlsx")
                        _browser.Main.ShowImportExcelDialog(firstFilePath);
                    else
                        _browser.Main.ShowOpenSubtitleDialog(firstFilePath);
                }
            }
        }

        private void OnMediaPlayStateChanged(MediaPlayStates obj)
        {
            if (obj == MediaPlayStates.Play)
                _previousPosition = 0;
        }

        private void OnMediaPositionChanged(decimal position)
        {
            if (SelectedTab == null) return;
            var iTextList = new List<IText>();
            try
            {
                if (Math.Abs(position - _previousPosition) < MIN_INTERVAL) return;
                _previousPosition = position;
                var totalSeconds = Convert.ToDouble(position);
                var timeSpan = TimeSpan.FromSeconds(totalSeconds);
                var rows = SelectedTab.Rows.ToList();
                foreach (var row in rows)
                {
                    row.CheckIsNowPlaying(timeSpan);
                    if (row.IsNowPlaying)
                    {
                        if (iTextList.Any())
                            iTextList.Add(new Normal
                            {
                                Text = "<br/>"
                            });
                        iTextList.AddRange(row.Texts.ToList());
                    }
                }

                MediaPlayer.CurrentPositionText = iTextList;
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
        }

        private bool CanDeleteSelectedItems()
        {
            return SelectedTab?.SelectedRow != null;
        }

        private bool CanAddItem()
        {
            return SelectedTab != null;
        }

        private void AddItem()
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void OnValidateRequested(SubtitleTabItemViewModel tab)
        {
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void DeleteSelectedItems()
        {
            SelectedTab?.DeleteSelectedItems();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<ISubtitleTabItemViewModel>();
        }

        private void OnTabSelected(SubtitleTabItemViewModel tab)
        {
            if (SelectedTab != null && !tab.Equals(SelectedTab))
                SelectedTab.IsSelected = false;
            SelectedTab = tab;
            SelectedItem = SelectedTab?.SelectedRow;
        }

        private void OnItemSelected(ISubtitleListItemViewModel row)
        {
            SelectedItem = row;
        }

        private void CloseTab(SubtitleTabItemViewModel tab)
        {
            Tabs.Remove(tab);
            if (tab.IsDeployedOnce || tab.Track != null)
                _removedTracks.Add(tab.Track);
            if (SelectedTab == null || !tab.Equals(SelectedTab)) return;
            var lastTab = Tabs.LastOrDefault();
            if (lastTab != null)
                lastTab.IsSelected = true;
            else
                SelectedTab = null;
        }

        private void SyncMediaPosition()
        {
            var selectedItem = SelectedTab?.SelectedRow;
            if (selectedItem != null && !string.IsNullOrEmpty(MediaPlayer?.MediaSource))
                MediaPlayer.SyncPosition(selectedItem.StartTime);
        }

        private void OnUnloaded()
        {
            UnregisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            if (_hasRegisteredMessageHandlers) return;
            _hasRegisteredMessageHandlers = true;
            MessageCenter.Instance.Regist<Subtitle.AutoAdjustEndtimesMessage>(OnAutoAdjustEndtimesRequested);
            MessageCenter.Instance.Regist<Subtitle.SettingsSavedMessage>(OnSettingsSaved);
            MessageCenter.Instance.Regist<JobFoundMessage>(OnJobFound);
            MessageCenter.Instance.Regist<Subtitle.SaveMessage>(OnSave);
            MessageCenter.Instance.Regist<Subtitle.SaveAllMessage>(OnSaveAll);
            MessageCenter.Instance.Regist<Subtitle.FileOpenedMessage>(OnFileOpened);
            MessageCenter.Instance.Regist<Subtitle.McmCaptionAssetOpenedMessage>(OnMcmCaptionAssetOpened);
            MessageCenter.Instance.Regist<Message.Excel.FileImportMessage>(OnImportExcelFile);
            MessageCenter.Instance.Regist<Subtitle.DeployRequestedMessage>(OnDeployRequested);
            MessageCenter.Instance.Regist<Subtitle.McmDeployRequestedMessage>(OnMcmDeployRequested);
            MessageCenter.Instance.Regist<Subtitle.DeleteTabMessage>(OnDeleteTabRequested);
            MessageCenter.Instance.Regist<MediaPlayer.OpenLocalMediaMessage>(OnOpenLocalMediaRequested);
            MessageCenter.Instance.Regist<MediaPlayer.OpenMediaFromUrlMessage>(OnOpenMediaFromUrlRequested);
            MessageCenter.Instance.Regist<Subtitle.CopyTabMessage>(OnCopySubtitle);
            MessageCenter.Instance.Regist<Subtitle.EditTabMessage>(OnEditSubtitle);

            MessageCenter.Instance.Regist<Subtitle.SyncStartTimeToCurrentMediaPositionMessage>(
                OnSyncStartTimeToCurrentMediaPositionRequested);
            MessageCenter.Instance.Regist<Subtitle.SyncEndTimeToCurrentMediaPositionMessage>(
                OnSyncEndTimeToCurrentMediaPositionRequested);

            MessageCenter.Instance.Regist<SubtitleView.RequestFindCountMessage>(OnFindCountRequested);
            MessageCenter.Instance.Regist<SubtitleView.FindTextMessage>(OnFindTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.AllFindTextMessage>(OnAllFindTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.ReplaceTextMessage>(OnReplaceTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.AllReplaceTextMessage>(OnAllReplaceTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.GoToLineNumberMessage>(OnGoToLineNumberRequested);
            MessageCenter.Instance.Regist<SubtitleView.SelectAllRowsMessage>(OnSelectAllRowsRequested);
            MessageCenter.Instance.Regist<SubtitleView.DeleteSelectRowsMessage>(OnDeleteSelectedRowsRequested);

            MessageCenter.Instance.Regist<Subtitle.AddNewRowMessage>(OnAddNewRowRequested);
            MessageCenter.Instance.Regist<Subtitle.InsertNewRowMessage>(OnInsertNewRowRequested);
            MessageCenter.Instance.Regist<Subtitle.InsertNewRowAfterSelectedRowMessage>(
                OnInsertNewRowAfterSelectedRowRequested);
            MessageCenter.Instance.Regist<Subtitle.InsertNewRowBeforeSelectedRowMessage>(
                OnInsertNewRowBeforeSelectedRowRequested);
            MessageCenter.Instance.Regist<Subtitle.CutSelectedRowsMessage>(OnCutSelectedRowRequested);
            MessageCenter.Instance.Regist<Subtitle.PasteRowsMessage>(OnPasteRowsRequested);
            MessageCenter.Instance.Regist<Subtitle.CopySelectedRowsMessage>(OnCopySelectedRowsRequested);
            MessageCenter.Instance.Regist<Subtitle.LoadTabsMessage>(OnGroupFileLoaded);
            MessageCenter.Instance.Regist<Subtitle.CopyContentsToClipboardMessage>(OnCopyContentsToClipboardRequested);
            MessageCenter.Instance.Regist<Subtitle.InsertRowAtCurrentMediaPositionMessage>(
                OnInsertRowAtCurrentMediaPositionRequested);
            MessageCenter.Instance.Regist<MediaPlayer.PlayBackByHalfSecondMessage>(OnPlayBackByHalfSecondRequested);
            MessageCenter.Instance.Regist<MediaPlayer.PlayForwardByHalfSecondMessage>(
                OnPlayForwardByHalfSecondRequested);

            MessageCenter.Instance.Regist<Subtitle.AdjustTimeMessage>(AdjustTime);
        }

        private void UnregisterMessageHandlers()
        {
            if (!_hasRegisteredMessageHandlers) return;
            MessageCenter.Instance.Unregist<Subtitle.AutoAdjustEndtimesMessage>(OnAutoAdjustEndtimesRequested);
            MessageCenter.Instance.Unregist<Subtitle.SettingsSavedMessage>(OnSettingsSaved);
            MessageCenter.Instance.Unregist<JobFoundMessage>(OnJobFound);
            MessageCenter.Instance.Unregist<Subtitle.SaveMessage>(OnSave);
            MessageCenter.Instance.Unregist<Subtitle.McmCaptionAssetOpenedMessage>(OnMcmCaptionAssetOpened);
            MessageCenter.Instance.Unregist<Subtitle.FileOpenedMessage>(OnFileOpened);
            MessageCenter.Instance.Unregist<Message.Excel.FileImportMessage>(OnImportExcelFile);
            MessageCenter.Instance.Unregist<Subtitle.DeployRequestedMessage>(OnDeployRequested);
            MessageCenter.Instance.Unregist<Subtitle.McmDeployRequestedMessage>(OnMcmDeployRequested);
            MessageCenter.Instance.Unregist<Subtitle.DeleteTabMessage>(OnDeleteTabRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.OpenLocalMediaMessage>(OnOpenLocalMediaRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.OpenMediaFromUrlMessage>(OnOpenMediaFromUrlRequested);
            MessageCenter.Instance.Unregist<Subtitle.CopyTabMessage>(OnCopySubtitle);
            MessageCenter.Instance.Unregist<Subtitle.SyncStartTimeToCurrentMediaPositionMessage>(
                OnSyncStartTimeToCurrentMediaPositionRequested);
            MessageCenter.Instance.Unregist<Subtitle.SyncEndTimeToCurrentMediaPositionMessage>(
                OnSyncEndTimeToCurrentMediaPositionRequested);

            MessageCenter.Instance.Unregist<SubtitleView.SelectAllRowsMessage>(OnSelectAllRowsRequested);
            MessageCenter.Instance.Unregist<SubtitleView.DeleteSelectRowsMessage>(OnDeleteSelectedRowsRequested);
            MessageCenter.Instance.Unregist<Subtitle.AddNewRowMessage>(OnAddNewRowRequested);
            MessageCenter.Instance
                .Unregist<Subtitle.CopyContentsToClipboardMessage>(OnCopyContentsToClipboardRequested);


            MessageCenter.Instance.Unregist<Subtitle.EditTabMessage>(OnEditSubtitle);
            MessageCenter.Instance.Unregist<Subtitle.InsertNewRowMessage>(OnInsertNewRowRequested);
            MessageCenter.Instance.Unregist<Subtitle.InsertNewRowAfterSelectedRowMessage>(
                OnInsertNewRowAfterSelectedRowRequested);
            MessageCenter.Instance.Unregist<Subtitle.InsertNewRowBeforeSelectedRowMessage>(
                OnInsertNewRowBeforeSelectedRowRequested);
            MessageCenter.Instance.Unregist<Subtitle.CutSelectedRowsMessage>(OnCutSelectedRowRequested);
            MessageCenter.Instance.Unregist<Subtitle.PasteRowsMessage>(OnPasteRowsRequested);
            MessageCenter.Instance.Unregist<Subtitle.CopySelectedRowsMessage>(OnCopySelectedRowsRequested);
            MessageCenter.Instance.Regist<Subtitle.LoadTabsMessage>(OnGroupFileLoaded);
            MessageCenter.Instance.Unregist<Subtitle.InsertRowAtCurrentMediaPositionMessage>(
                OnInsertRowAtCurrentMediaPositionRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.PlayBackByHalfSecondMessage>(OnPlayBackByHalfSecondRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.PlayForwardByHalfSecondMessage>(
                OnPlayForwardByHalfSecondRequested);

            MessageCenter.Instance.Unregist<Subtitle.AdjustTimeMessage>(AdjustTime);
        }

        public void OnAutoAdjustEndtimesRequested(Subtitle.AutoAdjustEndtimesMessage message)
        {
            var rows = SelectedTab?.Rows;
            var rowsCount = rows?.Count ?? 0;
            if (rowsCount < 1) return;
            for (var i = 0; i < rowsCount; i++)
            {
                if (rows == null) continue;
                var currentRow = rows[i];
                if (i + 1 < rowsCount)
                {
                    var nextRow = rows[i + 1];
                    currentRow.EndTime = nextRow.StartTime.Add(-TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    // last one
                    var job = AppContext.Job;
                    if (job == null) return;
                    var durationMillis =
                        job.Payload.Outputs?.FirstOrDefault()?.Duration ?? 0;
                    currentRow.EndTime = TimeSpan.FromMilliseconds(long.Parse(durationMillis.ToString()));
                }
            }
        }

        private void OnCopyContentsToClipboardRequested(Subtitle.CopyContentsToClipboardMessage message)
        {
            var contents = SelectedTab?.SelectedRow?.DisplayText;
            if (string.IsNullOrEmpty(contents)) return;
            Clipboard.SetText(contents);
        }

        private void AdjustTime(Subtitle.AdjustTimeMessage message)
        {
            if (SelectedTab == null)
                return;
            IEnumerable<ISubtitleListItemViewModel> rows;
            if (message.Way.Range == AdjustTimeRange.All)
            {
                rows = SelectedTab.Rows;
            }
            else if (message.Way.Range == AdjustTimeRange.SelectedItems)
            {
                rows = SelectedTab.SelectedRows?.Cast<ISubtitleListItemViewModel>()
                    .ToList();
            }
            else
            {
                var selectedRows = SelectedTab.SelectedRows?.Cast<ISubtitleListItemViewModel>()
                    .ToList();
                var lastSelectedItem = selectedRows?.OrderBy(i => i.Number)
                    .LastOrDefault();
                if (lastSelectedItem == null)
                    return;
                rows = selectedRows.Union(SelectedTab.Rows.Where(i => i.Number > lastSelectedItem.Number));
            }

            rows?.ForEach(i =>
            {
                var duration = i.Duration;
                if (message.Way.Behavior == AdjustTimeBehavior.Foward)
                    try
                    {
                        i.StartTime = i.StartTime.Subtract(message.Way.Time);
                    }
                    catch
                    {
                        i.StartTime = TimeSpan.Zero;
                    }
                else
                    try
                    {
                        i.StartTime = i.StartTime.Add(message.Way.Time);
                    }
                    catch
                    {
                        i.StartTime = TimeSpan.Zero;
                    }

                i.EndTime = i.StartTime.Add(duration);
            });
        }

        private void OnGroupFileLoaded(Subtitle.LoadTabsMessage message)
        {
            var tabs = message.Tabs;
            if (tabs == null)
                return;

            _subtitleListItemValidator.IsEnabled = false;

            Tabs.Clear();

            foreach (var tab in tabs)
            {
                var newTab = new SubtitleTabItemViewModel(tab.Name,
                    OnRowCollectionChanged,
                    OnValidateRequested,
                    OnTabSelected,
                    OnItemSelected,
                    tab.Kind,
                    OnDisplayTextChanged,
                    tab.LanguageCode,
                    tab.Track,
                    tab.Caption)
                {
                    IsSelected = tab.IsSelected,
                    FilePath = tab.FilePath,
                    VideoId = tab.VideoId,
                    CaptionAssetId = tab.CaptionAssetId
                };

                if (tab.Rows != null)
                    newTab.AddRows(tab.Rows.ToList());
                Tabs.Add(newTab);
            }

            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnLoaded()
        {
            RegisterMessageHandlers();

            InitializeTabs();
            InitializeEncodingItems();

            _browser.Main.LoadingManager.Show();
            this.InvokeOnTask(() =>
            {
                new AppContext().Initialize(_jobService,
                    success =>
                    {
                        this.InvokeOnUi(() =>
                        {
                            _browser.Main.LoadingManager.Hide();
                            if (!success)
                            {
                                _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                                    Resource.MSG_INVALID_APPLICATION_INFO,
                                    MessageBoxButton.OK));
                                Application.Current.Shutdown();
                            }
                        });
                    });
            });
        }

        private void OnSettingsSaved(Subtitle.SettingsSavedMessage message)
        {
            if (SelectedTab != null && (SelectedTab.Rows?.Any() ?? false))
                _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void OnPlayBackByHalfSecondRequested(MediaPlayer.PlayBackByHalfSecondMessage message)
        {
            MediaPlayer.PlayBackBy(TimeSpan.FromSeconds(0.5));
        }

        private void OnPlayForwardByHalfSecondRequested(MediaPlayer.PlayForwardByHalfSecondMessage message)
        {
            MediaPlayer.PlayForwardBy(TimeSpan.FromSeconds(0.5));
        }

        private void OnInsertRowAtCurrentMediaPositionRequested(Subtitle.InsertRowAtCurrentMediaPositionMessage message)
        {
            if (SelectedTab == null) return;
            var position = TimeSpan.FromSeconds(Convert.ToDouble(MediaPlayer.CurrentMediaPosition));
            var subtitleItem = new SubtitleItem(position,
                position + ConfigHolder.Current.Subtitle.MinDuration,
                null,
                null);
            var targetRow = SelectedTab.Rows.FirstOrDefault(row => row.StartTime > position);
            if (targetRow != null)
            {
                var index = SelectedTab.Rows.IndexOf(targetRow);
                SelectedTab.AddRow(subtitleItem, index);
            }
            else
            {
                SelectedTab.AddRow(subtitleItem);
            }

            MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, subtitleItem));
        }

        private void OnSyncEndTimeToCurrentMediaPositionRequested(
            Subtitle.SyncEndTimeToCurrentMediaPositionMessage message)
        {
            if (SelectedTab?.SelectedRow == null) return;
            var currentMediaPosition = MediaPlayer.CurrentMediaPosition;
            SelectedTab.SelectedRow.EndTime = TimeSpan.FromSeconds(Convert.ToDouble(currentMediaPosition));
        }

        private void OnSyncStartTimeToCurrentMediaPositionRequested(
            Subtitle.SyncStartTimeToCurrentMediaPositionMessage message)
        {
            if (SelectedTab?.SelectedRow == null) return;
            var currentMediaPosition = MediaPlayer.CurrentMediaPosition;
            SelectedTab.SelectedRow.StartTime = TimeSpan.FromSeconds(Convert.ToDouble(currentMediaPosition));
        }

        private void OnFindCountRequested(SubtitleView.RequestFindCountMessage message)
        {
            var findRows = SelectedTab.Rows.Where(item => item.Texts.HasFindText(message.FindText));
            message.Response?.Invoke(findRows.Count());
        }

        private void OnAddNewRowRequested(Subtitle.AddNewRowMessage message)
        {
            AddItem();
        }

        private void OnCopySelectedRowsRequested(Subtitle.CopySelectedRowsMessage message)
        {
            if (SelectedTab == null) return;
            _copiedRows = SelectedTab.SelectedRows?.Cast<SubtitleListItemViewModel>()
                .Select(r => r.Copy())
                .ToList();
            _isCutRequest = false;
        }

        private void OnCutSelectedRowRequested(Subtitle.CutSelectedRowsMessage message)
        {
            if (SelectedTab == null) return;
            _copiedRows = SelectedTab.SelectedRows?.Cast<SubtitleListItemViewModel>()
                .ToList();
            _isCutRequest = true;
            SelectedTab.DeleteSelectedItems();
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnPasteRowsRequested(Subtitle.PasteRowsMessage message)
        {
            if (_copiedRows == null || SelectedTab == null) return;
            if (_isCutRequest)
            {
                var copiedRows = _copiedRows.Select(r => r.Copy())
                    .ToList();
                if (SelectedTab.SelectedRow == null)
                    SelectedTab.AddRows(copiedRows);
                else
                    SelectedTab.AddRows(copiedRows, SelectedTab.SelectedRow.Number - 1);
                _copiedRows = null;
            }
            else
            {
                SelectedTab.AddRows(_copiedRows.Select(r => r.Copy())
                        .ToList(), SelectedTab.SelectedRow.Number - 1);
            }

            _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void OnInsertNewRowBeforeSelectedRowRequested(Subtitle.InsertNewRowBeforeSelectedRowMessage message)
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration,
                SubtitleTabItemViewModel.InsertRowDirection.BeforeSelectedItem,
                SelectedTab.SelectedRow.Number - 1);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void OnInsertNewRowAfterSelectedRowRequested(Subtitle.InsertNewRowAfterSelectedRowMessage message)
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration,
                SubtitleTabItemViewModel.InsertRowDirection.AfterSelectedItem,
                SelectedTab.SelectedRow.Number);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void OnInsertNewRowRequested(Subtitle.InsertNewRowMessage message)
        {
            AddItem();
        }

        private void OnDeleteSelectedRowsRequested(SubtitleView.DeleteSelectRowsMessage message)
        {
            if (!HasSelectedRows)
                return;

            SelectedTab.DeleteSelectedItems();
        }

        private void OnSelectAllRowsRequested(SubtitleView.SelectAllRowsMessage message)
        {
            if (SelectedTab == null) return;
            SelectedTab.Rows?.ToList()
                .ForEach(row => row.IsSelected = true);
            ListBoxScroll?.Focus();
        }

        private void OnReplaceTextRequested(SubtitleView.ReplaceTextMessage message)
        {
            Replace(message.FindText, message.ReplaceText);
        }

        private void OnAllReplaceTextRequested(SubtitleView.ReplaceTextMessage message)
        {
            ReplaceAll(message.FindText, message.ReplaceText);
        }

        private void OnFindTextRequested(SubtitleView.FindTextMessage message)
        {
            FindText(message.FindText);
        }

        private void OnAllFindTextRequested(SubtitleView.FindTextMessage message)
        {
            FindText(message.FindText, true);
        }

        private void OnGoToLineNumberRequested(SubtitleView.GoToLineNumberMessage message)
        {
            if (SelectedTab == null) return;
            var number = message.LineNumber;
            if (number < 0 || SelectedTab.Rows == null || number > SelectedTab.Rows.Count) return;
            var item = SelectedTab.Rows.FirstOrDefault(row => row.Number == number);
            ListBoxScroll?.ScrollIntoView(item, true);
        }

        private void OnOpenMediaFromUrlRequested(MediaPlayer.OpenMediaFromUrlMessage message)
        {
            MediaPlayer.OpenMedia(message.Url, false);
        }

        private void OnOpenLocalMediaRequested(MediaPlayer.OpenLocalMediaMessage message)
        {
            var supportedMediaExtensionFilters = "";
            foreach (var extension in FileTypeDistributer.SupportedVideoExtensions)
                supportedMediaExtensionFilters += $"*{extension};";
            foreach (var extension in FileTypeDistributer.SupportedAudioExtensions)
                supportedMediaExtensionFilters += $"*{extension};";
            var mediaFilePath = _fileManager.OpenFile($"Media files |{supportedMediaExtensionFilters}");
            if (string.IsNullOrEmpty(mediaFilePath)) return;
            MediaPlayer.OpenMedia(mediaFilePath, true);
        }

        private void OnDeleteTabRequested(Subtitle.DeleteTabMessage message)
        {
            CloseTab(message.Tab);
        }

        private string GetUsername(Job job)
        {
            //return job.Payload.UserMetadata["iam"];
            return job?.Payload?.UserMetadata?.Name;
        }

        private void OnMcmDeployRequested(Subtitle.McmDeployRequestedMessage message)
        {
            // 현재 정보
            //await WorkContext.DeployAsync(message.Param.Captions.ToList());
            _browser.Main.ShowMcmDeployConfirmDialog(message.Param.Video, message.Param.Asset, message.Param.Captions, GetVideoUrl());
            return;
            
            string GetVideoUrl()
            {
#if STAGE
                var hostUrl = "https://console.media.stg.continuum.co.kr"; // stage
#elif DEBUG
                var hostUrl = "http://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com"; // develop
#else
                var hostUrl = "https://console.media.megazone.io";  // Production
#endif
                return $"{hostUrl}/contents/videos/{message.Param.Video.Id}";
            }
        }

        private void OnDeployRequested(Subtitle.DeployRequestedMessage message)
        {
            _browser.Main.LoadingManager.Show();

            var dirtyTabs = Tabs.Where(tab => tab.CheckDirty())
                .ToList();
            this.InvokeOnTask(() =>
            {
                try
                {
                    var topicArn = PipelineLoader.Instance.SelectedPipeline.Notifications.Completed;
                    var job = AppContext.Job;
                    var credentialInfo = (CredentialInfo) PipelineLoader.Instance.OutputBucketCredentialInfo;
                    var isExistRemovedTracks = _removedTracks.Any();
                    if (isExistRemovedTracks)
                        DeleteRemovedTracks(credentialInfo, job, topicArn, _removedTracks);
                    var isExistDirtyTabs = dirtyTabs.Any();
                    if (isExistDirtyTabs)
                        DeployDirtyTabs(dirtyTabs, credentialInfo, job, topicArn);

                    if (isExistRemovedTracks || isExistDirtyTabs)
                        this.InvokeOnUi(() =>
                            {
                                _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                                    Resource.MSG_DEPLOY_SUCCESS,
                                    MessageBoxButton.OK));
                                CommandManager.InvalidateRequerySuggested();
                            }
                        );
                }
                catch (Exception ex)
                {
                    _logger.Error.Write(ex);
                    this.InvokeOnUi(() =>
                        {
                            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                                Resource.MSG_DEPLOY_FAIL,
                                MessageBoxButton.OK));
                        }
                    );
                }
                finally
                {
                    this.InvokeOnUi(() => { _browser.Main.LoadingManager.Hide(); });
                }
            });
        }

        private void DeployDirtyTabs(List<ISubtitleTabItemViewModel> dirtyTabs, CredentialInfo credentialInfo, Job job,
            string topicArn)
        {
            var tempFolderPath = this.GetTempFolderPath();
            var userName = GetUsername(AppContext.Job);
            foreach (var tab in dirtyTabs)
            {
                var epochTime = DateTime.UtcNow.DateTimeToEpoch();
                var newFileName = Guid.NewGuid().ToString() + epochTime;
                var filePath = tempFolderPath + "\\" + newFileName + ".temp";
                if (!Save(filePath, tab.Rows))
                    throw new Exception("Saving a temp file failed");
                var newTrack = new Track(newFileName + ".vtt",
                    tab.Name,
                    tab.Kind,
                    userName,
                    tab.LanguageCode,
                    (long) epochTime);
                var isUploaded = _trackService.UploadFile(new TrackInfo(credentialInfo,
                    credentialInfo.Region,
                    PipelineLoader.Instance.SelectedPipeline.OutputBucket,
                    job.Payload.OutputKeyPrefix,
                    newTrack,
                    filePath));
                if (!isUploaded)
                    throw new Exception("Uploading a file failed");
                try
                {
                    _trackService.Deploy(RegionManager.Instance.Current.API,
                        job.Payload.JobId,
                        topicArn,
                        newTrack,
                        tab.Track);
                }
                catch (DeleteTrackFailedException)
                {
                    // ignore
                }

                tab.SetAsDeployed();
            }
        }

        private void DeleteRemovedTracks(CredentialInfo credentialInfo, Job job, string topicArn,
            IEnumerable<Track> removedTracks)
        {
            var succeedList = new List<Track>();
            try
            {
                foreach (var removedTrack in removedTracks)
                {
                    var myTrackInfo = new TrackInfo(credentialInfo,
                        credentialInfo.Region,
                        PipelineLoader.Instance.SelectedPipeline.OutputBucket,
                        job.Payload.OutputKeyPrefix,
                        removedTrack);
                    if (!_trackService.DeleteFile(myTrackInfo))
                        throw new Exception("Deleting file failed");
                    if (!_trackService.Delete(RegionManager.Instance.Current.API,
                        job.Payload.JobId,
                        topicArn,
                        removedTrack))
                        throw new Exception("Deleting track failed");
                    succeedList.Add(removedTrack);
                }
            }
            finally
            {
                foreach (var track in succeedList)
                    _removedTracks.Remove(track);
            }
        }

        private string GetMediaItemUrl(Job job, int outputIndex = 0)
        {
            var outputList = job?.Payload?.Outputs?.ToList();
            if (outputList == null) return null;
            if (outputList.Count - 1 < outputIndex) return null;
            var output = outputList[outputIndex];
            if (output != null)
            {
                string relativeUrl;
                var outputKeyPrefix = job.Payload.OutputKeyPrefix;

                var playlist = job.Payload.Playlists?.FirstOrDefault(p => p.OutputKeys.Any(o => o == output.Key));
                if (playlist != null)
                {
                    // streaming type은 playlist봐야함 
                    var masterPlaylistName = playlist.Name;
                    var extension = playlist.Format.GetExtension();

                    relativeUrl = masterPlaylistName.Contains(extension)
                        ? $"{outputKeyPrefix}{masterPlaylistName}"
                        : $"{outputKeyPrefix}{masterPlaylistName}.{extension}";
                }
                else
                {
                    relativeUrl = $"{outputKeyPrefix}{output.Key}";
                }

                var outputBucket = PipelineLoader.Instance.SelectedPipeline.OutputBucket;
                var credentialInfo = AppContext.CredentialInfo;
                var baseUrl = _s3Service.GetUrlWith(credentialInfo.Region, outputBucket);
                return $"{baseUrl}{relativeUrl}";
            }

            return null;
        }

        private void OnJobFound(JobFoundMessage message)
        {
            if (Tabs?.Any() ?? false)
            {
                var messageBoxResult =
                    _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                        Resource.MSG_KEEP_CURRENT_TAB,
                        MessageBoxButton.YesNo));
                if (messageBoxResult != MessageBoxResult.Yes)
                {
                    Tabs.Clear();
                    SelectedTab = null;
                }
            }

            _browser.Main.LoadingManager.Show();
            this.InvokeOnTask(() =>
            {
                try
                {
                    var job = message.Job;
                    SetMediaItem(job, message.SelectedOutputIndex);
                    if (message.ShouldImportAllSubtitles)
                        LoadTabFromTrack(job);
                }
                finally
                {
                    this.InvokeOnUi(() => { _browser.Main.LoadingManager.Hide(); });
                }
            });
        }

        private void LoadTabFromTrack(Job job)
        {
            TrackList trackList;
            try
            {
                trackList = _trackService.Get(RegionManager.Instance.Current.API, job.Payload.JobId,
                    PipelineLoader.Instance.SelectedPipeline.Notifications.Completed);
                if (trackList?.Tracks == null)
                    return;
            }
            catch
            {
                // ignored
                return;
            }

            var credentialInfo = (CredentialInfo) PipelineLoader.Instance.InputBucketCredentialInfo;
            var tempFolderPath = this.GetTempFolderPath();

//#if DEBUG
//            foreach (var track in trackList.Tracks)
//            {
//                try
//                {
//                    _trackService.DeleteFile(new TrackInfo(credentialInfo, credentialInfo.Region,
//                        PipelineLoader.Instance.SelectedPipeline.OutputBucket,
//                        job.Payload.OutputKeyPrefix, track));
//                }
//                catch
//                {
//                    // ignored
//                }

//                _trackService.Delete(RegionManager.Instance.Current.API,
//                    job.Payload.JobId,
//                    PipelineLoader.Instance.SelectedPipeline.Notifications.Completed,
//                    track);
//            }
//#endif

            foreach (var track in trackList.Tracks)
                try
                {
                    var filePath = tempFolderPath + "\\" + Guid.NewGuid() + DateTime.UtcNow.DateTimeToEpoch() + ".vtt";
                    var isDownloaded =
                        _trackService.DownloadFile(new TrackInfo(credentialInfo,
                            credentialInfo.Region,
                            PipelineLoader.Instance.SelectedPipeline.OutputBucket,
                            job.Payload.OutputKeyPrefix,
                            track,
                            filePath));
                    if (isDownloaded)
                    {
                        var fileText = File.ReadAllText(filePath);
                        if (!string.IsNullOrEmpty(fileText))
                        {
                            var subtitles = _subtitleService.Load(fileText, TrackFormat.WebVtt);
                            if (subtitles == null) return;
                            this.InvokeOnUi(() =>
                            {
                                var newTab = new SubtitleTabItemViewModel(track.Label.ToUpper(),
                                    OnRowCollectionChanged,
                                    OnValidateRequested,
                                    OnTabSelected,
                                    OnItemSelected,
                                    track.Kind,
                                    OnDisplayTextChanged,
                                    track.Language,
                                    track,
                                    caption:null);
                                newTab.AddDatasheet(subtitles.ToList());
                                Tabs.Add(newTab);
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error.Write(ex);
                }

            this.InvokeOnUi(() =>
            {
                if (SelectedTab != null) return;
                var firstTab = Tabs.FirstOrDefault();
                if (firstTab != null)
                    firstTab.IsSelected = true;
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void OnDisplayTextChanged(SubtitleTabItemViewModel tab)
        {
            if (MediaPlayer.IsMediaPlaying) return;
            var nowPlayingRows = SelectedTab.Rows.Where(r => r.IsNowPlaying)
                .ToList();
            var iTextList = new List<IText>();
            foreach (var row in nowPlayingRows)
            {
                if (iTextList.Any())
                    iTextList.Add(new Normal
                    {
                        Text = "<br/>"
                    });
                var tempITextList = row.Texts.ToList();
                iTextList.AddRange(tempITextList);
            }

            MediaPlayer.CurrentPositionText = iTextList;
        }

        private void SetMediaItem(Job job, int outputIndex = 0)
        {
            if (outputIndex < 0)
            {
                this.InvokeOnUi(() => { MediaPlayer.RemoveMediaItem(); });
                return;
            }

            var url = GetMediaItemUrl(job, outputIndex);
            if (string.IsNullOrEmpty(url)) return;
            this.InvokeOnUi(() => { MediaPlayer.OpenMedia(url, false); });
        }

        private void InitializeEncodingItems()
        {
            var encodings = Encoding.GetEncodings();
            Encodings = new List<EncodingInfo>(encodings);
        }

        private async void OnSaveAll(Subtitle.SaveAllMessage message)
        {
            _browser.Main.LoadingManager.Show();
            try
            {
                foreach (var tab in Tabs)
                {
                    tab.IsSelected = true;
                    var saveFilePath = tab.FilePath;
                    if (string.IsNullOrEmpty(saveFilePath))
                    {
                        saveFilePath = _fileManager.OpenSaveFileDialog(null, "WebVtt files (.vtt)|*.vtt", tab.Name);
                        if (string.IsNullOrEmpty(saveFilePath)) return;
                    }

                    await SaveTabAsFile(tab, saveFilePath);
                }

                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_SUCCESS,
                            MessageBoxButton.OK));
                    });
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_FAIL,
                            MessageBoxButton.OK));
                    });
            }
            finally
            {
                this.InvokeOnUi(() => { _browser.Main.LoadingManager.Hide(); });
            }
        }

        private async Task SaveTabAsFile(ISubtitleTabItemViewModel tab, string saveFilePath)
        {
            var rows = tab.Rows.ToList();
            await this.CreateTask(() => { Save(saveFilePath, rows); });
            tab.FilePath = saveFilePath;
        }

        private async void OnSave(Subtitle.SaveMessage message)
        {
            var saveFilePath = SelectedTab.FilePath;
            if (string.IsNullOrEmpty(saveFilePath))
            {
                saveFilePath = _fileManager.OpenSaveFileDialog(null, "WebVtt files (.vtt)|*.vtt", SelectedTab.Name);
                if (string.IsNullOrEmpty(saveFilePath)) return;
            }

            _browser.Main.LoadingManager.Show();
            try
            {
                await SaveTabAsFile(SelectedTab, saveFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_FAIL,
                            MessageBoxButton.OK));
                    });
            }
            finally
            {
                _browser.Main.LoadingManager.Hide();
            }

            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO, Resource.MSG_SAVE_SUCCESS,
                MessageBoxButton.OK));
        }

        internal bool Save(string filePath, IList<ISubtitleListItemViewModel> rows)
        {
            //var encoding = SelectedEncoding?.GetEncoding() ?? Encoding.UTF8;
            var parser = SubtitleListItemParserProvider.Get(TrackFormat.WebVtt);
            var subtitles = rows.Select(s => s.ConvertToString(parser))
                .ToList();
            return _fileManager.Save(filePath,
                _subtitleService.ConvertToText(subtitles, TrackFormat.WebVtt),
                Encoding.UTF8);
        }


        private async void OnMcmCaptionAssetOpened(Subtitle.McmCaptionAssetOpenedMessage message)
        {
            if (message.Param == null)
                return;

            // 게시에 필요한 정보.
            var video = message.Param.Video;
            var captionAsset = message.Param.Asset;
            var captions = message.Param.Captions?.ToList() ?? new List<Caption>();

            WorkContext = new McmWorkContext(this, video, captionAsset);

            if (!string.IsNullOrEmpty(video?.Name))
                _browser.Main.SetWindowTitle($"{Resource.CNT_APPLICATION_NAME} - {video.Name}");

            if (!message.Param.Captions?.Any() ?? true)
                return;

            _browser.Main.LoadingManager.Show();

            if (MediaPlayer.MediaSource != null)
                MediaPlayer.RemoveMediaItem();

            // video영상을 가져온다.
            if (!string.IsNullOrEmpty(WorkContext.VideoMediaUrl))
                MediaPlayer.OpenMedia(WorkContext.VideoMediaUrl, false);

            var texts = await LoadCaptionTextListAsync();

            _subtitleListItemValidator.IsEnabled = false;

            foreach (var caption in captions)
            {
                var text = texts.ContainsKey(caption.Id) ? texts[caption.Id] : null;

                var newTab = new SubtitleTabItemViewModel(caption.Label,
                        OnRowCollectionChanged,
                        OnValidateRequested,
                        OnTabSelected,
                        OnItemSelected,
                        WorkContext.CaptionKind,
                        OnDisplayTextChanged,
                        caption.Language,
                        null,
                        caption)
                {
                    IsSelected = true,
                    VideoId = video?.Id,
                    CaptionAssetId = captionAsset.Id
                };

                if (!string.IsNullOrEmpty(text))
                {
                    var rows = _subtitleService.Load(text, TrackFormat.WebVtt)?.ToList();
                    if (rows != null)
                        newTab.AddRows(rows);
                }
                Tabs.Add(newTab);
            }

            _browser.Main.LoadingManager.Hide();
            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab?.Rows);
            CommandManager.InvalidateRequerySuggested();

            async Task<Dictionary<string, string>> LoadCaptionTextListAsync()
            {
                var dic = new Dictionary<string, string>();
                foreach (var caption in captions)
                {
                    try
                    {
                        var text = await _cloudMediaService.ReadAsync(new Uri(caption.Url), CancellationToken.None);
                        dic.Add(caption.Id, text);
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return dic;
            }
        }

        private void OnFileOpened(Subtitle.FileOpenedMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            _subtitleListItemValidator.IsEnabled = false;

            var subtitles = _subtitleService.Load(param.Text, TrackFormat.WebVtt);

            var newTab = new SubtitleTabItemViewModel(param.Label,
                OnRowCollectionChanged,
                OnValidateRequested,
                OnTabSelected,
                OnItemSelected,
                param.Kind,
                OnDisplayTextChanged,
                param.LanguageCode)
            {
                IsSelected = true,
                FilePath = param.FilePath,
                VideoId = WorkContext?.OpenedVideo?.Id,
                CaptionAssetId = WorkContext?.OpenedCaptionAsset?.Id,
            };
            if (subtitles != null)
                newTab.AddRows(subtitles.ToList());
            Tabs.Add(newTab);

            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnRowCollectionChanged(SubtitleTabItemViewModel tab)
        {
            _subtitleListItemValidator.Validate(tab.Rows);
        }

        //private void SetSelectedEncodingWithFileEncoding(string filePath)
        //{
        //    try
        //    {
        //        var fileEncoding = filePath.GetFileEncoding();
        //        SelectedEncoding = Encodings.FirstOrDefault(e => e.CodePage == fileEncoding.CodePage);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error.Write(ex);
        //    }
        //}

        private async void OnImportExcelFile(Message.Excel.FileImportMessage message)
        {
            _subtitleListItemValidator.IsEnabled = false;

            var filePath = message.FilePath;
            var sheetInfos = message.SheetInfos;

            _browser.Main.LoadingManager.Show();

            await this.CreateTask(() =>
            {
                IList<Excel.Subtitle> importedSubtitles =
                    _excelService.GetSheetContents(filePath, sheetInfos)
                        .ToList();

                if (importedSubtitles.All(item => !item.Datasets.Any()))
                    this.InvokeOnUi(() =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            Resource.MSG_NO_PRINTABLE_CONTENTS,
                            MessageBoxButton.OK));
                        _browser.Main.LoadingManager.Hide();
                    });

                _subtitleListItemValidator.IsEnabled = false;
                this.InvokeOnUi(() =>
                {
                    SubtitleTabItemViewModel firstTab = null;
                    foreach (var subtitle in importedSubtitles)
                    {
                        var newTab = new SubtitleTabItemViewModel(subtitle.Label,
                            OnRowCollectionChanged,
                            OnValidateRequested,
                            OnTabSelected,
                            OnItemSelected,
                            subtitle.Kind,
                            OnDisplayTextChanged,
                            subtitle.LanguageCode)
                        {
                            VideoId = WorkContext?.OpenedVideo?.Id,
                            CaptionAssetId = WorkContext?.OpenedCaptionAsset?.Id,
                        };
                        newTab.AddDatasheet(subtitle.Datasets);
                        if (firstTab == null)
                            firstTab = newTab;
                        Tabs.Add(newTab);
                    }

                    if (SelectedTab != null)
                        SelectedTab.IsSelected = false;
                    if (firstTab != null)
                        firstTab.IsSelected = true;
                    if (SelectedTab != null)
                    {
                        _subtitleListItemValidator.IsEnabled = true;
                        _subtitleListItemValidator.Validate(SelectedTab.Rows);
                    }

                    _browser.Main.LoadingManager.Hide();
                });
            });
        }

        private void OnCopySubtitle(Subtitle.CopyTabMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            _subtitleListItemValidator.IsEnabled = false;

            IList<SubtitleItem> subtitles = param.Rows.Select(row => new SubtitleItem
                {
                    Number = row.Number,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime,
                    Texts = row.Texts
                })
                .ToList();

            var newTab = new SubtitleTabItemViewModel(param.Label,
                OnRowCollectionChanged,
                OnValidateRequested,
                OnTabSelected,
                OnItemSelected,
                param.Kind,
                OnDisplayTextChanged,
                param.LanguageCode)
            {
                IsSelected = true,
                VideoId = WorkContext?.OpenedVideo?.Id,
                CaptionAssetId = WorkContext?.OpenedCaptionAsset?.Id,
            };
            if (subtitles.Any())
                newTab.AddRows(subtitles);

            Tabs.Add(newTab);

            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnEditSubtitle(Subtitle.EditTabMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            foreach (var tab in Tabs)
            {
                if (!tab.Id.Equals(param.Id))
                    continue;

                tab.Name = param.Label;
                tab.Kind = param.Kind;
                tab.LanguageCode = param.LanguageCode;
            }
        }

        private void SelectPreviousRow()
        {
            var selectedRowNumber = SelectedTab.SelectedRow.Number;
            var targetRowNumber = selectedRowNumber - 1;
            if (targetRowNumber < 0)
                targetRowNumber = 0;
            var nextRow = SelectedTab.Rows.FirstOrDefault(r => r.Number == targetRowNumber);
            SelectedTab.SelectedRow = nextRow;
        }

        private bool CanSelectPreviousRow()
        {
            if (SelectedTab == null)
                return false;
            var rowsCount = SelectedTab.Rows?.Count ?? 0;
            if (rowsCount == 0) return false;
            if (SelectedTab.SelectedRow == null || rowsCount == 1) return false;
            return SelectedTab.SelectedRow.Number > 1;
        }

        private void SelectNextRow()
        {
            var selectedRowNumber = SelectedTab.SelectedRow.Number;
            var targetRowNumber = selectedRowNumber + 1;
            if (targetRowNumber > SelectedTab.Rows.Count)
                targetRowNumber = SelectedTab.Rows.Count;
            var nextRow = SelectedTab.Rows.FirstOrDefault(r => r.Number == targetRowNumber);
            SelectedTab.SelectedRow = nextRow;
        }

        private bool CanSelectNextRow()
        {
            if (SelectedTab == null)
                return false;
            var rowsCount = SelectedTab.Rows?.Count ?? 0;
            if (rowsCount == 0) return false;
            if (SelectedTab.SelectedRow == null || rowsCount == 1) return false;
            return SelectedTab.SelectedRow.Number < rowsCount;
        }

        private void FindText(string findText, bool isAllFind = false)
        {
            if (SelectedTab?.Rows == null) return;

            if (isAllFind)
            {
                var selectedRow = SelectedTab?.SelectedRow;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;

                var selectedRows = SelectedTab.Rows.Where(item => item.Texts.HasFindText(findText))
                    .ToList();
                selectedRows.ForEach(item => item.IsSelected = true);
                SelectedTab.SelectedRows = selectedRows;
            }
            else
            {
                var selectedRow = SelectedTab?.SelectedRow;
                var nextIndex = selectedRow?.Number + 1 ?? 0;
                if (nextIndex >= SelectedTab.Rows.Count)
                    nextIndex = 0;

                var row =
                    SelectedTab.Rows.FirstOrDefault(
                        item => nextIndex <= item.Number && (item.Texts?.HasFindText(findText) ?? false));
                if (row == null) return;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;
                ListBoxScroll?.ScrollIntoView(row, true);
                ListBoxScroll?.Focus();
            }
        }

        private void ReplaceAll(string findText, string replaceText)
        {
            foreach (var row in SelectedTab.Rows)
            {
                row.Texts.ReplaceAll(findText, replaceText);
                row.Texts = row.Texts;
            }
        }

        private void Replace(string findText, string replaceText)
        {
            if (SelectedTab?.Rows == null) return;
            {
                var selectedRow = SelectedTab?.SelectedRow;
                var nextIndex = selectedRow?.Number + 1 ?? 0;
                if (nextIndex >= SelectedTab.Rows.Count)
                    nextIndex = 0;

                var row =
                    SelectedTab.Rows.FirstOrDefault(
                        item => nextIndex <= item.Number && (item.Texts?.HasFindText(findText) ?? false));

                if (row == null) return;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;

                if (row.Texts.ReplaceFirst(findText, replaceText))
                    row.Texts = row.Texts;

                ListBoxScroll?.ScrollIntoView(row, true);
                ListBoxScroll?.Focus();
            }
        }
    }
}